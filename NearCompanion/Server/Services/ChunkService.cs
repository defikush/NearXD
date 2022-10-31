using NearCompanion.Server.Helpers;
using NearCompanion.Server.Services.Interfaces;
using NearCompanion.Shared;

namespace NearCompanion.Server.Services
{
    public class ChunkService : IChunkService
    {
        public ChunkService(IRpcService rpcService)
        {
            this.rpcService = rpcService;
        }

        private readonly IRpcService rpcService;

        public async Task<ChunkModel?> GetChunk(int id, ulong blockHeight)
        {
            try
            {
                var rpcResponse = await rpcService.MakePostRequest(RpcJsonHelpers.GetChunkJson(id, blockHeight));

                if (rpcResponse == null)
                {
                    return null;
                }
                else if (rpcResponse.Result == null)
                {
                    return null;
                }
                else if (rpcResponse.IsError)
                {
                    Console.WriteLine($"Error received when getting chunk {id} for block {blockHeight}: {rpcResponse.ErrorMessage}");
                    return new ChunkModel() 
                    { 
                        ChunkError = rpcResponse.Result
                    };
                }

                return ReadChunkFromRpcResponse(rpcResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        private ChunkModel ReadChunkFromRpcResponse(RpcResponse rpcResponse)
        {
            var chunk = new ChunkModel();

            if (rpcResponse.Result == null)
            {
                return chunk;
            }

            var result = rpcResponse.Result;

            chunk.HeightCreated = result.header.height_created;
            chunk.HeightIncluded = result.header.height_included;
            chunk.ShardId = result.header.shard_id;
            chunk.Hash = result.header.chunk_hash;
            chunk.Author = result.author;
            chunk.UtilizationPercentage = Math.Round((decimal)result.header.gas_used /
                                                     (decimal)result.header.gas_limit * 100, 1);

            foreach (var transaction in result.transactions)
            {
                var transactionModel = new TransactionModel();

                transactionModel.SignerId = transaction.signer_id;
                transactionModel.ReceiverId = transaction.receiver_id;
                transactionModel.Hash = transaction.hash;

                foreach (var action in transaction.actions)
                {
                    transactionModel.Actions.Add(ReadAction(action));
                }

                chunk.Transactions.Add(transactionModel);
            }

            //TODO read receipts

            return chunk;
        }

        private ActionModel ReadAction(dynamic action)
        {
            var actionModel = new ActionModel();

            // FunctionCall
            try
            {
                actionModel.MethodName = action.FunctionCall.method_name;
                actionModel.Deposit = action.FunctionCall.deposit;
                actionModel.Kind = ActionKind.FunctionCall;
                return actionModel;
            }
            catch (Exception) { }

            // Transfer
            try
            {
                actionModel.Deposit = action.Transfer.deposit;
                actionModel.Kind = ActionKind.Transfer;
                return actionModel;
            }
            catch (Exception) { }


            // AddKey
            try
            {
                actionModel.AccessKeyPermission = action.AddKey.access_key.permission;
                actionModel.PublicKey = action.AddKey.public_key;
                actionModel.Kind = ActionKind.AddKey;
                return actionModel;
            }
            catch (Exception) { }

            // CreateAccount
            try
            {
                if (action.CreateAccount != null)
                {
                    actionModel.Kind = ActionKind.CreateAccount;
                    return actionModel;
                }
            }
            catch (Exception) { }

            // DeleteKey
            try
            {
                actionModel.PublicKey = action.DeleteKey.public_key;
                actionModel.Kind = ActionKind.DeleteKey;
                return actionModel;
            }
            catch (Exception) { }

            // DeployContract
            try
            {
                if (action.DeployContract != null)
                {
                    actionModel.Kind = ActionKind.DeployContract;
                    return actionModel;
                }
            }
            catch (Exception) { }

            // DeleteAccount
            try
            {
                if (action.DeleteAccount != null)
                {
                    actionModel.Kind = ActionKind.DeleteAccount;
                    return actionModel;
                }
            }
            catch (Exception) { }

            //Stake
            try
            {
                if (action.Stake != null)
                {
                    actionModel.Kind = ActionKind.Stake;
                    return actionModel;
                }
            }
            catch (Exception) { }

            return actionModel;
        }
    }
}

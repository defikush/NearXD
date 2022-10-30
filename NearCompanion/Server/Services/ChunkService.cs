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
            }
            catch (Exception) { }

            // Transfer
            try
            {
                actionModel.Deposit = action.Transfer.deposit;
                actionModel.Kind = ActionKind.Transfer;
            }
            catch (Exception) { }


            // AddKey
            try
            {
                actionModel.AccessKeyPermission = action.AddKey.access_key.permission;
                actionModel.PublicKey = action.AddKey.public_key;
                actionModel.Kind = ActionKind.AddKey;
            }
            catch (Exception) { }

            // CreateAccount
            try
            {
                var createAccount = action.CreateAccount;
                actionModel.Kind = ActionKind.CreateAccount;
            }
            catch (Exception) { }

            // DeleteKey
            try
            {
                actionModel.PublicKey = action.DeleteKey.public_key;
                actionModel.Kind = ActionKind.DeleteKey;
            }
            catch (Exception) { }

            // DeployContract
            try
            {
                var deploy = action.DeployContract;
                actionModel.Kind = ActionKind.DeployContract;
            }
            catch (Exception) { }

            // DeleteAccount
            try
            {
                var deleteAccount = action.DeleteAccount;
                actionModel.Kind = ActionKind.DeleteAccount;
            }
            catch (Exception) { }

            //Stake
            try
            {
                var stake = action.Stake;
                actionModel.Kind = ActionKind.Stake;
            }
            catch (Exception) { }

            return actionModel;
        }
    }
}

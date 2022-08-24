using NearCompanion.Server.Helpers;
using NearCompanion.Server.Services.Interfaces;
using NearCompanion.Shared;
using System.Collections.ObjectModel;

namespace NearCompanion.Server.Services
{
    public class BlockService : IBlockService
    {
        public BlockService(IRpcService rpcService)
        {
            this.rpcService = rpcService;
            blocks.CollectionChanged += Blocks_CollectionChanged;
            _ = Initialize();
        }

        private IRpcService rpcService;
        private bool keepPolling = true;
        private ObservableCollection<BlockModel> blocks = new ObservableCollection<BlockModel>();

        private async Task Initialize()
        {
            var response = await GetBlock();

            await Task.Delay(2000);

            if (response.Item1 == null)
            {
                _ = Initialize();
                return;
            }

            _ = PollBlocks(response.Item1.Height + 1);
        }

        private BlockModel? ReadBlockFromResponse(dynamic? blockResult)
        {
            if (blockResult == null)
            {
                return null;
            }

            var block = new BlockModel();
            double totalGasUsage = 0;
            double totalGasLimit = 0;

            foreach (var chunk in blockResult.chunks)
            {
                var chunkModel = new ChunkModel();
                chunkModel.ShardId = chunk.shard_id;
                chunkModel.UtilizationPercentage = Math.Round((double)chunk.gas_used / (double)chunk.gas_limit, 3) * 100;
                block.Chunks.Add(chunkModel);

                totalGasUsage += (double)chunk.gas_used;
                totalGasLimit += (double)chunk.gas_limit;
            }

            block.Author = blockResult.author;
            block.Hash = blockResult.hash;
            block.Height = blockResult.header.height;
            block.GasPrice = blockResult.header.gas_price;
            block.TimestampMs = blockResult.header.timestamp / 1000000;

            if (totalGasLimit > 0)
            {
                block.UtilizationPercentage = Math.Round(totalGasUsage / totalGasLimit, 3) * 100;
            }

            return block;
        }

        private async Task<Tuple<BlockModel?, uint>> GetBlock(ulong height = 0)
        {
            try
            {
                var content = height == 0 ? RpcJsonHelpers.GetLatestFinalBlockJson() : RpcJsonHelpers.GetBlockJson(height);
                var rpcResponse = await rpcService.MakePostRequest(content);

                if (rpcResponse.IsError ||
                    rpcResponse == null ||
                    rpcResponse.Result == null)
                {
                    return null;
                }

                return Tuple.Create(ReadBlockFromResponse(rpcResponse.Result), rpcResponse.Latency);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        private async Task PollBlocks(ulong blockHeight)
        {
            try
            {
                while (keepPolling)
                {
                    BlockModel? block = new BlockModel();
                    uint latency = 0;

                    var response = await GetBlock(blockHeight);
                    block = response.Item1;
                    latency = response.Item2;

                    if (block != null)
                    {
                        var previousBlock = blocks.FirstOrDefault(b => b.Height == block.Height - 1);

                        if (previousBlock != null)
                        {
                            block.LengthMs = block.TimestampMs - previousBlock.TimestampMs;
                        }

                        blocks.Add(block);
                    }

                    blockHeight++;
                    await Task.Delay((int)block.LengthMs - (int)latency - 10); //TODO fix latency, check once in a while for the latest block delta
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }
            finally
            {
                //Handle exit
            }
        }

        private void Blocks_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (var newItem in e.NewItems)
                {
                    if (newItem is BlockModel newBlock)
                    {
                        Console.WriteLine($"New block: {newBlock.Height}, Author: {newBlock.Author}, Length: {newBlock.LengthMs}, Utilization: {newBlock.UtilizationPercentage}%. ");
                        Console.WriteLine($"Containing the chunks: ");

                        foreach (var chunk in newBlock.Chunks)
                        {
                            Console.WriteLine($"Chunk {chunk.ShardId}, Utilization: {chunk.UtilizationPercentage}%");
                        }
                    }
                }
            }
        }
    }
}

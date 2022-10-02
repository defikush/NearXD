using NearCompanion.Server.Helpers;
using NearCompanion.Server.Services.Interfaces;
using NearCompanion.Shared;
using System.Collections.ObjectModel;
using System.Timers;

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
        private double blockTimeAverageMs = 1000;
        private ulong currentPollHeight = 0;
        private float pollSpeedCoefficient = 1;
        private uint pollRebasementDelayMs = 0;
        private System.Timers.Timer heightWatchdogTimer;

        private async Task Initialize()
        {
            var response = await GetBlock();

            await Task.Delay(2000);

            if (response == null || response.Item1 == null)
            {
                _ = Initialize();
                return;
            }

            _ = PollBlocks(response.Item1.Height + 1);
        }

        private async Task<Tuple<BlockModel?, uint>?> GetBlock(ulong height = 0)
        {
            try
            {
                var content = height == 0 ? RpcJsonHelpers.GetLatestFinalBlockJson() : RpcJsonHelpers.GetBlockJson(height);
                var rpcResponse = await rpcService.MakePostRequest(content);

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
                    Console.WriteLine($"Error received when polling for block {height}: {rpcResponse.ErrorMessage}");
                    return new Tuple<BlockModel?, uint>(new BlockModel() { BlockError = rpcResponse.RpcResult }, 
                                                        rpcResponse.Latency);
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
                currentPollHeight = blockHeight;
                StartHeightWatchdog();

                while (keepPolling)
                {
                    BlockModel block = new BlockModel();
                    uint latency = 0;

                    var response = await GetBlock(currentPollHeight);

                    if (response != null && response.Item1 != null)
                    {
                        block = response.Item1;
                        latency = response.Item2;
                    }

                    if (blocks.FirstOrDefault(b => b.Height == block.Height - 1) is var previousBlock && 
                        previousBlock != null)
                    {
                        block.LengthMs = (uint)(block.TimestampMs - previousBlock.TimestampMs);
                    }

                    blocks.Add(block);
                    currentPollHeight++;

                    if (latency > block.LengthMs)
                    {
                        latency = block.LengthMs;
                    }

                    var timeUntilNextPoll = (double)(blockTimeAverageMs - latency + pollRebasementDelayMs) / pollSpeedCoefficient;
                    Console.WriteLine($"Next poll height: {currentPollHeight}, awaiting {timeUntilNextPoll}");

                    if (timeUntilNextPoll > 50000)
                    {
                        Console.WriteLine("Time until next poll very high ||||||||||||||||||||||||||||||||||");
                        Console.WriteLine($"block.LengthMs: {block.LengthMs}");
                        Console.WriteLine($"latency: {latency}");
                        Console.WriteLine($"pollRebasementDelayMs: {pollRebasementDelayMs}");
                        Console.WriteLine($"pollSpeedCoefficient: {pollSpeedCoefficient}");
                        Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
                    }

                    if (timeUntilNextPoll > 0)
                    {
                        await Task.Delay((int)timeUntilNextPoll); //TODO fix latency, check once in a while for the latest block delta
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }
            finally
            {
                _ = Initialize();
            }
        }

        private void StartHeightWatchdog()
        {
            if (heightWatchdogTimer != null)
            {
                heightWatchdogTimer.Stop();
                heightWatchdogTimer.Close();
            }

            heightWatchdogTimer = new System.Timers.Timer();
            heightWatchdogTimer.Interval = 5000;
            heightWatchdogTimer.Elapsed += HeightWatchdogElapsed;
            heightWatchdogTimer.AutoReset = true;
            heightWatchdogTimer.Enabled = true;
        }

        private void HeightWatchdogElapsed(object? sender, ElapsedEventArgs e)
        {
            _ = ReadjustHeight();
        }

        private async Task ReadjustHeight()
        {
            try
            {
                var response = await GetBlock();

                if (response != null && response.Item1 != null)
                {
                    var heightDelta = (int)(response.Item1.Height - currentPollHeight);

                    Console.WriteLine($"[WATCHDOG] =============================================================================================================== CURRENT DELTA: {heightDelta}");

                    // Final block is higher than current poll
                    if (heightDelta >= 5)
                    {
                        pollRebasementDelayMs = 0;
                        pollSpeedCoefficient = Math.Min(pollSpeedCoefficient * 1.2f, 1.5f);
                        return;
                    }
                    
                    // Final block is lower than current poll
                    if (heightDelta < 0)
                    {
                        pollSpeedCoefficient = 1;
                        pollRebasementDelayMs = (uint)(Math.Abs(heightDelta) * (uint)blockTimeAverageMs);
                        return;
                    }

                    pollSpeedCoefficient = 1;
                    pollRebasementDelayMs = 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private BlockModel? ReadBlockFromResponse(dynamic? blockResult)
        {
            if (blockResult == null)
            {
                return null;
            }

            var block = new BlockModel();
            decimal totalGasUsage = 0;
            decimal totalGasLimit = 0;

            foreach (var chunk in blockResult.chunks)
            {
                var chunkModel = new ChunkModel();
                chunkModel.ShardId = chunk.shard_id;
                chunkModel.UtilizationPercentage = Math.Round(((decimal)chunk.gas_used / (decimal)chunk.gas_limit) * 100, 1);
                block.Chunks.Add(chunkModel);

                totalGasUsage += (decimal)chunk.gas_used;
                totalGasLimit += (decimal)chunk.gas_limit;
            }

            block.Author = blockResult.author;
            block.Hash = blockResult.hash;
            block.Height = blockResult.header.height;
            block.GasPrice = blockResult.header.gas_price;
            block.TimestampMs = blockResult.header.timestamp / 1000000;

            if (totalGasLimit > 0)
            {
                block.UtilizationPercentage = Math.Round((totalGasUsage / totalGasLimit) * 100, 1);
            }

            return block;
        }

        public Response<BlockModel> GetIntroductionBlock()
        {
            if (blocks.Count == 0)
            {
                return new Response<BlockModel>()
                {
                    Data = blocks[blocks.Count - 5],
                    Error = Errors.NoBlocks
                };
            }
            else if (blocks.Count >= 5)
            {
                return new Response<BlockModel>() 
                {
                    Data = blocks[blocks.Count - 5],
                };
            }
            else
            {
                return new Response<BlockModel>()
                {
                    Data = null,
                    Error = Errors.NotEnoughBlocks
                };
            }
        }

        public List<BlockModel> GetLatestBlocks(ulong afterHeight, ref Errors result)
        {
            if (blocks.FirstOrDefault(b => b.Height == afterHeight) is var rootBlock && 
                rootBlock != null)
            {
                var rootIndex = blocks.IndexOf(rootBlock);
                return blocks.ToList().GetRange(rootIndex, blocks.Count - rootIndex - 1);
            }

            result = Errors.UnknownBlock;
            return new List<BlockModel>();
        }

        private void Blocks_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems == null)
                {
                    return;
                }

                foreach (var newItem in e.NewItems)
                {
                    if (newItem is BlockModel newBlock)
                    {
                        Console.WriteLine($"New block: {newBlock.Height}, Author: {newBlock.Author}, Length: {newBlock.LengthMs}, Utilization: {newBlock.UtilizationPercentage}%. ");
                        //Console.WriteLine($"Containing the chunks: ");

                        //foreach (var chunk in newBlock.Chunks)
                        //{
                        //    Console.WriteLine($"Chunk {chunk.ShardId}, Utilization: {chunk.UtilizationPercentage}%");
                        //}
                    }
                }

                if (blocks.Count > 30)
                {
                    blocks.RemoveAt(0);
                }

                blockTimeAverageMs = blocks.Average(b => (long)b.LengthMs);
            }
        }
    }
}

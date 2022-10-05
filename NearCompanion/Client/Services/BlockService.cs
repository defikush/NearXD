using NearCompanion.Client.Services.Interfaces;
using NearCompanion.Shared;
using System.Net.Http.Json;

namespace NearCompanion.Client.Services
{
    public class NewBlocksReceivedEventArgs : EventArgs
    {
        public NewBlocksReceivedEventArgs(List<BlockModel> blocks)
        {
            Blocks = blocks;
        }

        public List<BlockModel> Blocks { get; set; } = null;
    }

    public class BlockService : IBlockService
    {
        public BlockService(HttpClient http)
        {
            httpClient = http;
        }

        private HttpClient httpClient;
        private bool keepQueryingBlocks = true;

        public event EventHandler<NewBlocksReceivedEventArgs>? NewBlocksReceivedEvent;

        public async Task StartReceivingBlocks()
        {
            var finalBlockResponse = await httpClient.GetFromJsonAsync<Response<BlockModel>>("block");

            if (finalBlockResponse == null || !finalBlockResponse.Success || finalBlockResponse.Data == null)
            {
                // No final block, server might be down, keep polling every few seconds
                await Task.Delay(5000);
                _ = StartReceivingBlocks();
                return;
            }

            ulong previousHeight = finalBlockResponse.Data.Height + 1;

            NewBlocksReceivedEvent?.Invoke(null, new NewBlocksReceivedEventArgs(new List<BlockModel>() { finalBlockResponse.Data }));

            while (keepQueryingBlocks)
            {
                Console.WriteLine($"Polling blocks after height: {previousHeight}");

                var latestBlocksResponse = await httpClient.GetFromJsonAsync<Response<List<BlockModel>>>($"block/{previousHeight}");

                if (latestBlocksResponse == null || 
                    latestBlocksResponse.Data == null)
                {
                    Console.WriteLine($"Received no blocks response...");
                    await Task.Delay(2000);
                    continue;
                }
                else if (!latestBlocksResponse.Success)
                {
                    Console.WriteLine($"Latest blocks response returned error {latestBlocksResponse.Error}...");

                    if (latestBlocksResponse.Error == Errors.UnknownBlock)
                    {
                        previousHeight++;
                        await Task.Delay(1000);
                        continue;
                    }
                }

                foreach (var newBlock in latestBlocksResponse.Data)
                {
                    previousHeight++;
                }

                Console.WriteLine($"Received {latestBlocksResponse.Data.Count} blocks after height {previousHeight}");

                NewBlocksReceivedEvent?.Invoke(null, new NewBlocksReceivedEventArgs(latestBlocksResponse.Data));

                await Task.Delay(5000);
            }
        }

        public Task StopReceivingBlocks()
        {
            throw new NotImplementedException();
        }
    }
}

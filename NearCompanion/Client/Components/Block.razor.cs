using Microsoft.AspNetCore.Components;
using NearCompanion.Client.Services.Interfaces;
using NearCompanion.Shared;

namespace NearCompanion.Client.Components
{
    public partial class Block : ComponentBase
    {
        private string authorTrimmed = string.Empty;

        [Inject]
        private IChunkService? chunkService { get; set; }

        [Parameter]
        public BlockModel? Model { get; set; } = null;

        private string GetAuthorTrimmed()
        {
            if (Model == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrEmpty(authorTrimmed))
            {
                return authorTrimmed;
            }

            if (!string.IsNullOrEmpty(Model.Author))
            {
                var split = Model.Author.Split('.');

                if (split.Count() > 0)
                {
                    authorTrimmed = split[0];
                }
                else
                {
                    authorTrimmed = Model.Author;
                }
            }

            return authorTrimmed = string.IsNullOrEmpty(authorTrimmed) ? "unknown" : authorTrimmed;
        }

        private async void HandleChunkClick(int id, ulong height)
        {
            try
            {
                var result = await chunkService.GetChunk(id, height);
                Console.WriteLine($"Received chunk: {result.ShardId} at height created {result.HeightCreated} and height included {result.HeightIncluded} with " +
                    $"hash {result.Hash} and author {result.Author} and utilization {result.UtilizationPercentage} chunk error {result.ChunkError}");

                foreach (var transaction in result.Transactions)
                {
                    Console.WriteLine($"Transaction by signer: {transaction.SignerId} to {transaction.ReceiverId} and hash {transaction.Hash} with the following Actions:");

                    foreach (var action in transaction.Actions)
                    {
                        Console.WriteLine($"~~~ Type: {action.Kind} method called: {action.MethodName} deposit: {action.Deposit}, " +
                            $"access key permission: {action.AccessKeyPermission}, public key {action.PublicKey}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}

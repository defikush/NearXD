using NearCompanion.Client.Services.Interfaces;
using NearCompanion.Shared;
using System.Net.Http.Json;

namespace NearCompanion.Client.Services
{
    public class ChunkService : IChunkService
    {
        public ChunkService(HttpClient http)
        {
            httpClient = http;
        }

        private readonly HttpClient httpClient;

        public async Task<ChunkModel> GetChunk(int id, ulong height)
        {
            try
            {
                var chunkResponse = await httpClient.GetFromJsonAsync<Response<ChunkModel>>($"chunk/{height}/{id}");

                if (chunkResponse == null)
                {
                    return new ChunkModel() { ChunkError = Errors.InternalError };
                }
                else if (!chunkResponse.Success || chunkResponse.Data == null)
                {
                    return new ChunkModel() { ChunkError = chunkResponse.Error };
                }

                return chunkResponse.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ChunkModel() { ChunkError = Errors.InternalError };
            }
        }
    }
}

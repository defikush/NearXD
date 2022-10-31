using NearCompanion.Shared;

namespace NearCompanion.Client.Services.Interfaces
{
    public interface IChunkService
    {
        Task<ChunkModel> GetChunk(int id, ulong height);
    }
}

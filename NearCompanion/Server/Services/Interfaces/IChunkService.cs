using NearCompanion.Shared;

namespace NearCompanion.Server.Services.Interfaces
{
    public interface IChunkService
    {
        Task<ChunkModel?> GetChunk(int id, ulong blockHeight);
    }
}

using NearCompanion.Shared;

namespace NearCompanion.Server.Services.Interfaces
{
    public interface IBlockService
    {
        BlockModel? GetIntroductionBlock();
        List<BlockModel> GetLatestBlocks(ulong afterHeight);
    }
}

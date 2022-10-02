using NearCompanion.Shared;

namespace NearCompanion.Server.Services.Interfaces
{
    public interface IBlockService
    {
        Response<BlockModel> GetIntroductionBlock();
        List<BlockModel> GetLatestBlocks(ulong afterHeight, ref Errors result);
    }
}

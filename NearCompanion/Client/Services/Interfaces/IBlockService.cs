using NearCompanion.Shared;

namespace NearCompanion.Client.Services.Interfaces
{
    public interface IBlockService
    {
        event EventHandler<List<BlockModel>> NewBlocksReceivedEvent;

        Task StartReceivingBlocks();
        Task StopReceivingBlocks();
    }
}

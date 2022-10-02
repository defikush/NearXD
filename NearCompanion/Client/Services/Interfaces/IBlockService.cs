using NearCompanion.Shared;

namespace NearCompanion.Client.Services.Interfaces
{
    public interface IBlockService
    {
        event EventHandler<NewBlocksReceivedEventArgs> NewBlocksReceivedEvent;

        Task StartReceivingBlocks();
        Task StopReceivingBlocks();
    }
}

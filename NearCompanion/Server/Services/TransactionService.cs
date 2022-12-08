using NearCompanion.Server.Services.Interfaces;

namespace NearCompanion.Server.Services
{
    public class TransactionService : ITransactionService
    {
        public TransactionService(IRpcService rpcService)
        {
            this.rpcService = rpcService;
        }

        private readonly IRpcService rpcService;
    }
}

using NearCompanion.Client.Services.Interfaces;

namespace NearCompanion.Client.Services
{
    public class BlockService : IBlockService
    {
        public BlockService(HttpClient http)
        {
            httpClient = http;
        }

        private HttpClient httpClient;


    }
}

namespace NearCompanion.Server.Services.Interfaces
{
    public interface IRpcService
    {
        Task<RpcResponse> MakePostRequest(string content);
    }
}

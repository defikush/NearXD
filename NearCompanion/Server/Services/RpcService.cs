using Microsoft.CSharp.RuntimeBinder;
using NearCompanion.Server.Helpers;
using NearCompanion.Server.Services.Interfaces;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace NearCompanion.Server.Services
{
    public class RpcService : IRpcService
    {
        public RpcService()
        {

        }

        private readonly string unknownError = "Unknown RPC Error";
        private readonly string nullError = "The RPC server response was null";
        private readonly string unableError = "Unable to read the Rpc response";

        public Uri RpcUri => new Uri("https://rpc.mainnet.near.org");

        public async Task<RpcResponse> MakePostRequest(string content)
        {
            dynamic? response = null;
            var latencyStopwatch = Stopwatch.StartNew();

            try
            {
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = RpcUri,
                        Content = new StringContent(content, Encoding.ASCII, "application/json")
                    };

                    var responseMessage = await client.SendAsync(request);

                    var rawResponse = await responseMessage.Content.ReadAsStringAsync();

                    if (rawResponse != null)
                    {
                        response = JsonConvert.DeserializeObject(rawResponse);

                        if (response != null)
                        {
                            return new RpcResponse(response.result, false, null, (uint)latencyStopwatch.ElapsedMilliseconds);
                        }
                        else
                        {
                            return new RpcResponse(null, true, nullError, 0);
                        }
                    }
                    else
                    {
                        return new RpcResponse(null, true, nullError, 0);
                    }
                }
            }
            catch (RuntimeBinderException rbe)
            {
                Console.WriteLine(rbe);
                return new RpcResponse(null, true, ReadErrorResponse(response), (uint)latencyStopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new RpcResponse(null, true, unknownError, 0);
            }
            finally
            {
                latencyStopwatch.Stop();
            }
        }

        private string ReadErrorResponse(dynamic? response)
        {
            try
            {
                if (response != null)
                {
                    return response.error.cause.name;
                }
                else
                {
                    return nullError;
                }
            }
            catch (RuntimeBinderException rbe)
            {
                Console.WriteLine(rbe);
                return unableError;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return unknownError;
            }
        }
    }

    public class RpcResponse
    {
        public RpcResponse(dynamic? result, bool isError, string errorMessage, uint latency)
        {
            Result = result;
            IsError = isError;
            ErrorMessage = errorMessage;
            Latency = latency;
        }

        public bool IsError { get; } = false;
        public string ErrorMessage { get; } = string.Empty;
        public dynamic? Result { get; } = null;
        public uint Latency { get; }
    }
}

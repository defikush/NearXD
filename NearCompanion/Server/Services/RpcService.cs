using Microsoft.CSharp.RuntimeBinder;
using NearCompanion.Server.Helpers;
using NearCompanion.Server.Services.Interfaces;
using NearCompanion.Shared;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;

namespace NearCompanion.Server.Services
{
    public class RpcService : IRpcService
    {
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

                    var responseMessage = await client.SendAsync(request).WithTimeout(4000);

                    var rawResponse = await responseMessage.Content.ReadAsStringAsync();

                    if (rawResponse != null)
                    {
                        response = JsonConvert.DeserializeObject(rawResponse);

                        if (response != null)
                        {
                            return new RpcResponse(response.result,
                                                   Errors.None,
                                                   (uint)latencyStopwatch.ElapsedMilliseconds);
                        }
                        else
                        {
                            return new RpcResponse(null, Errors.DeserializationError, (uint)latencyStopwatch.ElapsedMilliseconds);
                        }
                    }
                    else
                    {
                        return new RpcResponse(null, Errors.NullResponse, (uint)latencyStopwatch.ElapsedMilliseconds);
                    }
                }
            }
            catch (TimeoutException te)
            {
                Console.WriteLine(te);
                return new RpcResponse(null, Errors.Timeout, (uint)latencyStopwatch.ElapsedMilliseconds);
            }
            catch (RuntimeBinderException rbe)
            {
                Console.WriteLine(rbe);
                return new RpcResponse(null, ReadErrorResponse(response), (uint)latencyStopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new RpcResponse(null, Errors.UnknownError, (uint)latencyStopwatch.ElapsedMilliseconds);
            }
            finally
            {
                latencyStopwatch.Stop();
            }
        }

        private Errors ReadErrorResponse(dynamic? response)
        {
            try
            {
                if (response != null)
                {
                    switch (response.error.cause.name)
                    {
                        case "UNKNOWN_BLOCK":
                        {
                            return Errors.UnknownBlock;
                        }

                        case "UNKNOWN_CHUNK":
                        {
                            return Errors.UnknownChunk;
                        }

                        case "INVALID_SHARD_ID":
                        {
                            return Errors.InvalidShardId;
                        }

                        case "NOT_SYNCED_YET":
                        {
                            return Errors.NotSyncedYet;
                        }

                        case "PARSE_ERROR":
                        {
                            return Errors.ParseError;
                        }

                        case "INTERNAL_ERROR":
                        {
                            return Errors.InternalError;
                        }

                        default:
                        {
                            return Errors.UnknownError;
                        }
                    }
                }
                else
                {
                    return Errors.NullResponse;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Errors.UnknownError;
            }
        }
    }

    public class RpcResponse
    {
        public RpcResponse(dynamic? data, Errors result, uint latency)
        {
            Result = data;
            Latency = latency;
            RpcResult = result;
        }

        public dynamic? Result { get; } = null;
        public uint Latency { get; }
        public bool IsError => RpcResult != Errors.None;
        public Errors RpcResult { get; }
        public string ErrorMessage
        {
            get
            {
                if (RpcResult == Errors.None)
                {
                    return string.Empty;
                }    

                return EnumExtensions.GetEnumDescription(RpcResult);
            }
        }
    }
}

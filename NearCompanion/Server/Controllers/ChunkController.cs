using Microsoft.AspNetCore.Mvc;
using NearCompanion.Server.Services.Interfaces;
using NearCompanion.Shared;

namespace NearCompanion.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ChunkController : ControllerBase
    {
        public ChunkController(IChunkService chunkService)
        {
            this.chunkService = chunkService;
        }

        private readonly IChunkService chunkService;

        [HttpGet("{height}/{id}")]
        public async Task<ActionResult<Response<ChunkModel>>> GetChunk(ulong height, int id)
        {
            var response = new Response<ChunkModel>();
            var chunkResponse = await chunkService.GetChunk(id, height);

            if (chunkResponse == null)
            {
                response.Error = Errors.InternalError;
            }
            else if (chunkResponse.ChunkError != Errors.None)
            {
                response.Error = chunkResponse.ChunkError;
            }
            else
            {
                response.Data = chunkResponse;
            }

            return Ok(response);
        }
    }
}

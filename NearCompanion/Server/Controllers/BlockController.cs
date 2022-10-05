using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NearCompanion.Server.Services.Interfaces;
using NearCompanion.Shared;

namespace NearCompanion.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BlockController : ControllerBase
    {
        private readonly IBlockService blockService;

        public BlockController(IBlockService blockService)
        {
            this.blockService = blockService;
        }

        [HttpGet]
        public async Task<ActionResult<Response<BlockModel>>> GetIntroductionBlock()
        {
            var introductionBlockResponse = blockService.GetIntroductionBlock();
            return Ok(introductionBlockResponse);
        }

        [HttpGet("{afterHeight}")]
        public async Task<ActionResult<Response<List<BlockModel>>>> GetLatestBlocks(ulong afterHeight)
        {
            var operationResult = Errors.None;
            var latestBlocksResponse = blockService.GetLatestBlocks(afterHeight, ref operationResult);
            return Ok(latestBlocksResponse);
        }
    }
}

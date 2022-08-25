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
            var introductionBlock = blockService.GetIntroductionBlock();
            return Ok(new Response<BlockModel>() { Data = introductionBlock });
        }

        [HttpGet("{afterHeight}")]
        public async Task<ActionResult<Response<List<BlockModel>>>> GetLatestBlocks(ulong afterHeight)
        {
            var blocks = blockService.GetLatestBlocks(afterHeight);
            return Ok(new Response<List<BlockModel>>() { Data = blocks });
        }
    }
}

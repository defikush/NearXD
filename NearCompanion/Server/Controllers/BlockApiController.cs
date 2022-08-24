using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NearCompanion.Server.Services.Interfaces;

namespace NearCompanion.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BlockApiController : ControllerBase
    {
        private readonly IBlockService blockService;

        public BlockApiController(IBlockService blockService)
        {
            this.blockService = blockService;
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetBlock()
        {
            return Ok("21344");
        }
    }
}

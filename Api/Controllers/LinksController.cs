using System.Linq;
using System.Threading.Tasks;
using LinkVault.Context;
using Microsoft.AspNetCore.Mvc;
using Splat;

namespace LinkVault.Api.Controllers
{


    [Route("/")]
    [ApiController]
    public class LinksController : ControllerBase
    {
        private AppDbContext Context { get; }

        public LinksController(AppDbContext context)
        {
            Context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(Context.Links.ToList());
        }
    }
}
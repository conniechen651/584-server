using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolModel;

namespace _584_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController(SchoolDbContext context) : ControllerBase
    {
        [HttpPost("Districts")]
        public async Task<ActionResult> PostDistricts()
        {
            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("Schools")]
        public async Task<ActionResult> PostSchools()
        {
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
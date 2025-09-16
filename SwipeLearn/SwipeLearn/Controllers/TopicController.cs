using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwipeLearn.Context;
using SwipeLearn.Models;

namespace SwipeLearn.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopicController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TopicController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTopics()
        {
            return Ok(await _context.Topics.ToListAsync());
        }

    }
}

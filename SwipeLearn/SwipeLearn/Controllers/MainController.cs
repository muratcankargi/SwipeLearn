using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwipeLearn.Context;
using SwipeLearn.Models;
using SwipeLearn.Services;

namespace SwipeLearn.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopicController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly MainService _service;
        public TopicController(ApplicationDbContext context, MainService service)
        {
            _context = context;
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(Array), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTopics()
        {
            return Ok(await _context.Topics.ToListAsync());
        }

        //create topic 
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [HttpPost]
        public async Task<ActionResult> AddTopic(Topic topic)
        {
            var guid = await _service.CreateTopic(topic);
            //if (guid == Guid.Empty) return BadRequest("Empty or exist description"); //farklı düşün

            return Ok(new { id = guid });
        }

        [HttpGet("short-info")]
        public async Task<IActionResult> GetShortInfo([FromQuery] Guid id)
        {
            //var result = await _service.GetShortInfoAsync(id);
            //if (result == null)
            //    return NotFound(new { message = "Topic not found" });

            //return Ok(result);
        }
    }
}

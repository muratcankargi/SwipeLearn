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
        private readonly TopicService _service;
        public TopicController(ApplicationDbContext context, TopicService service)
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
        [HttpPost]
        public async Task<ActionResult> AddTopic(Topic topic)
        {
            var (guid, text, urls) = await _service.Create(topic);
            //if (guid == Guid.Empty) return BadRequest("Empty or exist description");

            return Ok(new { guid = guid, text = text, image_urls = urls });
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwipeLearn.Context;
using SwipeLearn.Models;
using SwipeLearn.Models.ViewModels;
using SwipeLearn.Services;

namespace SwipeLearn.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopicController : Controller
    {
        private readonly MainService _service;
        public TopicController(MainService service)
        {
            _service = service;
        }

        //create topic 
        [HttpPost]
        [ProducesResponseType(typeof(TopicGuid), StatusCodes.Status200OK)]
        public async Task<ActionResult> AddTopic(Topic topic)
        {
            var guid = await _service.CreateTopic(topic);
            //if (guid == Guid.Empty) return BadRequest("Empty or exist description"); //farklı düşün

            return Ok(guid);
        }
        
        [HttpGet("short-info")]
        [ProducesResponseType(typeof(TopicInfoItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetShortInfo([FromQuery] Guid id)
        {
            TopicInfoItem arr = await _service.GetStructuredTopicInfoAsync(id);
            if (arr == null) return NotFound();
            return Ok(arr);
        }

       
    }
}

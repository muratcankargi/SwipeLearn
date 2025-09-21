using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwipeLearn.Context;
using SwipeLearn.Models;
using SwipeLearn.Models.ViewModels;
using SwipeLearn.Services;
using SwipeLearn.Utils;
using System.Net;

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

        [HttpGet("is-videos-ready")]
        [ProducesResponseType(typeof(IsReady), StatusCodes.Status200OK)]
        public async Task<IActionResult> IsVideosReady([FromQuery] Guid id)
        {
            bool isReady = await _service.IsVideosReady(id);
            return Ok(new { isReady = isReady });
        }

        [HttpGet("/api/video")]
        [ProducesResponseType(typeof(VideoUrls), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetVideoByTopicId([FromQuery] Guid id)
        {
            VideoUrls videoPaths = await _service.GetVideoByTopicId(id);
            return videoPaths != null ? Ok(videoPaths) : NotFound();
        }


        [HttpGet("quiz")]
        [ProducesResponseType(typeof(QuizResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetQuiz([FromQuery] Guid id)
        {
            var quiz = await _service.GetQuizByTopicIdAsync(id);
            if (quiz.Questions.Count == 0)
                return NotFound();
            if (quiz.Questions.Count == 3)
                return Ok(quiz);
            return BadRequest();
        }

        [HttpPost("quiz")]
        [ProducesResponseType(typeof(QuizAnswerResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QuizAnswerResponse>> CheckQuizAnswer([FromBody] QuizAnswerRequest request)
        {
            QuizAnswerResponse result = await _service.CheckAnswerAsync(request);
            if (result.CorrectOptionIndex == -1) return StatusCode(500, new { error = "Something went wrong." });
            return Ok(result);
        }

        [HttpGet("/api/explore")]
        [ProducesResponseType(typeof(ListPagination<Topic>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetTopics()
        {
            var topics = await _service.GetTopics();
            return Ok(topics);
        }

    }
}

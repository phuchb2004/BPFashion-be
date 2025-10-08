using Microsoft.AspNetCore.Mvc;
using API_shop.Models;
using API_shop.Services;

namespace API_shop.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly ChatbotService _chatbotService;

        public ChatbotController(ChatbotService chatbotService)
        {
            _chatbotService = chatbotService;
        }

        [HttpPost("process")]
        public IActionResult ProcessMessage([FromBody] ChatMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.Text))
            {
                return BadRequest("Message text cannot be empty.");
            }

            try
            {
                Task.Delay(500 + new Random().Next(500)).Wait();

                var response = _chatbotService.ProcessMessage(message.Text);
                response.Timestamp = DateTime.UtcNow;

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An internal server error occurred." });
            }
        }
    }
}
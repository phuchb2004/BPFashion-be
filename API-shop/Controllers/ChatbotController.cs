using Microsoft.AspNetCore.Mvc;
using API_shop.Models;
using API_shop.Services;
using System.Threading.Tasks;

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
        public async Task<IActionResult> ProcessMessage([FromBody] ChatMessage message)
        {
            if (message == null || string.IsNullOrWhiteSpace(message.Text))
            {
                return BadRequest("Nội dung tin nhắn không được để trống.");
            }

            try
            {
                // Giả lập độ trễ để tạo cảm giác bot đang gõ
                await Task.Delay(500 + new Random().Next(500));

                var response = _chatbotService.ProcessMessage(message.Text);
                response.Timestamp = DateTime.UtcNow;

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log error here if needed
                return StatusCode(500, new { error = "Đã xảy ra lỗi máy chủ." });
            }
        }
    }
}
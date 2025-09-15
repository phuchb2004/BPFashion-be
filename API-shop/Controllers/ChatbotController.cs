using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_shop.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        [HttpPost("process")]
        public async Task<IActionResult> ProcessMessage([FromBody] ChatMessage message)
        {
            try
            {
                var response = await ProcessChatMessage(message.Text);

                return Ok(new ChatResponse
                {
                    Response = response,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        private async Task<string> ProcessChatMessage(string message)
        {
            await Task.Delay(500 + new Random().Next(1000));

            message = message.ToLower();

            if (message.Contains("xin chào") || message.Contains("hello") || message.Contains("hi"))
                return "Xin chào! Rất vui được gặp bạn. Tôi có thể giúp gì cho bạn?";

            if (message.Contains("cảm ơn"))
                return "Không có gì! Rất vui được giúp đỡ bạn.";

            if (message.Contains("tạm biệt") || message.Contains("bye"))
                return "Tạm biệt! Hy vọng bạn sẽ quay lại sớm.";

            if (message.Contains("giá") || message.Contains("giá cả"))
                return "Bạn đang quan tâm đến sản phẩm nào? Tôi có thể cung cấp thông tin giá cả chi tiết.";

            if (message.Contains("sản phẩm") || message.Contains("mặt hàng"))
                return "Chúng tôi có nhiều loại sản phẩm đa dạng. Bạn quan tâm đến danh mục nào?";

            if (message.Contains("hỗ trợ") || message.Contains("giúp đỡ"))
                return "Tôi ở đây để giúp bạn! Hãy cho tôi biết vấn đề bạn đang gặp phải.";

            if (message.Contains("website") || message.Contains("trang web"))
                return "Bạn có thể truy cập trang web chính thức của chúng tôi để biết thêm thông tin chi tiết.";

            return "Xin lỗi, tôi chưa hiểu câu hỏi của bạn. Bạn có thể diễn đạt theo cách khác hoặc liên hệ với bộ phận hỗ trợ trực tiếp?";
        }
    }

    public class ChatMessage
    {
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class ChatResponse
    {
        public string Response { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

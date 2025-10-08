using System.Text.RegularExpressions;
using API_shop.Models;

namespace API_shop.Services
{
    public class ChatbotRule
    {
        public List<string> Keywords { get; set; }
        public Func<string, string> GetResponse { get; set; }
        public List<string> QuickReplies { get; set; } = new List<string>();
    }

    public class ChatbotService
    {
        private readonly List<ChatbotRule> _rules;

        public ChatbotService()
        {
            _rules = new List<ChatbotRule>
            {
                new ChatbotRule
                {
                    Keywords = new List<string> { "xin chào", "hello", "hi", "chào shop" },
                    GetResponse = message => "Chào bạn! Shop thời trang ABC rất vui được hỗ trợ. Bạn cần tư vấn về vấn đề gì ạ?",
                    QuickReplies = new List<string> { "Xem sản phẩm mới", "Tư vấn size", "Chính sách đổi trả" }
                },
                new ChatbotRule
                {
                    Keywords = new List<string> { "sản phẩm mới", "hàng mới", "new arrival" },
                    GetResponse = message => "Shop vừa về bộ sưu tập hè 2025 với rất nhiều mẫu áo thun và quần jeans thời trang. Bạn có thể xem chi tiết tại [link bộ sưu tập] nhé!",
                    QuickReplies = new List<string> { "Giá áo thun?", "Quần jeans có size gì?", "Chất liệu là gì?" }
                },
                new ChatbotRule
                {
                    Keywords = new List<string> { "size", "kích cỡ", "vừa không" },
                    GetResponse = message => "Để tư vấn size chính xác nhất, bạn vui lòng cho shop xin chiều cao và cân nặng ạ.",
                    QuickReplies = new List<string> { "1m70, 65kg", "Bảng size ở đâu?" }
                },
                new ChatbotRule
                {
                    Keywords = new List<string> { "giá", "bao nhiêu tiền", "giá cả" },
                    GetResponse = message => {
                        if (message.Contains("áo thun")) return "Áo thun đồng giá 250.000đ bạn nhé. Chất liệu cotton 100% thoáng mát ạ.";
                        if (message.Contains("quần jean")) return "Quần jeans có giá từ 450.000đ đến 600.000đ tùy mẫu ạ.";
                        return "Bạn muốn hỏi giá của sản phẩm nào ạ? Ví dụ: 'giá áo thun' hoặc 'giá quần jeans'.";
                    },
                    QuickReplies = new List<string> { "Giá áo thun?", "Giá quần jeans?" }
                },
                new ChatbotRule
                {
                    Keywords = new List<string> { "đổi trả", "bảo hành", "hoàn hàng" },
                    GetResponse = message => "Shop hỗ trợ đổi trả trong vòng 7 ngày nếu sản phẩm còn nguyên tem mác và chưa qua sử dụng ạ. Bạn chỉ cần mang hóa đơn đến cửa hàng gần nhất nhé.",
                    QuickReplies = new List<string> { "Địa chỉ cửa hàng?", "Cảm ơn shop" }
                },
                new ChatbotRule
                {
                    Keywords = new List<string> { "cảm ơn", "thank you", "ok shop", "ty", "thanks" },
                    GetResponse = message => "Không có gì ạ! Rất vui được hỗ trợ bạn. Nếu cần gì thêm, đừng ngần ngại hỏi nhé!",
                },
                new ChatbotRule
                {
                    Keywords = new List<string> { "tạm biệt", "bye" },
                    GetResponse = message => "Tạm biệt bạn, chúc bạn một ngày vui vẻ!",
                }
            };
        }

        public ChatResponse ProcessMessage(string userMessage)
        {
            var message = userMessage.ToLower().Trim();

            foreach (var rule in _rules)
            {
                // Kiểm tra xem tin nhắn có chứa bất kỳ từ khóa nào trong quy tắc không
                if (rule.Keywords.Any(keyword => message.Contains(keyword)))
                {
                    return new ChatResponse
                    {
                        Response = rule.GetResponse(message),
                        QuickReplies = rule.QuickReplies
                    };
                }
            }

            // Câu trả lời mặc định nếu không có quy tắc nào khớp
            return new ChatResponse
            {
                Response = "Xin lỗi, shop chưa hiểu ý bạn. Bạn có thể hỏi về: sản phẩm mới, tư vấn size, chính sách đổi trả...",
                QuickReplies = new List<string> { "Xem sản phẩm mới", "Tư vấn size", "Chính sách đổi trả" }
            };
        }
    }
}

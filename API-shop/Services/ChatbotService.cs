using System.Text;
using System.Text.RegularExpressions;
using System.Globalization; // Quan trọng: Để xử lý tiếng Việt
using API_shop.Models;

namespace API_shop.Services
{
    public class ChatbotRule
    {
        public List<string> Keywords { get; set; }
        public int Priority { get; set; } = 100; 
        public Func<string, string> GetResponse { get; set; }
        public List<string> QuickReplies { get; set; } = new List<string>();
    }

    public class ChatbotService
    {
        private readonly List<ChatbotRule> _rules;
        private const string ShopName = "BPFashion";

        public ChatbotService()
        {
            _rules = new List<ChatbotRule>
            {
                // 1. Chào hỏi
                new ChatbotRule
                {
                    Priority = 1,
                    Keywords = new List<string> { "xin chao", "hello", "hi", "chao shop", "hi shop" },
                    GetResponse = _ => $"Chào bạn! {ShopName} rất vui được đón tiếp. Bạn cần mình hỗ trợ gì không ạ?",
                    QuickReplies = new List<string> { "Sản phẩm mới", "Sale off", "Tư vấn size" }
                },

                // 2. Sản phẩm mới
                new ChatbotRule
                {
                    Keywords = new List<string> { "san pham moi", "hang moi", "new arrival", "bo suu tap" },
                    GetResponse = _ => $"{ShopName} vừa cập bến BST Hè 2025 cực cháy 🔥. Bạn có muốn xem qua Áo phông hay Quần Jeans không ạ?",
                    QuickReplies = new List<string> { "Xem áo phông", "Xem quần jeans", "Xem phụ kiện" }
                },

                // 3. Tư vấn Size
                new ChatbotRule
                {
                    Keywords = new List<string> { "size", "kich co", "mac vua", "bang size" },
                    GetResponse = _ => "Để chọn size chuẩn nhất tại BPFashion, bạn cho mình xin thông tin Chiều cao (cm) và Cân nặng (kg) nhé. \nVí dụ: 1m70 60kg.",
                    QuickReplies = new List<string> { "1m65, 55kg", "1m70, 65kg", "1m75, 75kg" }
                },

                // 4. Hỏi giá
                new ChatbotRule
                {
                    Keywords = new List<string> { "gia", "bao nhieu tien", "tien", "cost" },
                    GetResponse = message =>
                    {
                        if (message.Contains("ao")) return "Các mẫu Áo bên mình dao động từ 199k - 350k tùy mẫu ạ.";
                        if (message.Contains("quan")) return "Quần Jeans/Kaki bên mình có giá từ 350k - 600k ạ.";
                        if (message.Contains("phu kien") || message.Contains("that lung")) return "Phụ kiện (Thắt lưng, ví...) có giá từ 150k nhé bạn.";
                        return "Dạ bạn đang quan tâm giá của sản phẩm nào ạ? (Áo, Quần hay Phụ kiện)";
                    },
                    QuickReplies = new List<string> { "Giá áo thun", "Giá quần jeans", "Giá phụ kiện" }
                },

                // 5. Chính sách đổi trả & Ship
                new ChatbotRule
                {
                    Keywords = new List<string> { "doi tra", "bao hanh", "hoan hang", "ship", "van chuyen", "phi ship" },
                    GetResponse = message => 
                    {
                        if(message.Contains("ship") || message.Contains("van chuyen"))
                            return $"{ShopName} freeship cho đơn từ 500k. Phí ship nội thành là 20k, ngoại thành 30k ạ.";
                        
                        return $"{ShopName} hỗ trợ đổi size/mẫu trong vòng 7 ngày (giữ nguyên tem mác). Bảo hành đường may trọn đời bạn nhé!";
                    },
                    QuickReplies = new List<string> { "Phí ship bao nhiêu?", "Địa chỉ shop?" }
                },

                // 6. Địa chỉ & Liên hệ
                new ChatbotRule
                {
                    Keywords = new List<string> { "dia chi", "o dau", "cua hang", "lien he", "so dien thoai" },
                    GetResponse = _ => $"{ShopName} có cửa hàng tại: 123 Đường Thời Trang, Quận 1, TP.HCM.\nHotline: 1900 1234 (8h-22h).",
                    QuickReplies = new List<string> { "Xem bản đồ", "Gọi hotline" }
                },

                // 7. Tạm biệt
                new ChatbotRule
                {
                    Keywords = new List<string> { "tam biet", "bye", "cam on", "thanks", "ok shop" },
                    GetResponse = _ => $"Cảm ơn bạn đã ghé thăm {ShopName}. Chúc bạn một ngày tràn đầy năng lượng! ❤️",
                }
            };
        }

        public ChatResponse ProcessMessage(string userMessage)
        {
            if (string.IsNullOrWhiteSpace(userMessage)) 
                return new ChatResponse { Response = "Bạn ơi, bạn nhắn gì đi ạ?", QuickReplies = new List<string>() };

            var rawMessage = userMessage.ToLower().Trim();
            var normalizedMessage = RemoveVietnameseAccents(rawMessage);
            var sortedRules = _rules.OrderBy(r => r.Priority).ToList();

            foreach (var rule in sortedRules)
            {
                // Kiểm tra keyword trong chuỗi đã bỏ dấu
                if (rule.Keywords.Any(keyword => normalizedMessage.Contains(keyword)))
                {
                    return new ChatResponse
                    {
                        Response = rule.GetResponse(normalizedMessage),
                        QuickReplies = rule.QuickReplies
                    };
                }
            }

            // Fallback
            return new ChatResponse
            {
                Response = $"Xin lỗi, {ShopName} chưa hiểu ý bạn lắm. Bạn có thể hỏi về:",
                QuickReplies = new List<string> { "Sản phẩm mới", "Bảng size", "Địa chỉ shop", "Phí ship" }
            };
        }

        private string RemoveVietnameseAccents(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC).Replace("đ", "d");
        }
    }
}
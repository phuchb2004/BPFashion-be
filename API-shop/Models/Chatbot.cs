namespace API_shop.Models
{
    public class ChatMessage
    {
        public string Text { get; set; }
    }

    public class ChatResponse
    {
        public string Response { get; set; }
        public List<string> QuickReplies { get; set; } = new List<string>();
        public DateTime Timestamp { get; set; }
    }
}

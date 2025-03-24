using Microsoft.Extensions.AI;

namespace LlmPractice.Models
{
    public class ChatPrompt
    {
        public string Message { get; set; }
        public string Model { get; set; }
        public List<ChatMessage> ConversationHistory { get; set; } = [];
    }
}

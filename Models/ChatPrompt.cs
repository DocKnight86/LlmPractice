using Microsoft.Extensions.AI;

namespace LlmPractice.Models
{
    public class ChatPrompt
    {
        public required string Message { get; init; }
        public required string Model { get; init; }

        public List<ChatMessage> ConversationHistory { get; init; } = [];
    }
}

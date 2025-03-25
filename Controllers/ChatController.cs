using LlmPractice.Factories;
using LlmPractice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using System.Text.Json;

namespace LlmPractice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private const int ApproxTokensPerMessage = 50; // Rough estimate: average number of tokens per chat message. What should it be?

        private readonly IChatClientFactory _chatClientFactory;
        private readonly ILogger<ChatController> _logger;
        private readonly IConfiguration _configuration;

        public ChatController(IChatClientFactory chatClientFactory, ILogger<ChatController> logger, IConfiguration configuration)
        {
            _chatClientFactory = chatClientFactory;
            _logger = logger;
            _configuration = configuration;
        }
        
        [HttpPost("chathistory")]
        public async Task<IActionResult> Chat(ChatPrompt chatPrompt)
        {
            List<ChatMessage> messages = GroundPrompt(chatPrompt);

            // Use the model from the prompt or fallback to the configured default.
            string? selectedModel = string.IsNullOrWhiteSpace(chatPrompt.Model)
                ? _configuration["AI:Ollama:ChatModel"]
                : chatPrompt.Model;
            
            try
            {
                // write the new user message to the ChatHistory file.
                await AppendChatMessageAsync(new ChatMessage(ChatRole.User, chatPrompt.Message));

                IChatClient chatClient = _chatClientFactory.Create(selectedModel ?? string.Empty);
                ChatResponse response = await chatClient.GetResponseAsync(messages);

                // Extract the reply text and add it to the messages
                string replyText = response.Text;
        
                // Create a new ChatMessage using the extracted text.
                ChatMessage assistantMessage = new ChatMessage(ChatRole.Assistant, replyText);
                messages.Add(assistantMessage);

                // write the bot message to the ChatHistory file.
                await AppendChatMessageAsync(assistantMessage);

                return Ok(messages.Select(m => new
                {
                    Role = m.Role.ToString(),
                    Message = m.Text // m.Text concatenates the message's contents.
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat prompt");

                return StatusCode(500, "Internal server error");
            }
        }

        // function to append chat messages to a local NDJSON file
        private async Task AppendChatMessageAsync(ChatMessage message)
        {
            // Define the folder for chat history.
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ChatHistory");

            // If the folder not exist, create it.
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Define the file path.
            string filePath = Path.Combine(folderPath, "chat_history.ndjson");

            // Serialize the message to JSON.
            string json = JsonSerializer.Serialize(message);

            await System.IO.File.AppendAllTextAsync(filePath, json + Environment.NewLine);
        }

        private List<ChatMessage> GroundPrompt(ChatPrompt chatPrompt)
        {
            List<ChatMessage> messages = new List<ChatMessage>();

            // Add a system prompt to help the model focus on the latest query.
            // How can this be optimized in the future? Is there a better way I can be doing this?
            messages.Add(new ChatMessage(ChatRole.System, "Focus on the most recent question and ignore older context if it's not relevant."));

            // Trim the conversation history to prevent token overrun.
            List<ChatMessage> trimmedHistory = TrimConversationHistory(chatPrompt.ConversationHistory, maxTokenCount: 2048);
            if (trimmedHistory.Any())
            {
                messages.AddRange(trimmedHistory);
            }

            // Only append the latest user message if it's not already the last message in history.
            if (!trimmedHistory.Any() ||
                trimmedHistory.Last().Role != ChatRole.User ||
                trimmedHistory.Last().Text != chatPrompt.Message)
            {
                messages.Add(new ChatMessage(ChatRole.User, chatPrompt.Message));
            }
    
            // Log the estimated total token count for the full prompt.
            int totalPromptTokens = messages.Count * ApproxTokensPerMessage;

            // Debug for now to view tokens and limits in the console.
            Console.WriteLine($"[GroundPrompt] Total tokens for built prompt: {totalPromptTokens} (approximation, {messages.Count} messages * {ApproxTokensPerMessage} tokens each)");

            return messages;
        }

        private List<ChatMessage> TrimConversationHistory(List<ChatMessage> history, int maxTokenCount)
        {
            int totalTokens = history.Count * ApproxTokensPerMessage;
            int allowedMessages = maxTokenCount / ApproxTokensPerMessage;
    
            // Debug for now to view tokens and limits in the console.
            Console.WriteLine($"[TrimConversationHistory] Total tokens in history: {totalTokens}");
            Console.WriteLine($"[TrimConversationHistory] Allowed messages (max tokens {maxTokenCount}): {allowedMessages}");
    
            if (history.Count > allowedMessages)
            {
                List<ChatMessage> trimmed = history.Skip(history.Count - allowedMessages).ToList();
                int trimmedTokens = trimmed.Count * ApproxTokensPerMessage;

                // Debug for now to view tokens and limits in the console.
                Console.WriteLine($"[TrimConversationHistory] Total tokens after trimming: {trimmedTokens}");

                return trimmed;
            }
    
            // Debug for now to view tokens and limits in the console.
            Console.WriteLine($"[TrimConversationHistory] No trimming required. Total tokens: {totalTokens}");

            return history;
        }
    }
}

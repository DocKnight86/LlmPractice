using LlmPractice.Factories;
using LlmPractice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;

namespace LlmPractice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatClientFactory _chatClientFactory;
        private readonly ILogger<ChatController> _logger;
        private readonly IConfiguration _configuration;

        public ChatController(IChatClientFactory chatClientFactory, ILogger<ChatController> logger, IConfiguration configuration)
        {
            _chatClientFactory = chatClientFactory;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Chat(ChatPrompt chatPrompt)
        {
            List<ChatMessage> messages = GroundPrompt(chatPrompt);

            // Use the model from the prompt or fall back to the configured default.
            string selectedModel = string.IsNullOrWhiteSpace(chatPrompt.Model)
                ? _configuration["AI:Ollama:ChatModel"]
                : chatPrompt.Model;

            try
            {
                IChatClient chatClient = _chatClientFactory.Create(selectedModel);
                ChatResponse response = await chatClient.GetResponseAsync(messages);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat prompt");

                return StatusCode(500, "Internal server error");
            }
        }
        
        [HttpPost("chathistory")]
        public async Task<IActionResult> ChatHistory(ChatPrompt chatPrompt)
        {
            List<ChatMessage> messages = GroundPrompt(chatPrompt);

            // Use the model from the prompt or fallback to the configured default.
            string selectedModel = string.IsNullOrWhiteSpace(chatPrompt.Model)
                ? _configuration["AI:Ollama:ChatModel"]
                : chatPrompt.Model;
            
            try
            {
                IChatClient chatClient = _chatClientFactory.Create(selectedModel);
                ChatResponse response = await chatClient.GetResponseAsync(messages);

                // Extract the reply text and add it to the messages
                string replyText = response.Text;
        
                // Create a new ChatMessage using the extracted text.
                messages.Add(new ChatMessage(ChatRole.Assistant, replyText));

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

        private List<ChatMessage> GroundPrompt(ChatPrompt chatPrompt)
        {
            return new List<ChatMessage>
            {
                new ChatMessage(ChatRole.User, chatPrompt.Message)
            };
        }
    }
}

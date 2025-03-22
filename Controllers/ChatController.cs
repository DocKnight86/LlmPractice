using LlmPractice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;

namespace LlmPractice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatClient _chatClient;
        private readonly ILogger<ChatController> _logger;
        private readonly IConfiguration _configuration;

        public ChatController(IChatClient chatClient, ILogger<ChatController> logger, IConfiguration configuration)
        {
            _chatClient = chatClient;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Chat(ChatPrompt chatPrompt)
        {
            List<ChatMessage> messages = GroundPrompt(chatPrompt);
            try
            {
                ChatResponse response = await _chatClient.GetResponseAsync(messages);
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
            try
            {
                ChatResponse response = await _chatClient.GetResponseAsync(messages);
        
                // Extract the reply text from the ChatResponse.
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
            string? systemMessage = _configuration["ChatSettings:SystemMessage"];
            string? assistantMessage = _configuration["ChatSettings:AssistantMessage"];

            return new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, systemMessage ?? "Default system message."),
                new ChatMessage(ChatRole.Assistant, assistantMessage ?? "Default assistant message."),
                new ChatMessage(ChatRole.User, chatPrompt.Message)
            };
        }
    }
}

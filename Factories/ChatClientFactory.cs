using Microsoft.Extensions.AI;

namespace LlmPractice.Factories
{
    public interface IChatClientFactory
    {
        IChatClient Create(string model);
    }

    public class ChatClientFactory : IChatClientFactory
    {
        private readonly IConfiguration _configuration;

        public ChatClientFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IChatClient Create(string model)
        {
            string? endpoint = _configuration["AI:Ollama:Endpoint"];
            return new OllamaChatClient(endpoint, model);
        }
    }
}

using Microsoft.Extensions.AI;

namespace LlmPractice.Factories
{
    public interface IChatClientFactory
    {
        IChatClient Create(string model);
    }

    public class ChatClientFactory(IConfiguration configuration) : IChatClientFactory
    {
        public IChatClient Create(string model)
        {
            string? endpoint = configuration["AI:Ollama:Endpoint"];

            return new OllamaChatClient(endpoint ?? string.Empty, model);
        }
    }
}

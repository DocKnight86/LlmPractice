using Microsoft.Extensions.AI;

namespace LlmPractice
{
    public static class Extensions
    {
        // This method is called on an instance of IHostApplicationBuilder and adds AI-related services.
        public static void AddApplicationServices(this IHostApplicationBuilder builder)
        {
            builder.AddAIServices();
        }

        /*
        This private method retrieves necessary configurations, sets up a chat client, and configures various 
        services like function invocation, OpenTelemetry, and logging.
        */
        private static void AddAIServices(this IHostApplicationBuilder builder)
        {
            ILoggerFactory? loggerFactory = builder.Services.BuildServiceProvider().GetService<ILoggerFactory>();

            string? ollamaEndpoint = builder.Configuration["AI:Ollama:Endpoint"];

            if (!string.IsNullOrWhiteSpace(ollamaEndpoint))
            {
                builder.Services.AddChatClient(new OllamaChatClient(ollamaEndpoint, builder.Configuration["AI:Ollama:ChatModel"] ?? "llama3.1"))
                    .UseFunctionInvocation()
                    .UseOpenTelemetry(configure: t => t.EnableSensitiveData = true)
                    .UseLogging(loggerFactory)
                    .Build();
            }
        }
    }
}

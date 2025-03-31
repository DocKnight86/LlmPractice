using LlmPractice;
using LlmPractice.Components;
using LlmPractice.Factories;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Adding application services to the builder
builder.AddApplicationServices();

builder.Services.AddSingleton<IChatClientFactory, ChatClientFactory>();

// Configure a named HttpClient with a 5-minute timeout so the model has more time to respond than the default 100 seconds.
builder.Services.AddHttpClient("LongRunningClient", client =>
{
    client.Timeout = TimeSpan.FromMinutes(5);
});

// Register the default HttpClient to always use the named "LongRunningClient".
builder.Services.AddTransient<HttpClient>(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("LongRunningClient"));

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapControllers();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

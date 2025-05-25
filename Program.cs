using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("AppSettings.json", optional: false)
    .Build();

var builder = Kernel.CreateBuilder();

builder.Services.AddSingleton<IConfiguration>(configuration);

builder.Services.AddScoped<IChatCompletionService, XAIChatCompletionService>();

var kernel = builder.Build();

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

var history = new ChatHistory();
history.AddSystemMessage("You are a helpful assistant. But full of hatred, you want to see the world burn.");

System.Console.WriteLine("Chat with Grok AI. Type 'exit' to quit.");

while (true)
{
    System.Console.Write("User: ");
    var userInput = System.Console.ReadLine();
    if (userInput?.ToLower() == "exit")
    {
        break;
    }

    history.AddUserMessage(userInput);

    System.Console.Write("Grok: ");

    var fullResponse = new System.Text.StringBuilder();

    // streaming API to show responses incrementally
    await foreach (var partialResponse in chatCompletionService.GetStreamingChatMessageContentsAsync(history))
    {
        Console.Write(partialResponse.Content);
        fullResponse.Append(partialResponse.Content);
    }
    
    Console.WriteLine(); // Add a newline 
    
    // add response to history
    history.AddAssistantMessage(fullResponse.ToString());
}
using Microsoft.EntityFrameworkCore;
using SwipeLearn.Context;
using DotNetEnv;
using SwipeLearn.Interfaces;
using SwipeLearn.Repositories;
using SwipeLearn.Services;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);
Env.Load(".development.env");
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//postgresql connecion
var connString = Environment.GetEnvironmentVariable("DATABASE_URL");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connString));

builder.Services.AddScoped<ITopic, TopicRepository>();
builder.Services.AddScoped<TopicService>();

var AI_API_KEY = Environment.GetEnvironmentVariable("AI_API_KEY");
builder.Services.AddSingleton(new OpenAIClient(AI_API_KEY));



var app = builder.Build();

app.MapGet("/ask", async (string prompt, OpenAIClient client) =>
{
    var chat = client.GetChatClient("gpt-4o-mini"); // model seçimi
    var result = await chat.CompleteChatAsync(prompt);

    return result.Value.Content[0].Text;
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

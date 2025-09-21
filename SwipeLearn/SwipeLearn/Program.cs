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
builder.Services.AddScoped<ITopicMaterial, TopicMaterialRepository>();
builder.Services.AddScoped<IVideo, VideoRepository>();
builder.Services.AddScoped<IQuestion, QuestionRepository>();
builder.Services.AddScoped<MainService>();

var AI_API_KEY = Environment.GetEnvironmentVariable("CHATGPT_API_KEY");
builder.Services.AddSingleton(new OpenAIClient(AI_API_KEY));

builder.Services.AddHttpClient();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // frontend
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

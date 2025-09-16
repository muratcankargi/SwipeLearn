using Microsoft.EntityFrameworkCore;
using SwipeLearn.Context;
using System;
using System.Data.Common;
using DotNetEnv;
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

var app = builder.Build();

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

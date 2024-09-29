using System.ComponentModel;
using System.Text.Json;
using TestDockerNet8;
using TestDockerNet8.Interfaces;
using TestDockerNet8.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://*:8888");

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddSingleton<ISpecializeService, SpecializeService>();
builder.Services.AddTransient<IFunctionService, FunctionService>();
builder.Services.AddSingleton<IFunctionStoreService, FunctionStoreService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting(); // Add the routing middleware

app.Run();

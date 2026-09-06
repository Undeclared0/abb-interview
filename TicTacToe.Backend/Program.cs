using Microsoft.EntityFrameworkCore;
using TicTacToe.Backend.Data;
using TicTacToe.Backend.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseInMemoryDatabase("TicTacToeDb"));

builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IScoreboardService, ScoreboardService>();

// Enable CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("TicTacToeFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173").AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("TicTacToeFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();

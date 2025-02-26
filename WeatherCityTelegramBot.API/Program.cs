using Microsoft.Data.SqlClient;
using System.Data;
using WeatherCityTelegramBot.API.HostBuilders.TelegramBot;
using WeatherCityTelegramBot.BLL.EnvironmentVariables;
using WeatherCityTelegramBot.DAL.Persistence;
using WeatherCityTelegramBot.DAL.Persistence.Contracts;
using WeatherCityTelegramBot.DAL.Repositories;
using WeatherCityTelegramBot.DAL.Repositories.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging();

// Connection/Transaction DAPPER database
builder.Services.AddScoped((s) => new SqlConnection(builder.Configuration.GetConnectionString("MSSQLConnection")));
builder.Services.AddScoped<IDbTransaction>(s =>
{
    SqlConnection conn = s.GetRequiredService<SqlConnection>();
    conn.Open();
    return conn.BeginTransaction();
});

// Environment Variables
builder.Services.Configure<WeatherApiEnvironment>(builder.Configuration.GetSection("WeatherAPI"));

// Repositories
builder.Services.AddScoped<IUserRepository,UserRepository>();
builder.Services.AddScoped<IWeatherHistoryRepository,WeatherHistoryRepository>();

// Other
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Telegram Bot
builder.Services.AddSingleton<TelegramBotHost>();

var app = builder.Build();

var botHost = app.Services.GetRequiredService<TelegramBotHost>();
botHost.Start();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

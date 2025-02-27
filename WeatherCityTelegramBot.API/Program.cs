using Microsoft.Data.SqlClient;
using System.Data;
using WeatherCityTelegramBot.API.HostBuilders.TelegramBot;
using WeatherCityTelegramBot.BLL.EnvironmentVariables;
using WeatherCityTelegramBot.BLL.Services;
using WeatherCityTelegramBot.BLL.Services.Contracts;
using WeatherCityTelegramBot.DAL.Persistence;
using WeatherCityTelegramBot.DAL.Persistence.Contracts;
using WeatherCityTelegramBot.DAL.Repositories;
using WeatherCityTelegramBot.DAL.Repositories.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging();
builder.Services.AddHttpClient();

// Connection/Transaction DAPPER database
var conn = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddScoped<SqlConnection>(s =>
{
    var connection = new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"));
    connection.Open();
    return connection;
});

builder.Services.AddScoped<IDbTransaction>(s =>
{
    var connection = s.GetRequiredService<SqlConnection>();
    return connection.BeginTransaction();
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

// HttpClients
builder.Services.AddHttpClient<IWeatherApiService, WeatherApiService>();

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

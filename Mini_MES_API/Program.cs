using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Mini_MES_API.Data;
using Mini_MES_API.Api;
using Mini_MES_API.Handlers;
using Mini_MES_API.Services;
using Mini_MES_API.Stores;
using MQTTnet;
using MQTTnet.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<MachineStateStore>();

builder.Services.AddSingleton<FactorySnapshotStore>();
builder.Services.AddHostedService<FactoryMqttListener>();

builder.Services.AddSingleton<IMqttClient>(provider =>
{
    var factory = new MqttFactory();
    return factory.CreateMqttClient();
});

builder.Services.AddDbContextFactory<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LocalDB")));

builder.Services.AddScoped<ProductionOrderHandlers>();

builder.Services.AddHostedService<StartupService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
    );
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapProductionOrderEndPoints();
app.MapMachineStateEndpoints();

app.Run();

public partial class Program { }

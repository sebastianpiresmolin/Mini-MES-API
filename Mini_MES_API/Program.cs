using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Mini_MES_API.Data;
using Mini_MES_API.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LocalDB"))
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapProductionOrderEndPoints();

app.Run();

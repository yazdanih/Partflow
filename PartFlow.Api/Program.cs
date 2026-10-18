using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using PartFlow.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PartFlowDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("PartFlowDb")));
builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

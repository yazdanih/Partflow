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

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<PartFlowDbContext>();

    await dbContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

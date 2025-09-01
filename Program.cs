using System.Text.Json.Serialization;
using DotNetEnv;
using F360.JobsProcessor.API.Infrastructure;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Env.TraversePath().Load();

builder.Services.AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddMongoDbConfiguration();
builder.Services.AddMassTransitConfiguration();
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddOpenTelemetryConfiguration();
builder.Services.AddOpenApi();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
using DotNetEnv;
using F360.JobsProcessor.API.Infrastructure;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Env.TraversePath().Load();

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddMongoDbConfiguration();
builder.Services.AddMassTransitConfiguration();
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddOpenTelemetryConfiguration();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
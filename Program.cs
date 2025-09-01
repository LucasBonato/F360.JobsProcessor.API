using DotNetEnv;
using F360.JobsProcessor.API;
using F360.JobsProcessor.API.Infrastructure;
using MassTransit;
using MongoDB.Driver;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Env.TraversePath().Load();

builder.Services.AddOpenApi();

builder.Services.AddMongoDb();

builder.Services.AddMassTransit(configuration => {
    configuration.AddMongoDbOutbox(config => {
        config.QueryDelay = TimeSpan.FromSeconds(1);

        config.ClientFactory(provider => provider.GetRequiredService<IMongoClient>());
        config.DatabaseFactory(provider => provider.GetRequiredService<IMongoDatabase>());

        config.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);

        config.UseBusOutbox();
    });

    configuration.UsingRabbitMq((context, configurator) => {
        configurator.Host(new Uri(AppEnv.MESSAGE_BROKER.HOST.NotNull()), host => {
            host.Username(AppEnv.MESSAGE_BROKER.USERNAME.NotNull());
            host.Password(AppEnv.MESSAGE_BROKER.PASSWORD.NotNull());
        });

        configurator.ConfigureEndpoints(context);
    });
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.Run();
using System.Text.Json;
using System.Text.Json.Serialization;
using F360.JobsProcessor.API.Application;
using F360.JobsProcessor.API.Domain.Contracts.UseCases;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace F360.JobsProcessor.API.Infrastructure;

public static class DependencyInjectionExtensions {

	public static IServiceCollection AddServices(this IServiceCollection services) {
		return services
				.AddScoped<ICreateJobUseCase, CreateJobUseCase>()
				.AddScoped<IGetJobUseCase, GetJobUseCase>()
				.Configure<JsonOptions>(options => {
					options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
					options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
					options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
					options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper));
				})
			;
	}

	public static IServiceCollection AddMongoDbConfiguration(this IServiceCollection services) {
		return services
			.AddSingleton<IMongoClient>(_ => new MongoClient(AppEnv.DATABASE.MONGO.CONNECTION_URI.NotNull()))
			.AddSingleton<IMongoDatabase>(provider => provider.GetRequiredService<IMongoClient>().GetDatabase(AppEnv.DATABASE.MONGO.DATABASE_NAME.NotNull()))
			;
	}

	public static IServiceCollection AddMassTransitConfiguration(this IServiceCollection services) {
		return services.AddMassTransit(configuration => {
			// configuration.AddMongoDbOutbox(config => {
			// 	config.QueryDelay = TimeSpan.FromSeconds(1);
			//
			// 	config.ClientFactory(provider => provider.GetRequiredService<IMongoClient>());
			// 	config.DatabaseFactory(provider => provider.GetRequiredService<IMongoDatabase>());
			//
			// 	config.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
			//
			// 	config.UseBusOutbox();
			// });

			configuration.SetKebabCaseEndpointNameFormatter();

			configuration.AddConsumer<JobSubmittedConsumer>();

			configuration.UsingRabbitMq((context, configurator) => {
				configurator.Host(new Uri(AppEnv.MESSAGE_BROKER.HOST.NotNull()), host => {
					host.Username(AppEnv.MESSAGE_BROKER.USERNAME.NotNull());
					host.Password(AppEnv.MESSAGE_BROKER.PASSWORD.NotNull());
				});

				configurator.ConfigureEndpoints(context);
			});

		});
	}
}
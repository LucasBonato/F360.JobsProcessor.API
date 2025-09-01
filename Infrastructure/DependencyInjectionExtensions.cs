using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using F360.JobsProcessor.API.Application;
using F360.JobsProcessor.API.Domain.Contracts;
using F360.JobsProcessor.API.Domain.Contracts.UseCases;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace F360.JobsProcessor.API.Infrastructure;

public static class DependencyInjectionExtensions {

	public static IServiceCollection AddRepositories(this IServiceCollection services) {
		return services
				.AddSingleton<IJobRepository, JobMongoRepository>()
			;
	}

	public static IServiceCollection AddServices(this IServiceCollection services) {
		return services
				.AddScoped<ICreateJobUseCase, CreateJobUseCase>()
				.AddScoped<IGetJobUseCase, GetJobUseCase>()
				.Configure<JsonOptions>(options => {
					options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
					options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
					options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
					options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
				})
				.AddProblemDetails(options =>
					options.CustomizeProblemDetails = context => {
						HttpContext httpContext = context.HttpContext;
						string traceId = Activity.Current?.TraceId.ToString()?? httpContext.TraceIdentifier;
						string traceParent = Activity.Current?.Id ?? httpContext.TraceIdentifier;
						ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<Program>();
						if (context.Exception is not null)
							logger.LogError(context.Exception, "{httpContext}, {traceId}, {traceParent}", httpContext, traceId, traceParent);

						if (string.IsNullOrEmpty(context.ProblemDetails.Type)) {
							context.ProblemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
						}

						context.ProblemDetails.Instance = httpContext.Request.Path;
						context.ProblemDetails.Extensions.TryAdd("method", httpContext.Request.Method);

						if (context.ProblemDetails.Extensions.ContainsKey("traceId"))
							context.ProblemDetails.Extensions["traceId"] = traceId;
						else
							context.ProblemDetails.Extensions.TryAdd("traceId", traceId);

						httpContext.Response.StatusCode = context.ProblemDetails.Status?? (int)HttpStatusCode.InternalServerError;
						httpContext.Response.Headers.TryAdd("traceparent", traceParent);
					}
				)
			;
	}

	public static IServiceCollection AddOpenTelemetryConfiguration(this IServiceCollection services)
	{
		string serviceName = AppEnv.OTEL_SERVICE_NAME.NotNull();

		services
			.AddOpenTelemetry()
			.UseOtlpExporter()
			.ConfigureResource(resource => {
				resource.AddService(serviceName: serviceName);
			})
			.WithTracing(tracing => {
				tracing
					.AddSource(serviceName)
					.AddAspNetCoreInstrumentation()
					.AddHttpClientInstrumentation()
					;
			})
			;

		services.AddLogging(logger => {
			logger
				.AddOpenTelemetry(telemetryLoggerOptions => {
					telemetryLoggerOptions.IncludeScopes = true;
					telemetryLoggerOptions.ParseStateValues = true;
					telemetryLoggerOptions.IncludeFormattedMessage = true;
				})
				;
		});

		return services;
	}

	public static IServiceCollection AddMongoDbConfiguration(this IServiceCollection services) {
		return services
			.AddSingleton<IMongoClient>(_ => new MongoClient(AppEnv.DATABASE.MONGO.CONNECTION_URI.NotNull()))
			.AddSingleton<IMongoDatabase>(provider => provider.GetRequiredService<IMongoClient>().GetDatabase(AppEnv.DATABASE.MONGO.DATABASE_NAME.NotNull()))
			;
	}

	public static IServiceCollection AddMassTransitConfiguration(this IServiceCollection services) {
		return services.AddMassTransit(configuration => {
			configuration.SetKebabCaseEndpointNameFormatter();

			configuration.AddConsumer<JobSubmittedConsumer>();

			configuration.UsingRabbitMq((context, configurator) => {
				configurator.Host(new Uri(AppEnv.MESSAGE_BROKER.HOST.NotNull()), host => {
					host.Username(AppEnv.MESSAGE_BROKER.USERNAME.NotNull());
					host.Password(AppEnv.MESSAGE_BROKER.PASSWORD.NotNull());
				});

				configurator.UseMessageRetry(retry => retry.Interval(5, TimeSpan.FromSeconds(5)));

				configurator.ReceiveEndpoint("job-submitted", endpoint => {
					endpoint.ConfigureConsumer<JobSubmittedConsumer>(context);

					endpoint.PrefetchCount = 10;
					endpoint.ConcurrentMessageLimit = 5;
				});
			});

		});
	}
}
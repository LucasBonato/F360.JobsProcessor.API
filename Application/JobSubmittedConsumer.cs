using System.Text.Json;
using F360.JobsProcessor.API.Domain;
using F360.JobsProcessor.API.Infrastructure;
using MassTransit;
using MongoDB.Driver;

namespace F360.JobsProcessor.API.Application;

public class JobSubmittedConsumer(
	ILogger<JobSubmittedConsumer> logger,
	IMongoDatabase database
) : IConsumer<Job> {
	public async Task Consume(ConsumeContext<Job> context) {
		logger.LogInformation("Received job submitted event: {payload}", JsonSerializer.Serialize(context.Message));

		if (context.Message.MaxAttempts == context.Message.Attempts)
			return;

		IMongoCollection<Job> jobsCollection = database.JobsCollection();

		await Task.Delay(10000);

		FilterDefinition<Job>? filter = Builders<Job>.Filter.Eq(job => job.Id, context.Message.Id);

		UpdateDefinition<Job>? update = Builders<Job>.Update
			.Set(job => job.Status, JobStatus.Processing)
			.Set(job => job.StartedAt, DateTime.UtcNow)
			.Set(job => job.Attempts, context.Message.Attempts + 1);

		await jobsCollection.UpdateOneAsync(filter, update);
		logger.LogInformation("Job updated: {status}", JobStatus.Processing);

		await Task.Delay(5000);

		UpdateDefinition<Job>? updateToFinish = Builders<Job>.Update
			.Set(job => job.Status, JobStatus.Completed)
			.Set(job => job.FinishedAt, DateTime.UtcNow);

		await jobsCollection.UpdateOneAsync(filter, updateToFinish);
		logger.LogInformation("Job updated: {status}", JobStatus.Completed);

		await ValueTask.CompletedTask;
	}
}
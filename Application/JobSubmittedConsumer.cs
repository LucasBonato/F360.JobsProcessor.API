using System.Text.Json;
using F360.JobsProcessor.API.Domain;
using F360.JobsProcessor.API.Domain.Contracts;
using MassTransit;

namespace F360.JobsProcessor.API.Application;

public class JobSubmittedConsumer(
	ILogger<JobSubmittedConsumer> logger,
	IJobRepository jobRepository
) : IConsumer<Job> {
	public async Task Consume(ConsumeContext<Job> context) {
		logger.LogInformation("Received job submitted event: {payload}", JsonSerializer.Serialize(context.Message));

		if (context.Message.MaxAttempts == context.Message.Attempts)
			return;

		Job? job = await jobRepository.GetOneByIdAsync(context.Message.Id, context.CancellationToken);

		if (job?.IsProcessingOrCompleted()?? true)
			return;

		await Task.Delay(TimeSpan.FromSeconds(5), context.CancellationToken);

		try {
			context.Message.SetStatus(JobStatus.Processing);
			context.Message.StartedAt = DateTime.UtcNow;
			context.Message.IncreaseAttempt();

			await jobRepository.UpdateOneAsync(context.Message, context.CancellationToken);

			logger.LogInformation("Job {jobId} updated: {status}", context.Message.Id, JobStatus.Processing);

			Random random = new();

			await Task.Delay(TimeSpan.FromSeconds(10), context.CancellationToken);

			if (random.Next(0, 100) == 7)
				throw new Exception("Random exception to simulate an failed job");

			context.Message.SetStatus(JobStatus.Completed);
			context.Message.FinishedAt = DateTime.UtcNow;

			await jobRepository.UpdateOneAsync(context.Message, context.CancellationToken);

			logger.LogInformation("Job {jobId} updated: {status}", context.Message.Id, JobStatus.Completed);
		}
		catch(Exception e) {
			logger.LogError(e, "Job {jobId} error: {message}", context.Message.Id, e.Message);
			context.Message.SetStatus(JobStatus.Failed);
			context.Message.LastError = e.Message;
			await jobRepository.UpdateOneAsync(context.Message, context.CancellationToken);
			throw;
		}

		await ValueTask.CompletedTask;
	}
}
using F360.JobsProcessor.API.Domain;
using F360.JobsProcessor.API.Domain.Contracts;
using F360.JobsProcessor.API.Domain.Contracts.UseCases;
using F360.JobsProcessor.API.Domain.DTOs.Request;
using MassTransit;

namespace F360.JobsProcessor.API.Application;

public class CreateJobUseCase(
	IJobRepository jobRepository,
	IPublishEndpoint publishEndpoint
) : ICreateJobUseCase {
	public async Task<Job> ExecuteAsync(JobRequest request, CancellationToken cancellationToken) {
		Job job = new(request);

		await jobRepository.CreateOneAsync(job, cancellationToken);

		Logger.Info($"Job created: {job.Id}");

		await publishEndpoint.Publish(job, cancellationToken);

		return job;
	}
}
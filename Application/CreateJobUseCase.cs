using F360.JobsProcessor.API.Domain;
using F360.JobsProcessor.API.Domain.Contracts.UseCases;
using F360.JobsProcessor.API.Domain.DTOs.Request;
using F360.JobsProcessor.API.Infrastructure;
using MassTransit;
using MongoDB.Driver;

namespace F360.JobsProcessor.API.Application;

public class CreateJobUseCase(
	IMongoDatabase database,
	IPublishEndpoint publishEndpoint
) : ICreateJobUseCase {
	public async Task<Job> ExecuteAsync(JobRequest request, CancellationToken cancellationToken) {
		IMongoCollection<Job> jobsCollection = database.JobsCollection();

		Job job = new(request);

		await jobsCollection.InsertOneAsync(
			document: job,
			cancellationToken: cancellationToken
		);

		await publishEndpoint.Publish(job, cancellationToken);

		return job;
	}
}
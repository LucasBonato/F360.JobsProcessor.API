using F360.JobsProcessor.API.Domain;
using F360.JobsProcessor.API.Domain.Contracts.UseCases;
using F360.JobsProcessor.API.Infrastructure;
using MongoDB.Driver;

namespace F360.JobsProcessor.API.Application;

public class GetJobUseCase(
	IMongoDatabase database
) : IGetJobUseCase {
	public async Task<Job> ExecuteAsync(string jobId, CancellationToken cancellationToken) {
		IMongoCollection<Job> jobsCollection = database.JobsCollection();

		FilterDefinition<Job>? filter = Builders<Job>.Filter.Eq(job => job.Id, jobId);

		Job? job = await jobsCollection
			.Find(filter)
			.FirstOrDefaultAsync(cancellationToken);

		if (job == null)
			throw new Exception("Job not found");

		return job;
	}
}
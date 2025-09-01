using F360.JobsProcessor.API.Domain;
using F360.JobsProcessor.API.Domain.Contracts;
using MongoDB.Driver;

namespace F360.JobsProcessor.API.Infrastructure;

public class JobMongoRepository(
	IMongoDatabase database
) : IJobRepository {
	public async Task<Job> CreateOneAsync(Job job, CancellationToken cancellationToken) {
		IMongoCollection<Job> jobsCollection = database.JobsCollection();

		await jobsCollection.InsertOneAsync(
			document: job,
			cancellationToken: cancellationToken
		);

		return job;
	}
	public async Task<Job> UpdateOneAsync(Job job, CancellationToken cancellationToken) {
		IMongoCollection<Job> jobsCollection = database.JobsCollection();

		FilterDefinition<Job>? filter = Builders<Job>.Filter.Eq(jobDocument => jobDocument.Id, job.Id);

		await jobsCollection.ReplaceOneAsync(filter, job, cancellationToken: cancellationToken);

		return job;
	}
	public async Task<Job?> GetOneByIdAsync(string jobId, CancellationToken cancellationToken) {
		IMongoCollection<Job> jobsCollection = database.JobsCollection();

		FilterDefinition<Job>? filter = Builders<Job>.Filter.Eq(job => job.Id, jobId);

		Job? job = await jobsCollection
			.Find(filter)
			.FirstOrDefaultAsync(cancellationToken);

		return job;
	}
}
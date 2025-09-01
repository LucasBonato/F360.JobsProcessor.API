namespace F360.JobsProcessor.API.Domain.Contracts;

public interface IJobRepository {
	Task<Job> CreateOneAsync(Job job, CancellationToken cancellationToken);
	Task<Job> UpdateOneAsync(Job job, CancellationToken cancellationToken);
	Task<Job?> GetOneByIdAsync(string jobId, CancellationToken cancellationToken);
}
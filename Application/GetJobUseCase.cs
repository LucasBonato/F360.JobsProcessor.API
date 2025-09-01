using F360.JobsProcessor.API.Domain;
using F360.JobsProcessor.API.Domain.Contracts;
using F360.JobsProcessor.API.Domain.Contracts.UseCases;

namespace F360.JobsProcessor.API.Application;

public class GetJobUseCase(
	IJobRepository jobRepository
) : IGetJobUseCase {
	public async Task<Job> ExecuteAsync(string jobId, CancellationToken cancellationToken) {
		Job? job = await jobRepository.GetOneByIdAsync(jobId, cancellationToken);

		if (job == null)
			throw new Exception("Job not found");

		return job;
	}
}
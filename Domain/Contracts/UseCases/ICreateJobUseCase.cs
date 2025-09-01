using F360.JobsProcessor.API.Domain.DTOs.Request;

namespace F360.JobsProcessor.API.Domain.Contracts.UseCases;

public interface ICreateJobUseCase {
	Task<Job> ExecuteAsync(JobRequest request, CancellationToken cancellationToken);
}
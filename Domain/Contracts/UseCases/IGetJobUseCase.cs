namespace F360.JobsProcessor.API.Domain.Contracts.UseCases;

public interface IGetJobUseCase {
	Task<Job> ExecuteAsync(string jobId, CancellationToken cancellationToken);
}
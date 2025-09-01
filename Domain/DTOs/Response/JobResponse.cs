namespace F360.JobsProcessor.API.Domain.DTOs.Response;

public record JobResponse(
	string Id,
	JobType Type,
	JobStatus Status
) {
	public JobResponse(Job job) : this(job.Id, job.Type, job.Status) {}
};
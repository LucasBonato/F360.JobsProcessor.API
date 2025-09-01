namespace F360.JobsProcessor.API.Domain.DTOs.Request;

public record JobRequest(
	JobType Type,
	string? Sender,
	string? To,
	string Content
);
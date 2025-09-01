namespace F360.JobsProcessor.API.Domain.DTOs.Request;

public record JobRequest(
	JobType Type,
	string? Sender,
	string? To,
	string? Subject = null,
	string? Content = null,
	string? ReportName = null,
	DateTime? ScheduledAt = null,
	Dictionary<string, object>? Metadata = null
);
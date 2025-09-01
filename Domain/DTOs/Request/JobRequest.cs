using System.Text.Json.Serialization;

namespace F360.JobsProcessor.API.Domain.DTOs.Request;

public record JobRequest(
	[property: JsonConverter(typeof(JsonStringEnumConverter))] JobType Type,
	string? Sender,
	string? To,
	string? Subject = null,
	string? Content = null,
	string? ReportName = null,
	DateTime? ScheduledAt = null,
	Dictionary<string, object>? Metadata = null
);
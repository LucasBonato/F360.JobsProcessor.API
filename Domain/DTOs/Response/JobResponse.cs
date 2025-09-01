using System.Text.Json.Serialization;

namespace F360.JobsProcessor.API.Domain.DTOs.Response;

public record JobResponse(
	string Id,
	[property: JsonConverter(typeof(JsonStringEnumConverter))] JobType Type,
	[property: JsonConverter(typeof(JsonStringEnumConverter))] JobStatus Status
) {
	public JobResponse(Job job) : this(job.Id, job.Type, job.Status) {}
};
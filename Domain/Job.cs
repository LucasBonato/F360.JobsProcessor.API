using System.Text.Json;
using F360.JobsProcessor.API.Domain.DTOs.Request;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace F360.JobsProcessor.API.Domain;

public class Job {
	[BsonId]
	[BsonRepresentation(BsonType.String)]
	public string Id { get; set; }
	public JobStatus Status { get; set; } = JobStatus.Pending;
	public JobType Type { get; set; }
	public string Payload { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public DateTime? StartedAt { get; set; }
	public DateTime? FinishedAt { get; set; }
	public int Attempts { get; set; } = 0;
	public int MaxAttempts { get; set; } = 5;

	public Job() {}

	public Job(JobRequest jobRequest) {
		Id = Guid.CreateVersion7().ToString();
		Type = jobRequest.Type;
		Payload = JsonSerializer.Serialize(jobRequest);
	}
}
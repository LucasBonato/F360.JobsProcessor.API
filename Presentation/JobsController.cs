using F360.JobsProcessor.API.Domain;
using F360.JobsProcessor.API.Domain.Contracts;
using F360.JobsProcessor.API.Domain.Contracts.UseCases;
using F360.JobsProcessor.API.Domain.DTOs.Request;
using F360.JobsProcessor.API.Domain.DTOs.Response;
using Microsoft.AspNetCore.Mvc;

namespace F360.JobsProcessor.API.Presentation;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase {
	[HttpPost]
	public async Task<ActionResult<JobResponse>> CreateNewJob(
		[FromServices] ICreateJobUseCase useCase,
		[FromBody] JobRequest request,
		CancellationToken cancellationToken
	) {
		Job job = await useCase.ExecuteAsync(request, cancellationToken);

		return Created($"api/jobs/{job.Id}", new JobResponse(job));
	}

	[HttpGet("{jobId}")]
	public async Task<IActionResult> GetJobById(
		[FromServices] IGetJobUseCase useCase,
		[FromRoute] string jobId,
		CancellationToken cancellationToken
	) {
		return Ok(await useCase.ExecuteAsync(jobId, cancellationToken));
	}
}
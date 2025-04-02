using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Projects;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectById;

namespace ProjectsService.API.GrpcServices;

public class ProjectsGrpcService(
    IMediator mediator,
    ILogger<ProjectsGrpcService> logger) : Projects.Projects.ProjectsBase
{
    [Authorize]
    public override async Task<GetProjectByIdResponse> GetProjectById(GetProjectByIdRequest request, ServerCallContext context)
    {
        logger.LogInformation("Getting project by ID: {ProjectId}", request.Id);
        
        var project = await mediator.Send(new GetProjectByIdQuery(Guid.Parse(request.Id)));
        
        logger.LogInformation("Successfully returned project data for {ProjectId}", request.Id);
        
        return new GetProjectByIdResponse
        {
            Id = project.Id.ToString(), 
            BudgetInCents = (int)(project.Budget * 100), 
            PaymentIntentId = project.PaymentIntentId, 
            FreelancerId = project.FreelancerId.ToString()
        };
    }
}
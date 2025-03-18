using Microsoft.AspNetCore.Authorization;
using ProjectsService.API.Constants;
using ProjectsService.API.Contracts.CommonContracts;
using ProjectsService.Application.UseCases.Commands.CategoryUseCases.CreateCategory;
using ProjectsService.Application.UseCases.Commands.CategoryUseCases.DeleteCategory;
using ProjectsService.Application.UseCases.Commands.CategoryUseCases.UpdateCategory;
using ProjectsService.Application.UseCases.Queries.CategoryUseCases.GetAllCategories;
using ProjectsService.Application.UseCases.Queries.CategoryUseCases.GetCategoryById;

namespace ProjectsService.API.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryDto categoryDto, CancellationToken cancellationToken = default)
    {
        await mediator.Send(new CreateCategoryCommand(categoryDto.Name), cancellationToken);

        return Created();
    }

    [HttpGet]
    [Route("{categoryId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetCategoryById([FromRoute] Guid categoryId, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetCategoryByIdQuery(categoryId), cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllCategories([FromQuery] GetPaginatedListRequest request, 
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAllCategoriesQuery(request.PageNo, request.PageSize), cancellationToken);

        return Ok(result);
    }

    [HttpPut]
    [Route("{categoryId:guid}")]
    [Authorize(Policy = AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> UpdateCategory([FromRoute] Guid categoryId, [FromBody] CategoryDto categoryDto, 
        CancellationToken cancellationToken = default)
    {
        await mediator.Send(new UpdateCategoryCommand(categoryId, categoryDto.Name), cancellationToken);

        return NoContent();
    }

    [HttpDelete]
    [Route("{categoryId:guid}")]
    [Authorize(Policy = AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> DeleteCategory([FromRoute] Guid categoryId, CancellationToken cancellationToken = default)
    {
        await mediator.Send(new DeleteCategoryCommand(categoryId), cancellationToken);
        
        return NoContent();
    }
}
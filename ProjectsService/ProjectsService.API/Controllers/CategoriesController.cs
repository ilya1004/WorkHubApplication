using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjectsService.API.DTOs;

namespace ProjectsService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO categoryDto, CancellationToken cancellationToken)
    {
        await mediator.Send();

        return Created();
    }
}
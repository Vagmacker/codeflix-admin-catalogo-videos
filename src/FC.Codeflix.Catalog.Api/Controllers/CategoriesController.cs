using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using FC.Codeflix.Catalog.Domain.Pagination;
using FC.Codeflix.Catalog.Api.Category.Models;
using FC.Codeflix.Catalog.Application.Category;
using FC.Codeflix.Catalog.Application.Category.Create;
using FC.Codeflix.Catalog.Application.Category.Delete;
using FC.Codeflix.Catalog.Application.Category.Retrieve.Get;
using FC.Codeflix.Catalog.Application.Category.Retrieve.List;

namespace FC.Codeflix.Catalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CategoriesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CategoryOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(summary: "Create a new category")]
    [SwaggerResponse(201, "The product was created", typeof(CategoryOutput))]
    [SwaggerResponse(400, "A validation error was thrown")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest input, CancellationToken cancellationToken)
    {
        var aCommand = CreateCategoryCommand.With(input.Name, input.Description, input.IsActive);
        var output = await mediator.Send(aCommand, cancellationToken);

        return CreatedAtAction(nameof(Create), new { output.Id }, output);
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(summary: "Get a category by it's identifier")]
    [ProducesResponseType(typeof(CategoryOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var output = await mediator.Send(new GetCategoryCommand(id), cancellationToken);
        return Ok(output);
    }

    [HttpGet]
    [SwaggerOperation(summary: "List all categories paginated")]
    [ProducesResponseType(typeof(ListCategoriesOutput), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery(Name = "per_page")] int perPage = 10,
        [FromQuery] string search = "",
        [FromQuery] string sort = "name",
        [FromQuery] SearchOrder dir = SearchOrder.Asc
    )
    {
        var aCommand = new ListCategoriesCommand(page, perPage, search, sort, dir);
        var output = await mediator.Send(aCommand, cancellationToken);

        return Ok(output.Data);
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(summary: "Delete a category by it's identifier")]
    [SwaggerResponse(204, "Category deleted successfully")]
    [SwaggerResponse(404, "Category was not found")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        await mediator.Send(new DeleteCategoryCommand(id), cancellationToken);
        return NoContent();
    }
}
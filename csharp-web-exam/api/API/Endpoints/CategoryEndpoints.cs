using api.Application.DTOs;
using api.Application.UseCases.Categories;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace api.API.Endpoints;

public static class CategoryEndpoints
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(CategoryEndpoints));

    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/categories")
            .WithTags("Categories")
            .WithOpenApi()
            .RequireAuthorization(); // 🔒 Require JWT authentication for all category endpoints

        // GET /api/categories
        group.MapGet("/", async (GetAllCategoriesUseCase useCase) =>
        {
            _log.Info("GET /api/categories - Retrieving all categories");
            try
            {
                var categories = await useCase.ExecuteAsync();
                _log.Info($"Successfully retrieved {categories.Count()} categories");
                return Results.Ok(categories);
            }
            catch (Exception ex)
            {
                _log.Error("Error retrieving categories", ex);
                throw;
            }
        })
        .WithName("GetAllCategories")
        .WithSummary("Get all categories")
        .WithDescription("Retrieves a list of all categories in the system. Requires authentication.")
        .Produces<IEnumerable<CategoryDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        // GET /api/categories/{id}
        group.MapGet("/{id:int}", async (int id, GetCategoryByIdUseCase useCase) =>
        {
            _log.Info($"GET /api/categories/{id} - Retrieving category");
            try
            {
                var category = await useCase.ExecuteAsync(id);
                if (category == null)
                {
                    _log.Warn($"Category with ID {id} not found");
                    return Results.NotFound(new { message = $"Category with ID {id} not found" });
                }

                _log.Info($"Successfully retrieved category with ID {id}");
                return Results.Ok(category);
            }
            catch (Exception ex)
            {
                _log.Error($"Error retrieving category with ID {id}", ex);
                throw;
            }
        })
        .WithName("GetCategoryById")
        .WithSummary("Get category by ID")
        .WithDescription("Retrieves a specific category by its unique identifier")
        .Produces<CategoryDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        // POST /api/categories
        group.MapPost("/", async ([FromBody] CreateCategoryDto createDto, CreateCategoryUseCase useCase) =>
        {
            _log.Info($"POST /api/categories - Creating new category: {createDto.Name}");
            try
            {
                var category = await useCase.ExecuteAsync(createDto);
                _log.Info($"Successfully created category with ID {category.Id}");
                return Results.CreatedAtRoute("GetCategoryById", new { id = category.Id }, category);
            }
            catch (Exception ex)
            {
                _log.Error($"Error creating category: {createDto.Name}", ex);
                throw;
            }
        })
        .WithName("CreateCategory")
        .WithSummary("Create a new category")
        .WithDescription("Creates a new category with the provided information. Requires authentication.")
        .Produces<CategoryDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        // PUT /api/categories/{id}
        group.MapPut("/{id:int}", async (int id, [FromBody] UpdateCategoryDto updateDto, UpdateCategoryUseCase useCase) =>
        {
            _log.Info($"PUT /api/categories/{id} - Updating category");
            try
            {
                var success = await useCase.ExecuteAsync(id, updateDto);
                if (!success)
                {
                    _log.Warn($"Category with ID {id} not found for update");
                    return Results.NotFound(new { message = $"Category with ID {id} not found" });
                }

                _log.Info($"Successfully updated category with ID {id}");
                return Results.NoContent();
            }
            catch (Exception ex)
            {
                _log.Error($"Error updating category with ID {id}", ex);
                throw;
            }
        })
        .WithName("UpdateCategory")
        .WithSummary("Update an existing category")
        .WithDescription("Updates an existing category with the provided information. Requires authentication.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        // DELETE /api/categories/{id}
        group.MapDelete("/{id:int}", async (int id, DeleteCategoryUseCase useCase) =>
        {
            _log.Info($"DELETE /api/categories/{id} - Deleting category");
            try
            {
                var success = await useCase.ExecuteAsync(id);
                if (!success)
                {
                    _log.Warn($"Category with ID {id} not found for deletion");
                    return Results.NotFound(new { message = $"Category with ID {id} not found" });
                }

                _log.Info($"Successfully deleted category with ID {id}");
                return Results.NoContent();
            }
            catch (Exception ex)
            {
                _log.Error($"Error deleting category with ID {id}", ex);
                throw;
            }
        })
        .WithName("DeleteCategory")
        .WithSummary("Delete a category")
        .WithDescription("Deletes a category and all associated products (CASCADE DELETE)")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}

using api.Application.DTOs;
using api.Application.UseCases.Products;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace api.API.Endpoints;

public static class ProductEndpoints
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(ProductEndpoints));

    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
            .WithTags("Products")
            .WithOpenApi()
            .RequireAuthorization(); // 🔒 Require JWT authentication for all product endpoints

        // GET /api/products?page=1&pageSize=10&search=...&categoryId=...&sortBy=...&sortDesc=false
        group.MapGet("/", async (
            GetProductsUseCase useCase,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDesc = false
            ) =>
        {
            _log.Info($"GET /api/products - Page: {page}, PageSize: {pageSize}, Search: {search}, CategoryId: {categoryId}, SortBy: {sortBy}, SortDesc: {sortDesc}");
            try
            {
                var result = await useCase.ExecuteAsync(page, pageSize, search, categoryId, sortBy, sortDesc);
                _log.Info($"Successfully retrieved {result.Items.Count()} products (Total: {result.TotalCount})");
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                _log.Error("Error retrieving products", ex);
                throw;
            }
        })
        .WithName("GetProducts")
        .WithSummary("Get paginated products")
        .WithDescription("Retrieves a paginated list of products with optional filtering, searching, and sorting. Requires authentication.")
        .Produces<PaginatedResultDto<ProductDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        // GET /api/products/grouped
        group.MapGet("/grouped", async (GetGroupedProductsUseCase useCase) =>
        {
            _log.Info("GET /api/products/grouped - Retrieving products grouped by category");
            try
            {
                var groups = await useCase.ExecuteAsync();
                _log.Info($"Successfully retrieved {groups.Count()} product groups");
                return Results.Ok(groups);
            }
            catch (Exception ex)
            {
                _log.Error("Error retrieving grouped products", ex);
                throw;
            }
        })
        .WithName("GetProductsGrouped")
        .WithSummary("Get products grouped by category")
        .WithDescription("Retrieves products grouped by category with aggregated statistics. Requires authentication.")
        .Produces<IEnumerable<ProductGroupDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        // GET /api/products/{id}
        group.MapGet("/{id:int}", async (int id, GetProductByIdUseCase useCase) =>
        {
            _log.Info($"GET /api/products/{id} - Retrieving product");
            try
            {
                var product = await useCase.ExecuteAsync(id);
                if (product == null)
                {
                    _log.Warn($"Product with ID {id} not found");
                    return Results.NotFound(new { message = $"Product with ID {id} not found" });
                }

                _log.Info($"Successfully retrieved product with ID {id}");
                return Results.Ok(product);
            }
            catch (Exception ex)
            {
                _log.Error($"Error retrieving product with ID {id}", ex);
                throw;
            }
        })
        .WithName("GetProductById")
        .WithSummary("Get product by ID")
        .WithDescription("Retrieves a specific product by its unique identifier. Requires authentication.")
        .Produces<ProductDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        // POST /api/products
        group.MapPost("/", async ([FromBody] CreateProductDto createDto, CreateProductUseCase useCase) =>
        {
            _log.Info($"POST /api/products - Creating new product: {createDto.Name}");
            try
            {
                var product = await useCase.ExecuteAsync(createDto);
                _log.Info($"Successfully created product with ID {product.Id}");
                return Results.CreatedAtRoute("GetProductById", new { id = product.Id }, product);
            }
            catch (InvalidOperationException ex)
            {
                _log.Warn($"Validation error creating product: {ex.Message}");
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _log.Error($"Error creating product: {createDto.Name}", ex);
                throw;
            }
        })
        .WithName("CreateProduct")
        .WithSummary("Create a new product")
        .WithDescription("Creates a new product with the provided information. Requires authentication.")
        .Produces<ProductDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        // PUT /api/products/{id}
        group.MapPut("/{id:int}", async (int id, [FromBody] UpdateProductDto updateDto, UpdateProductUseCase useCase) =>
        {
            _log.Info($"PUT /api/products/{id} - Updating product");
            try
            {
                var success = await useCase.ExecuteAsync(id, updateDto);
                if (!success)
                {
                    _log.Warn($"Product with ID {id} not found for update");
                    return Results.NotFound(new { message = $"Product with ID {id} not found" });
                }

                _log.Info($"Successfully updated product with ID {id}");
                return Results.NoContent();
            }
            catch (InvalidOperationException ex)
            {
                _log.Warn($"Validation error updating product: {ex.Message}");
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _log.Error($"Error updating product with ID {id}", ex);
                throw;
            }
        })
        .WithName("UpdateProduct")
        .WithSummary("Update an existing product")
        .WithDescription("Updates an existing product with the provided information. Requires authentication.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        // DELETE /api/products/{id}
        group.MapDelete("/{id:int}", async (int id, DeleteProductUseCase useCase) =>
        {
            _log.Info($"DELETE /api/products/{id} - Deleting product");
            try
            {
                var success = await useCase.ExecuteAsync(id);
                if (!success)
                {
                    _log.Warn($"Product with ID {id} not found for deletion");
                    return Results.NotFound(new { message = $"Product with ID {id} not found" });
                }

                _log.Info($"Successfully deleted product with ID {id}");
                return Results.NoContent();
            }
            catch (Exception ex)
            {
                _log.Error($"Error deleting product with ID {id}", ex);
                throw;
            }
        })
        .WithName("DeleteProduct")
        .WithSummary("Delete a product")
        .WithDescription("Deletes a product from the system. Requires authentication.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}

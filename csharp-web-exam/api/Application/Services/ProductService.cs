using api.Application.DTOs;
using api.Application.Interfaces;
using api.Domain.Entities;
using log4net;

namespace api.Application.Services;

public class ProductService : IProductService
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(ProductService));
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    }

    public async Task<PaginatedResultDto<ProductDto>> GetProductsAsync(
        int page = 1, 
        int pageSize = 10, 
        string? searchTerm = null, 
        int? categoryId = null,
        string? sortBy = null,
        bool sortDescending = false)
    {
        _log.Info($"Retrieving products - Page: {page}, PageSize: {pageSize}, Search: {searchTerm}, CategoryId: {categoryId}");
        
        try
        {
            var (items, totalCount) = await _productRepository.GetPagedAsync(
                page, pageSize, searchTerm, categoryId, sortBy, sortDescending);

            var productDtos = items.Select(MapToDto).ToList();
            
            _log.Info($"Retrieved {productDtos.Count} products out of {totalCount} total");
            
            return new PaginatedResultDto<ProductDto>
            {
                Items = productDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _log.Error("Error retrieving products", ex);
            throw;
        }
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        _log.Info($"Retrieving product with ID: {id}");
        
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            
            if (product == null)
            {
                _log.Warn($"Product with ID {id} not found");
                return null;
            }
            
            _log.Info($"Product with ID {id} retrieved successfully");
            return MapToDto(product);
        }
        catch (Exception ex)
        {
            _log.Error($"Error retrieving product with ID {id}", ex);
            throw;
        }
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto createDto)
    {
        _log.Info($"Creating new product: {createDto.Name}");
        
        try
        {
            // Validate category exists
            var categoryExists = await _categoryRepository.ExistsAsync(createDto.CategoryId);
            if (!categoryExists)
            {
                _log.Warn($"Category with ID {createDto.CategoryId} does not exist");
                throw new InvalidOperationException($"Category with ID {createDto.CategoryId} does not exist");
            }

            var product = new Product
            {
                Name = createDto.Name,
                Price = createDto.Price,
                CategoryId = createDto.CategoryId,
                CreatedAt = DateTime.UtcNow
            };

            var id = await _productRepository.CreateAsync(product);
            product.Id = id;
            
            // Fetch category name for response
            var category = await _categoryRepository.GetByIdAsync(createDto.CategoryId);
            if (category != null)
            {
                product.Category = category;
            }
            
            _log.Info($"Product created successfully with ID: {id}");
            return MapToDto(product);
        }
        catch (Exception ex)
        {
            _log.Error($"Error creating product: {createDto.Name}", ex);
            throw;
        }
    }

    public async Task<bool> UpdateProductAsync(int id, UpdateProductDto updateDto)
    {
        _log.Info($"Updating product with ID: {id}");
        
        try
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);
            
            if (existingProduct == null)
            {
                _log.Warn($"Product with ID {id} not found for update");
                return false;
            }

            // Validate category exists
            var categoryExists = await _categoryRepository.ExistsAsync(updateDto.CategoryId);
            if (!categoryExists)
            {
                _log.Warn($"Category with ID {updateDto.CategoryId} does not exist");
                throw new InvalidOperationException($"Category with ID {updateDto.CategoryId} does not exist");
            }

            existingProduct.Name = updateDto.Name;
            existingProduct.Price = updateDto.Price;
            existingProduct.CategoryId = updateDto.CategoryId;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            var result = await _productRepository.UpdateAsync(existingProduct);
            
            if (result)
            {
                _log.Info($"Product with ID {id} updated successfully");
            }
            else
            {
                _log.Warn($"Failed to update product with ID {id}");
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _log.Error($"Error updating product with ID {id}", ex);
            throw;
        }
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        _log.Info($"Deleting product with ID: {id}");
        
        try
        {
            var exists = await _productRepository.ExistsAsync(id);
            
            if (!exists)
            {
                _log.Warn($"Product with ID {id} not found for deletion");
                return false;
            }

            var result = await _productRepository.DeleteAsync(id);
            
            if (result)
            {
                _log.Info($"Product with ID {id} deleted successfully");
            }
            else
            {
                _log.Warn($"Failed to delete product with ID {id}");
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _log.Error($"Error deleting product with ID {id}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<ProductGroupDto>> GetProductsGroupedByCategoryAsync()
    {
        _log.Info("Retrieving products grouped by category");
        
        try
        {
            var groupedData = await _productRepository.GetGroupedByCategoryAsync();
            
            var result = groupedData.Select(g => new ProductGroupDto
            {
                CategoryId = g.CategoryId,
                CategoryName = g.CategoryName,
                ProductCount = g.Count,
                TotalValue = g.TotalValue,
                AveragePrice = g.AvgPrice,
                MinPrice = g.MinPrice,
                MaxPrice = g.MaxPrice
            }).ToList();
            
            _log.Info($"Retrieved {result.Count} category groups");
            return result;
        }
        catch (Exception ex)
        {
            _log.Error("Error retrieving grouped products", ex);
            throw;
        }
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}

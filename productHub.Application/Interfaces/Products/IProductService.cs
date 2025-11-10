using productHub.Application.DTOs.Products;

namespace productHub.Application.Interfaces;

public interface IProductService
{
    Task<ProductDto> CreateAsync(ProductDto product);
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto?> GetByIdAsync(Guid id);
    Task<bool> UpdateAsync(Guid id, ProductDto product);
    Task<bool> DeleteAsync(Guid id);
}
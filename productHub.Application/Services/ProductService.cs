using productHub.Application.DTOs.Products;
using productHub.Application.Interfaces;
using productHub.Domain.Entities;
using productHub.Domain.Interfaces;

namespace productHub.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(
        IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto> CreateAsync(ProductDto dto)
    {
        var product = new Product(dto.Name, dto.Description, dto.Price, dto.Stock);
        await _productRepository.AddAsync(product);

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock
        };
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();

        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock
        });
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        var p = await _productRepository.GetByIdAsync(id);
        if (p is null) return null;

        return new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock
        };
    }

    public async Task<bool> UpdateAsync(Guid id, ProductDto dto)
    {
        var existing = await _productRepository.GetByIdAsync(id);
        if (existing is null) return false;

        var updated = new Product(dto.Name, dto.Description, dto.Price, dto.Stock);

        typeof(Product).GetProperty("Id")!.SetValue(updated, id);

        await _productRepository.UpdateAsync(updated);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await _productRepository.DeleteAsync(id);
        return true;
    }
}

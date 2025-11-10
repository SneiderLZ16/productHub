using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using productHub.Application.DTOs.Products;
using productHub.Application.Interfaces;

namespace productHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // todas requieren JWT 
public class ProductsController : ControllerBase
{
    private readonly IProductService _products;

    public ProductsController(IProductService products)
    {
        _products = products;
    }

    // POST /api/products
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductDto dto)
    {
        var created = await _products.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // GET /api/products
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _products.GetAllAsync();
        return Ok(list);
    }

    // GET /api/products/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var item = await _products.GetByIdAsync(id);
        if (item is null) return NotFound();
        return Ok(item);
    }

    // PUT /api/products/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductDto dto)
    {
        var ok = await _products.UpdateAsync(id, dto);
        if (!ok) return NotFound();
        return NoContent();
    }

    // DELETE /api/products/{id}  (solo Admin)
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await _products.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}
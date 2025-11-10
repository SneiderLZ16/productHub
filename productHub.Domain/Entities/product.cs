namespace productHub.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; }

   
    protected Product() { }

    public Product(string name, string? description, decimal price, int stock)
    {
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
    }
}
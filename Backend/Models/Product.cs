using Backend.Dtos;

namespace Backend.Models
{
    public class Product
    {
        public Product() { }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public int? StockQuantity { get; set; }
        public Product(CreateProductRequest newProduct)
        {
            Id = 0;
            Name = newProduct.Name;
            Description = newProduct.Description;
            Price = newProduct.Price;
            StockQuantity = newProduct.StockQuantity;
        }
    }
}
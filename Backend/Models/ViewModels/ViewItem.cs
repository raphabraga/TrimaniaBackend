using System.Runtime.CompilerServices;
namespace Backend.Models.ViewModels
{
    public class ViewItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public ViewItem(ChartItem item)
        {
            ProductId = item.Product.Id;
            ProductName = item.Product.Name;
            Price = item.Price;
            Quantity = item.Quantity;
        }
    }

}
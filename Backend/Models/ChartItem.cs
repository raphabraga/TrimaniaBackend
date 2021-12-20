namespace Backend.Models
{
    public class ChartItem
    {
        public ChartItem() { }
        public int Id { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
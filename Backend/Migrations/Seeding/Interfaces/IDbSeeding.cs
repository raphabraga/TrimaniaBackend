using Backend.Models;

namespace Backend.Migrations.Seeding.Interfaces
{
    public interface IDbSeeding
    {
        public Address GenerateAddress();
        public User GenerateUser();
        public Product GenerateProduct();
        public ChartItem GenerateChartItem(int id);
        public Order GenerateOrder();

    }
}
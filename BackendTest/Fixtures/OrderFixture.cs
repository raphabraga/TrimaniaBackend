using System;
using Backend.Repositories;
using Backend.Services;
using BackendTest.Mocks;

namespace BackendTest.Fixtures
{
    public class OrderFixture : IDisposable
    {
        public OrderService OrderService { get; set; }
        public ApplicationContextMoq Context { get; set; }
        public OrderFixture()
        {
            Context = new ApplicationContextMoq();
            DbSeedingMock.Seeding(Context);
            var unitOfWork = new UnitOfWork(Context);
            var productService = new ProductService(unitOfWork);
            OrderService = new OrderService(unitOfWork, productService);
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
        }
    }
}
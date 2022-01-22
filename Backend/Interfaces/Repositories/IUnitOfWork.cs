using System;
using System.Threading.Tasks;
using Backend.Models;

namespace Backend.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> UserRepository { get; }
        IRepository<Product> ProductRepository { get; }
        IRepository<Order> OrderRepository { get; }
        IRepository<Address> AddressRepository { get; }
        IRepository<ChartItem> ChartItemRepository { get; }
        public Task Commit();

    }
}
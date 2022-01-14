using System;
using Backend.Interfaces.Repositories;
using Backend.Models;

namespace Backend.Interfaces.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> UserRepository { get; }
        IRepository<Product> ProductRepository { get; }
        IRepository<Order> OrderRepository { get; }
        IRepository<Address> AddressRepository { get; }
        IRepository<ChartItem> ChartItemRepository { get; }
        public void Commit();

    }
}
using System;
using System.Threading.Tasks;
using Backend.Data;
using Backend.Interfaces.Repositories;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private DbContext _applicationContext = null;
        private Repository<User> _userRepository = null;
        private Repository<Product> _productRepository = null;
        private Repository<Order> _orderRepository = null;
        private Repository<Address> _addressRepository = null;
        private Repository<ChartItem> _chartItemRepository = null;

        public IRepository<User> UserRepository
        {
            get
            {
                if (_userRepository == null)
                    _userRepository = new Repository<User>(_applicationContext);
                return _userRepository;
            }
        }

        public IRepository<Product> ProductRepository
        {
            get
            {
                if (_productRepository == null)
                    _productRepository = new Repository<Product>(_applicationContext);
                return _productRepository;
            }
        }

        public IRepository<Order> OrderRepository
        {
            get
            {
                if (_orderRepository == null)
                    _orderRepository = new Repository<Order>(_applicationContext);
                return _orderRepository;
            }
        }

        public IRepository<Address> AddressRepository
        {
            get
            {
                if (_addressRepository == null)
                    _addressRepository = new Repository<Address>(_applicationContext);
                return _addressRepository;
            }
        }

        public IRepository<ChartItem> ChartItemRepository
        {
            get
            {
                if (_chartItemRepository == null)
                    _chartItemRepository = new Repository<ChartItem>(_applicationContext);
                return _chartItemRepository;
            }
        }

        public UnitOfWork(DbContext context)
        {
            _applicationContext = context;
        }

        public async Task Commit()
        {
            await _applicationContext.SaveChangesAsync();
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                    _applicationContext.Dispose();
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
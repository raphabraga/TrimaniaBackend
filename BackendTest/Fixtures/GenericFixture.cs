using System;
using Backend.Repositories;
using BackendTest.Mocks;

namespace BackendTest.Fixtures
{
    public class GenericFixture<T> : IDisposable where T : class, new()
    {
        public T Service { get; set; }
        public ApplicationContextMoq Context { get; set; }
        public GenericFixture()
        {
            Context = new ApplicationContextMoq();
            DbSeedingMock.Seeding(Context);
            var unitOfWork = new UnitOfWork(Context);
            Service = (T)Activator.CreateInstance(typeof(T), unitOfWork);
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
        }
    }
}
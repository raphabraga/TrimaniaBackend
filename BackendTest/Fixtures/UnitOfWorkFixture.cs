using System;
using Backend.Repositories;
using Backend.Services;
using BackendTest.Mocks;

namespace BackendTest.Fixtures
{
    public class UnitOfWorkFixture : IDisposable
    {
        public SalesReportService SalesReportService { get; set; }
        public ApplicationContextMoq Context { get; set; }
        public UnitOfWorkFixture()
        {
            Context = new ApplicationContextMoq();
            DbSeedingMock.Seeding(Context);
            var unitOfWork = new UnitOfWork(Context);
            SalesReportService = new SalesReportService(unitOfWork);
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
        }
    }
}
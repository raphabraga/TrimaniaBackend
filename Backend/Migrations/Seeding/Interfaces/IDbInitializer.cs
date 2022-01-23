namespace Backend.Migrations.Seeding.Interfaces
{
    public interface IDbInitializer
    {
        void Initialize();
        void SeedAdmin();
        void SeedData();
    }
}
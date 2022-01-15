namespace Backend.Migrations
{
    public interface IDbInitializer
    {
        void Initialize();
        void SeedAdmin();
        void SeedData();
    }
}
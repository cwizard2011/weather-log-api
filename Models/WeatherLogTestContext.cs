using Microsoft.EntityFrameworkCore;


namespace weatherlogapi.Models
{
    public class TestDbContext : DbContext
    {
        public string ConnectionString { get; set; }
        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        {
        }


        public DbSet<WeatherLog> WeatherLogs { get; set; }
    }
}
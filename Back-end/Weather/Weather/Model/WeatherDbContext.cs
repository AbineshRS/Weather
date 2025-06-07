using Microsoft.EntityFrameworkCore;

namespace Weather.Model
{
    public class WeatherDbContext:DbContext
    {
        public WeatherDbContext(DbContextOptions options) : base(options) { }

        public DbSet<WeatherRecord> Weathers { get; set; }
    }
}

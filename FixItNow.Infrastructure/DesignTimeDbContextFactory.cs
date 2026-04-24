using FixItNow.Infrastructure.Models.Commons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FixItNow.Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FixItNowDataContext>
    {
        public FixItNowDataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FixItNowDataContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Database=FixItNow;Username=postgres;Password=ernesto04");
            return new FixItNowDataContext(optionsBuilder.Options);
        }
    }
}

using Claims.Models.Claim;
using Claims.Models.Cover;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Claims.Persistence
{
    public class ClaimsContext : DbContext
    {
        public DbSet<Claim> Claims { get; init; }
        public DbSet<Cover> Covers { get; init; }

        public ClaimsContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Claim>().ToCollection("claims");
            modelBuilder.Entity<Cover>().ToCollection("covers");
        }
    }
}

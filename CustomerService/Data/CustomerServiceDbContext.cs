using CustomerService.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Data
{
    public class CustomerServiceDbContext : DbContext
    {
        public CustomerServiceDbContext(DbContextOptions<CustomerServiceDbContext> options) : base(options) {
        }

        public DbSet<Agent> Agents { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Purchase> Purchases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agent>().HasData(
                new Agent()
                {
                    Id = 1,
                    FirstName = "Marko",
                    LastName = "Markovic",
                    Email = "markomarkovic@gmail.com"
                },
                new Agent()
                {
                    Id = 2,
                    FirstName = "Ivan",
                    LastName = "Ivanovic",
                    Email = "ivanivanovic@gmail.com"
                },
                new Agent()
                {
                    Id = 3,
                    FirstName = "Nikola",
                    LastName = "Nikolic",
                    Email = "nikolanikolic@gmail.com"
                }
            );

            modelBuilder.Entity<Purchase>()
      .HasOne(p => p.Agent)
      .WithMany(a => a.Purchases)
      .HasForeignKey(p => p.AgentId);

            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.Campaign)
                .WithMany(c => c.Purchases)
                .HasForeignKey(p => p.CampaignId);
        }
    }
}

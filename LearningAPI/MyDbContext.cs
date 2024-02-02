using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UPlay.Models;

namespace UPlay
{
    public class MyDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public MyDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? connectionString = _configuration.GetConnectionString("MyConnection");

            if (connectionString != null)
            {
                optionsBuilder.UseMySQL(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            base.OnModelCreating(modelBuilder);

            // Configure the one-to-many relationship between User (referred) and ReferralTracking
            modelBuilder.Entity<ReferralTracking>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.ReferralTrackings) // Assuming User has a collection property for this
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the one-to-many relationship between User (referring) and ReferralTracking
            modelBuilder.Entity<ReferralTracking>()
                .HasOne(rt => rt.ReferringUser)
                .WithMany(u => u.ReferredUsers) // Assuming User has a collection property for this
                .HasForeignKey(rt => rt.ReferringUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ReferralTracking> ReferralTrackings { get; set; }
    }
}

// MyDbContext.cs
using LearningAPI.Models;
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

        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ReferralTracking> ReferralTrackings { get; set; }
    }
}

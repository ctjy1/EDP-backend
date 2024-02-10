using Microsoft.EntityFrameworkCore;
using Uplay.Models;
using Uplay.Models.BudgetModels;
using Uplay.Models.RewardModels;
using Uplay.Models.SurveyModels;
using Uplay.Models.ActivityModels;

namespace Uplay
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

        // user databases
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ReferralTracking> ReferralTrackings { get; set; }

        // budget databases
		public DbSet<Cart> Carts { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderDetails> OrderDetails { get; set; }
		public DbSet<UserBudget> UserBudget { get; set; }

        // reward databases
		public DbSet<Reward> Rewards { get; set; }

		// feedback & survey databases
		public DbSet<Cust_Feedback> Customer_Feedback { get; set; }
		public DbSet<Cust_Survey> Customer_Surveys { get; set; }

		// activity databases

		public DbSet<Activity> Activitys { get; set; }
	}
}

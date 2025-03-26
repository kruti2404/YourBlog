using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using BlogProject.Models;
namespace BlogProject.Data
{
    public class ProgramDbContext : DbContext
    {
        public ProgramDbContext(DbContextOptions<ProgramDbContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=LAPTOP-46S21PDI;Database=BlogDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Blogs>()
                .HasKey(b => b.Id);

            modelBuilder.Entity<Blogs>()
                .HasOne(b => b.user)
                .WithMany(u => u.Blogs)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Keep Cascade on Blogs if needed

            modelBuilder.Entity<Blogs>()
                .HasMany(b => b.Genres)
                .WithMany(g => g.Blogs);

            modelBuilder.Entity<Likes>()
                .HasOne(l => l.Blog)
                .WithMany(b => b.Likes)
                .HasForeignKey(l => l.BlogId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Likes>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Modify Comments Foreign Key to User to avoid multiple cascade paths
            modelBuilder.Entity<Blogcomments>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserID)
                .OnDelete(DeleteBehavior.Cascade); // Change this to NoAction to prevent conflict
        }

        public DbSet<User> User { get; set; }
        public DbSet<Blogs> Blogs { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Blogcomments> Comments { get; set; }
        public DbSet<Likes> Likes { get; set; }

    }
}

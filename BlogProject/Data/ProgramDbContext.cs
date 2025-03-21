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
            .HasForeignKey(b => b.UserId).IsRequired(true)
            .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Blogs>()
                .HasMany(b => b.Genres)
                .WithMany(g => g.Blogs);


        }
        public DbSet<User> User { get; set; }
        public DbSet<Blogs> Blogs { get; set; } 
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Blogcomments> Comments { get; set; }

    }
}

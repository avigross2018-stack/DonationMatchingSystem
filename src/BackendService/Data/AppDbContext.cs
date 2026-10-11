using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Donor> Donors { get; set; }
        public DbSet<Recipient> Recipients { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<PopulationType> PopulationTypes { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Redemption> Redemptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User -> Donor (One-to-One)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Donor)
                .WithOne(d => d.User)
                .HasForeignKey<Donor>(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);



            // User -> Recipient (One-to-One)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Recipient)
                .WithOne(r => r.User)
                .HasForeignKey<Recipient>(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User -> Admin (One-to-One)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Admin)
                .WithOne(a => a.User)
                .HasForeignKey<Admin>(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Donor -> Donations (One-to-Many)
            modelBuilder.Entity<Donation>()
                .HasOne(d => d.Donor)
                .WithMany(d => d.Donations)
                .HasForeignKey(d => d.DonorId)
                .OnDelete(DeleteBehavior.Restrict);

            // PopulationType -> Donations (One-to-Many)
            modelBuilder.Entity<Donation>()
                .HasOne(d => d.PopulationType)
                .WithMany(p => p.Donations)
                .HasForeignKey(d => d.PopulationTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // PopulationType -> Recipients (One-to-Many)
            modelBuilder.Entity<Recipient>()
                .HasOne(r => r.PopulationType)
                .WithMany(p => p.Recipients)
                .HasForeignKey(r => r.PopulationTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Donation -> Redemptions (One-to-Many)
            modelBuilder.Entity<Redemption>()
                .HasOne(r => r.Donation)
                .WithMany(d => d.Redemptions)
                .HasForeignKey(r => r.DonationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Recipient -> Redemptions (One-to-Many)
            modelBuilder.Entity<Redemption>()
                .HasOne(r => r.Recipient)
                .WithMany(rec => rec.Redemptions)
                .HasForeignKey(r => r.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Admin -> Redemptions (One-to-Many)
            modelBuilder.Entity<Redemption>()
                .HasOne(r => r.HandledByAdmin)
                .WithMany(a => a.HandledRedemptions)
                .HasForeignKey(r => r.HandledByAdminId)
                .OnDelete(DeleteBehavior.Restrict);

            // Store strings instead of Enums
            modelBuilder.Entity<User>()
                .Property(u => u.UserType)
                .HasMaxLength(20)
                .IsRequired();

            modelBuilder.Entity<Donation>()
                .Property(d => d.Status)
                .HasMaxLength(20)
                .IsRequired();

            modelBuilder.Entity<Redemption>()
                .Property(r => r.Status)
                .HasMaxLength(20)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Phone)
                .IsUnique();
        }
    }
}
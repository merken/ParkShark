using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ParkShark.Domain;

namespace ParkShark.Data.Model
{
    public class ParkSharkDbContext : DbContext
    {
        public virtual DbSet<Division> Divisions { get; set; }

        public ParkSharkDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Division>()
                .ToTable("Divisions");

            modelBuilder.Entity<Division>()
                .HasKey(d => d.Id);

            modelBuilder.Entity<Division>()
                .Property(d => d.OriginalName)
                .HasColumnName("Original_Name");

            modelBuilder.Entity<Division>()
                .HasOne(d => d.ParentDivision)
                .WithMany()
                .HasForeignKey(d => d.ParentDivisionId);

            base.OnModelCreating(modelBuilder);
        }
    }
}

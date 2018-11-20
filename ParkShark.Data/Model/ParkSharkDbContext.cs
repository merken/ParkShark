using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using ParkShark.Domain;

namespace ParkShark.Data.Model
{
    public class ParkSharkDbContext : DbContext
    {
        public virtual DbSet<Division> Divisions { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<ParkingLot> ParkingLots { get; set; }
        public virtual DbSet<Member> Members { get; set; }

        public ParkSharkDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Division>()
                .ToTable("Divisions")
                .HasKey(d => d.Id);

            modelBuilder.Entity<Division>()
                .Property(d => d.OriginalName)
                .HasColumnName("Original_Name");

            modelBuilder.Entity<Division>()
                .HasOne(d => d.ParentDivision)
                .WithMany(d => d.SubDivisions)
                .HasForeignKey(d => d.ParentDivisionId);

            modelBuilder.Entity<BuildingType>()
                .ToTable("BuildingTypes")
                .HasKey(b => b.Id);

            modelBuilder.Entity<ParkingLot>()
                .ToTable("ParkingLots")
                .HasKey(p => p.Id);

            modelBuilder.Entity<ParkingLot>()
                .HasOne(p => p.BuildingType)
                .WithMany()
                .HasForeignKey(p => p.BuildingTypeId);

            modelBuilder.Entity<ParkingLot>()
                .HasOne(p => p.Division)
                .WithMany()
                .HasForeignKey(p => p.DivisionId);

            modelBuilder.Entity<ParkingLot>()
                .HasOne(p => p.Contact)
                .WithMany()
                .HasForeignKey(p => p.ContactId);

            modelBuilder.Entity<Contact>()
                .ToTable("Contacts")
                .HasKey(c => c.Id);

            modelBuilder.Entity<Contact>()
                .OwnsOne(c => c.Address,
                    contact =>
                    {
                        contact.Property(c => c.Street).HasColumnName("Street");
                        contact.Property(c => c.StreetNumber).HasColumnName("StreetNumber");
                        contact.Property(c => c.PostalCode).HasColumnName("PostalCode");
                        contact.Property(c => c.PostalName).HasColumnName("PostalName");
                    });

            modelBuilder.Entity<Member>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<Member>()
                .HasOne(m => m.Contact)
                .WithMany()
                .HasForeignKey(m => m.ContactId);

            modelBuilder.Entity<Member>()
                .OwnsOne(m => m.LicensePlate,
                    licensePlate =>
                    {
                        licensePlate.Property(c => c.Country).HasColumnName("LicensePlateNumber");
                        licensePlate.Property(c => c.Number).HasColumnName("LicensePlateCountry");
                    });

            modelBuilder.Entity<Member>()
                .Property(m => m.MemberShipLevel)
                .HasConversion<string>();

            modelBuilder.Entity<Member>()
                .HasOne(m => m.RelatedMemberShipLevel)
                .WithMany()
                .HasForeignKey(m => m.MemberShipLevel);

            modelBuilder.Entity<MemberShipLevel>()
                .Property(m => m.Name)
                .HasConversion<string>();

            modelBuilder.Entity<MemberShipLevel>()
                .ToTable("MemberShipLevels")
                .HasKey(m => m.Name);

            base.OnModelCreating(modelBuilder);
        }
    }
}

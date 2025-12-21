using System;
using System.Collections.Generic;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Context;

public partial class InsuranceDbContext : DbContext
{
    public InsuranceDbContext()
    {
    }

    public InsuranceDbContext(DbContextOptions<InsuranceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<ClientProfile> ClientProfiles { get; set; }

    public virtual DbSet<InsuranceClaim> InsuranceClaims { get; set; }

    public virtual DbSet<InsurancePolicy> InsurancePolicies { get; set; }

    public virtual DbSet<Manager> Managers { get; set; }

    public virtual DbSet<Model> Models { get; set; }

    public virtual DbSet<PaymentForClaim> PaymentForClaims { get; set; }

    public virtual DbSet<PaymentForPolicy> PaymentForPolicies { get; set; }

    public virtual DbSet<StatusOfClaim> StatusOfClaims { get; set; }

    public virtual DbSet<StatusOfPolicy> StatusOfPolicies { get; set; }

    public virtual DbSet<TypeOfPolicy> TypeOfPolicies { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.ToTable("Brand");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
        });

        modelBuilder.Entity<ClientProfile>(entity =>
        {
            entity.ToTable("ClientProfile");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.DriverLicense)
                .HasMaxLength(20)
                .HasColumnName("driverLicense");
            entity.Property(e => e.DrivingExperience).HasColumnName("drivingExperience");
            entity.Property(e => e.Passport)
                .HasMaxLength(20)
                .HasColumnName("passport");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.ClientProfile)
                .HasForeignKey<ClientProfile>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClientProfile_User");
        });

        modelBuilder.Entity<InsuranceClaim>(entity =>
        {
            entity.ToTable("InsuranceClaim");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClaimDate)
                .HasColumnType("datetime")
                .HasColumnName("claimDate");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.EstimatedDamage).HasColumnName("estimatedDamage");
            entity.Property(e => e.Location)
                .HasMaxLength(100)
                .HasColumnName("location");
            entity.Property(e => e.PolicyId).HasColumnName("policy_id");
            entity.Property(e => e.ProcessedBy).HasColumnName("processed_by");
            entity.Property(e => e.StatusId).HasColumnName("status_id");

            entity.HasOne(d => d.Policy).WithMany(p => p.InsuranceClaims)
                .HasForeignKey(d => d.PolicyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InsuranceClaim_InsurancePolicy");

            entity.HasOne(d => d.ProcessedByNavigation).WithMany(p => p.InsuranceClaims)
                .HasForeignKey(d => d.ProcessedBy)
                .HasConstraintName("FK_InsuranceClaim_Manager");

            entity.HasOne(d => d.Status).WithMany(p => p.InsuranceClaims)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InsuranceClaim_StatusOfClaim");
        });

        modelBuilder.Entity<InsurancePolicy>(entity =>
        {
            entity.ToTable("InsurancePolicy");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BasePrice).HasColumnName("basePrice");
            entity.Property(e => e.BonusMalusCoefficient).HasColumnName("bonusMalusCoefficient");
            entity.Property(e => e.CancelledBy).HasColumnName("cancelled_by");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("endDate");
            entity.Property(e => e.ExperienceCoefficient).HasColumnName("experienceCoefficient");
            entity.Property(e => e.PolicyNumber)
                .HasMaxLength(20)
                .HasColumnName("policyNumber");
            entity.Property(e => e.PowerCoefficient).HasColumnName("powerCoefficient");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("startDate");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.TotalPrice)
                .HasComputedColumnSql("((([basePrice]*[powerCoefficient])*[experienceCoefficient])*[bonusMalusCoefficient])", false)
                .HasColumnName("totalPrice");
            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");

            entity.HasOne(d => d.CancelledByNavigation).WithMany(p => p.InsurancePolicies)
                .HasForeignKey(d => d.CancelledBy)
                .HasConstraintName("FK_InsurancePolicy_Manager");

            entity.HasOne(d => d.Status).WithMany(p => p.InsurancePolicies)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InsurancePolicy_StatusOfPolicy");

            entity.HasOne(d => d.Type).WithMany(p => p.InsurancePolicies)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InsurancePolicy_TypeOfPolicy");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.InsurancePolicies)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InsurancePolicy_Vehicle");
        });

        modelBuilder.Entity<Manager>(entity =>
        {
            entity.ToTable("Manager");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Manager)
                .HasForeignKey<Manager>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Manager_User");
        });

        modelBuilder.Entity<Model>(entity =>
        {
            entity.ToTable("Model");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasOne(d => d.Brand).WithMany(p => p.Models)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Model_Brand");
        });

        modelBuilder.Entity<PaymentForClaim>(entity =>
        {
            entity.ToTable("PaymentForClaim");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.AuthorizedBy).HasColumnName("authorized_by");
            entity.Property(e => e.ClaimId).HasColumnName("claim_id");
            entity.Property(e => e.PaymentDate)
                .HasColumnType("datetime")
                .HasColumnName("paymentDate");

            entity.HasOne(d => d.AuthorizedByNavigation).WithMany(p => p.PaymentForClaims)
                .HasForeignKey(d => d.AuthorizedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PaymentForClaim_Manager");

            entity.HasOne(d => d.Claim).WithMany(p => p.PaymentForClaims)
                .HasForeignKey(d => d.ClaimId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PaymentForClaim_InsuranceClaim");
        });

        modelBuilder.Entity<PaymentForPolicy>(entity =>
        {
            entity.ToTable("PaymentForPolicy");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.PolicyId).HasColumnName("policy_id");

            entity.HasOne(d => d.Client).WithMany(p => p.PaymentForPolicies)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PaymentForPolicy_ClientProfile");

            entity.HasOne(d => d.Policy).WithMany(p => p.PaymentForPolicies)
                .HasForeignKey(d => d.PolicyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PaymentForPolicy_InsurancePolicy");
        });

        modelBuilder.Entity<StatusOfClaim>(entity =>
        {
            entity.ToTable("StatusOfClaim");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<StatusOfPolicy>(entity =>
        {
            entity.ToTable("StatusOfPolicy");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<TypeOfPolicy>(entity =>
        {
            entity.ToTable("TypeOfPolicy");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("firstName");
            entity.Property(e => e.IsClient).HasColumnName("isClient");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("lastName");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .HasColumnName("login");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(50)
                .HasColumnName("middleName");
            entity.Property(e => e.PasswordHash).HasColumnName("passwordHash");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phoneNumber");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.ToTable("Vehicle");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Category)
                .HasMaxLength(10)
                .HasColumnName("category");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.Color)
                .HasMaxLength(50)
                .HasColumnName("color");
            entity.Property(e => e.ModelId).HasColumnName("model_id");
            entity.Property(e => e.PlateNum)
                .HasMaxLength(20)
                .HasColumnName("plateNum");
            entity.Property(e => e.PowerHp).HasColumnName("powerHP");
            entity.Property(e => e.Vin)
                .HasMaxLength(50)
                .HasColumnName("VIN");
            entity.Property(e => e.YearOfProduction).HasColumnName("yearOfProduction");

            entity.HasOne(d => d.Client).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehicle_ClientProfile");

            entity.HasOne(d => d.Model).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.ModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehicle_Model");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

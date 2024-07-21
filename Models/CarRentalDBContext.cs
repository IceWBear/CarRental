using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Project_RentACar.Models
{
    public partial class CarRentalDBContext : DbContext
    {
        public CarRentalDBContext()
        {
        }

        public CarRentalDBContext(DbContextOptions<CarRentalDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Branch> Branches { get; set; } = null!;
        public virtual DbSet<Car> Cars { get; set; } = null!;
        public virtual DbSet<CarManufacturer> CarManufacturers { get; set; } = null!;
        public virtual DbSet<CarType> CarTypes { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Feedback> Feedbacks { get; set; } = null!;
        public virtual DbSet<Insurance> Insurances { get; set; } = null!;
        public virtual DbSet<Maintenance> Maintenances { get; set; } = null!;
        public virtual DbSet<RentalContract> RentalContracts { get; set; } = null!;
        public virtual DbSet<RentalHistory> RentalHistories { get; set; } = null!;
        public virtual DbSet<Transaction> Transactions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                              .SetBasePath(Directory.GetCurrentDirectory())
                              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("MyCnn"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Branch>(entity =>
            {
                entity.ToTable("Branch");

                entity.Property(e => e.BranchId).HasColumnName("BranchID");

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.BranchName).HasMaxLength(100);

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            });

            modelBuilder.Entity<Car>(entity =>
            {
                entity.ToTable("Car");

                entity.HasIndex(e => e.LicensePlate, "UQ__Car__026BC15CE1F39696")
                    .IsUnique();

                entity.Property(e => e.CarId).HasColumnName("CarID");

                entity.Property(e => e.BranchId).HasColumnName("BranchID");

                entity.Property(e => e.CarTypeId).HasColumnName("CarTypeID");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.Fuel)
                    .HasMaxLength(20)
                    .HasDefaultValueSql("('Gasoline')");

                entity.Property(e => e.Image).HasMaxLength(255);

                entity.Property(e => e.LicensePlate).HasMaxLength(20);

                entity.Property(e => e.ManufacturerId).HasColumnName("ManufacturerID");

                entity.Property(e => e.Model).HasMaxLength(100);

                entity.Property(e => e.RentalPrice)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0.00))");

                entity.Property(e => e.Seats).HasDefaultValueSql("((4))");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Transmission)
                    .HasMaxLength(10)
                    .HasDefaultValueSql("('Manual')");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.Cars)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Car__BranchID__5535A963");

                entity.HasOne(d => d.CarType)
                    .WithMany(p => p.Cars)
                    .HasForeignKey(d => d.CarTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Car__CarTypeID__5629CD9C");

                entity.HasOne(d => d.Manufacturer)
                    .WithMany(p => p.Cars)
                    .HasForeignKey(d => d.ManufacturerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Car__Manufacture__571DF1D5");
            });

            modelBuilder.Entity<CarManufacturer>(entity =>
            {
                entity.HasKey(e => e.ManufacturerId)
                    .HasName("PK__CarManuf__357E5CA1DF8C9733");

                entity.ToTable("CarManufacturer");

                entity.Property(e => e.ManufacturerId).HasColumnName("ManufacturerID");

                entity.Property(e => e.Country).HasMaxLength(100);

                entity.Property(e => e.ManufacturerName).HasMaxLength(100);
            });

            modelBuilder.Entity<CarType>(entity =>
            {
                entity.ToTable("CarType");

                entity.Property(e => e.CarTypeId).HasColumnName("CarTypeID");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.TypeName).HasMaxLength(100);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.Balance)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0.00))");

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.Image).HasMaxLength(500);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Password).HasMaxLength(255);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);

                entity.Property(e => e.UnlockTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.ToTable("Feedback");

                entity.Property(e => e.FeedbackId).HasColumnName("FeedbackID");

                entity.Property(e => e.CarId).HasColumnName("CarID");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.FeedbackContent).HasMaxLength(1000);

                entity.Property(e => e.FeedbackDate)
                    .HasColumnType("date")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Feedback_Customer");
            });

            modelBuilder.Entity<Insurance>(entity =>
            {
                entity.ToTable("Insurance");

                entity.Property(e => e.InsuranceId).HasColumnName("InsuranceID");

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CarId).HasColumnName("CarID");

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.InsuranceCompany).HasMaxLength(100);

                entity.Property(e => e.PolicyNumber).HasMaxLength(50);

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.HasOne(d => d.Car)
                    .WithMany(p => p.Insurances)
                    .HasForeignKey(d => d.CarId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Insurance__CarID__59FA5E80");
            });

            modelBuilder.Entity<Maintenance>(entity =>
            {
                entity.ToTable("Maintenance");

                entity.Property(e => e.MaintenanceId).HasColumnName("MaintenanceID");

                entity.Property(e => e.CarId).HasColumnName("CarID");

                entity.Property(e => e.Cost).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.MaintenanceDate).HasColumnType("date");

                entity.HasOne(d => d.Car)
                    .WithMany(p => p.Maintenances)
                    .HasForeignKey(d => d.CarId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Maintenan__CarID__5AEE82B9");
            });

            modelBuilder.Entity<RentalContract>(entity =>
            {
                entity.HasKey(e => e.ContractId)
                    .HasName("PK__RentalCo__C90D3409A78C2738");

                entity.ToTable("RentalContract");

                entity.Property(e => e.ContractId).HasColumnName("ContractID");

                entity.Property(e => e.CarId).HasColumnName("CarID");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.Destination).HasMaxLength(50);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.PaymentMethod).HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Car)
                    .WithMany(p => p.RentalContracts)
                    .HasForeignKey(d => d.CarId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RentalCon__CarID__59FA5E80");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.RentalContracts)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RentalCon__Custo__5AEE82B9");
            });

            modelBuilder.Entity<RentalHistory>(entity =>
            {
                entity.HasKey(e => e.HistoryId)
                    .HasName("PK__RentalHi__4D7B4ADD9D203CC3");

                entity.ToTable("RentalHistory");

                entity.Property(e => e.HistoryId).HasColumnName("HistoryID");

                entity.Property(e => e.CarId).HasColumnName("CarID");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.Note).HasMaxLength(255);

                entity.Property(e => e.RentalDate).HasColumnType("date");

                entity.Property(e => e.ReturnDate).HasColumnType("date");

                entity.HasOne(d => d.Car)
                    .WithMany(p => p.RentalHistories)
                    .HasForeignKey(d => d.CarId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RentalHis__CarID__5DCAEF64");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.RentalHistories)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RentalHis__Custo__5EBF139D");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("Transaction");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Action).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.IssueDate).HasColumnType("datetime");

                entity.Property(e => e.Note).HasMaxLength(255);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transacti__Custo__5FB337D6");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

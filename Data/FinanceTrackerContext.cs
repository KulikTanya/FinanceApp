using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class FinanceTrackerContext : DbContext
    {
        public FinanceTrackerContext(DbContextOptions<FinanceTrackerContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<OperationHistory> OperationsHistory { get; set; }
        public DbSet<OperationCategory> OperationsCategories { get; set; }
        public DbSet<OperationType> OperationsType { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Users
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users", "dbo");
                entity.HasKey(e => e.Id).HasName("PK_Users");

                entity.Property(e => e.Id)
                    .HasColumnName("Id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Login)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("Login");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("Password");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("Name");

                entity.HasIndex(u => u.Login)
                    .IsUnique()
                    .HasDatabaseName("IX_Users_Login");
            });

            // Accounts
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Accounts", "dbo");
                entity.HasKey(e => e.Id).HasName("PK_Accounts");

                entity.Property(e => e.Id)
                    .HasColumnName("Id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.UserId)
                    .HasColumnName("UserId");

                entity.Property(e => e.AccountNumber)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("AccountNumber");

                entity.HasOne(a => a.User)
                    .WithMany(u => u.Accounts)
                    .HasForeignKey(a => a.UserId)
                    .HasConstraintName("FK_Accounts_Users");
            });

            // OperationsCategories
            modelBuilder.Entity<OperationCategory>(entity =>
            {
                entity.ToTable("OperationsCategories", "dbo");
                entity.HasKey(e => e.Id).HasName("PK_OperationsCategories");

                entity.Property(e => e.Id)
                    .HasColumnName("Id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("CategoryName");
            });

            // OperationsType
            modelBuilder.Entity<OperationType>(entity =>
            {
                entity.ToTable("OperationsType", "dbo");
                entity.HasKey(e => e.Id).HasName("PK_OperationsType");

                entity.Property(e => e.Id)
                    .HasColumnName("Id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.OperationName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("OperationName");
            });

            // OperationsHistory
            modelBuilder.Entity<OperationHistory>(entity =>
            {
                entity.ToTable("OperationsHistory", "dbo");
                entity.HasKey(e => e.Id).HasName("PK_OperationsHistory");

                entity.Property(e => e.Id)
                    .HasColumnName("Id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Date)
                    .IsRequired()
                    .HasColumnName("Date");

                entity.Property(e => e.AccountId)
                    .IsRequired()
                    .HasColumnName("AccountId");

                entity.Property(e => e.OperationCategoryId)
                    .HasColumnName("OperationCategoryId");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired()
                    .HasColumnName("Amount");

                entity.Property(e => e.OperationTypeId)
                    .HasColumnName("OperationTypeId");


                entity.HasOne(oh => oh.OperationCategory)
                    .WithMany()
                    .HasForeignKey(oh => oh.OperationCategoryId)
                    .HasConstraintName("FK_OperationsHistory_OperationsCategories");

                entity.HasOne(oh => oh.Account)
                    .WithMany() 
                    .HasForeignKey(oh => oh.AccountId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_OperationsHistory_Accounts");

                entity.HasOne(oh => oh.OperationType)
                    .WithMany()
                    .HasForeignKey(oh => oh.OperationTypeId)
                    .HasConstraintName("FK_OperationsHistory_OperationsType");
            });
        }
    }
}
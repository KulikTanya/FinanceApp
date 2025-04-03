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
        public DbSet<Role> Roles { get; set; }
        public DbSet<Privilege> Privileges { get; set; }
        public DbSet<RolePrivilege> RolePrivileges { get; set; }

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

                entity.Property(e => e.RoleId)
                    .HasColumnName("RoleId")
                    .HasDefaultValue(2); 

                entity.HasIndex(u => u.Login)
                    .IsUnique()
                    .HasDatabaseName("IX_Users_Login");

                entity.HasOne(u => u.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RoleId)
                    .HasConstraintName("FK_Users_Roles");
            });

            // Roles
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles", "dbo");
                entity.HasKey(e => e.Id).HasName("PK_Roles");

                entity.Property(e => e.Id)
                    .HasColumnName("Id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("RoleName");

                entity.HasIndex(r => r.RoleName)
                    .IsUnique()
                    .HasDatabaseName("IX_Roles_RoleName");
            });

            // Privileges
            modelBuilder.Entity<Privilege>(entity =>
            {
                entity.ToTable("Privileges", "dbo");
                entity.HasKey(e => e.Id).HasName("PK_Privileges");

                entity.Property(e => e.Id)
                    .HasColumnName("Id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.PrivilegeName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("PrivilegeName");

                entity.HasIndex(p => p.PrivilegeName)
                    .IsUnique()
                    .HasDatabaseName("IX_Privileges_PrivilegeName");
            });

            // RolePrivileges 
            modelBuilder.Entity<RolePrivilege>(entity =>
            {
                entity.ToTable("RolePrivileges", "dbo");
                entity.HasKey(e => new { e.RoleId, e.PrivilegeId })
                    .HasName("PK_RolePrivileges");

                entity.Property(e => e.RoleId)
                    .HasColumnName("RoleId");

                entity.Property(e => e.PrivilegeId)
                    .HasColumnName("PrivilegeId");

                entity.HasOne(rp => rp.Role)
                    .WithMany(r => r.RolePrivileges)
                    .HasForeignKey(rp => rp.RoleId)
                    .HasConstraintName("FK_RolePrivileges_Roles")
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rp => rp.Privilege)
                    .WithMany(p => p.RolePrivileges)
                    .HasForeignKey(rp => rp.PrivilegeId)
                    .HasConstraintName("FK_RolePrivileges_Privileges")
                    .OnDelete(DeleteBehavior.Cascade);
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
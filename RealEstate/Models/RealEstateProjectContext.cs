using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using RealEstate.DTO_s.UsersDTO_s;

namespace RealEstate.Models
{
    public partial class RealEstateProjectContext : IdentityDbContext
    {
        public RealEstateProjectContext()
        {
        }

        public RealEstateProjectContext(DbContextOptions<RealEstateProjectContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Aspnetrole> Aspnetroles { get; set; }
        public virtual DbSet<Aspnetroleclaim> Aspnetroleclaims { get; set; }
        public virtual DbSet<Aspnetuser> Aspnetusers { get; set; }
        public virtual DbSet<Aspnetuserclaim> Aspnetuserclaims { get; set; }
        public virtual DbSet<Aspnetuserlogin> Aspnetuserlogins { get; set; }
        public virtual DbSet<Aspnetusertoken> Aspnetusertokens { get; set; }
        public virtual DbSet<Buycontract> Buycontracts { get; set; }
        public virtual DbSet<Buyer> Buyers { get; set; }
        public virtual DbSet<Efmigrationshistory> Efmigrationshistories { get; set; }
        public virtual DbSet<Estate> Estates { get; set; }
        public virtual DbSet<Guarantor> Guarantors { get; set; }
        public virtual DbSet<Mortgage> Mortgages { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Renter> Renters { get; set; }
        public virtual DbSet<Rentingcontract> Rentingcontracts { get; set; }
        public virtual DbSet<NewIdentityUser> UsersNewColumns { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.UseCollation("latin1_swedish_ci")
                .HasCharSet("latin1");

            modelBuilder.Entity<Aspnetrole>(entity =>
            {
                entity.ToTable("aspnetroles");

                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                    .IsUnique();

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<Aspnetroleclaim>(entity =>
            {
                entity.ToTable("aspnetroleclaims");

                entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Aspnetroleclaims)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_AspNetRoleClaims_AspNetRoles_RoleId");
            });

            modelBuilder.Entity<Aspnetuser>(entity =>
            {
                entity.ToTable("aspnetusers");

                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                    .IsUnique();

                entity.Property(e => e.AccessFailedCount).HasColumnType("int(11)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.LockoutEnd).HasMaxLength(6);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.HasMany(d => d.Roles)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "Aspnetuserrole",
                        l => l.HasOne<Aspnetrole>().WithMany().HasForeignKey("RoleId").HasConstraintName("FK_AspNetUserRoles_AspNetRoles_RoleId"),
                        r => r.HasOne<Aspnetuser>().WithMany().HasForeignKey("UserId").HasConstraintName("FK_AspNetUserRoles_AspNetUsers_UserId"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId").HasName("PRIMARY").HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                            j.ToTable("aspnetuserroles");

                            j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                        });
            });

            modelBuilder.Entity<Aspnetuserclaim>(entity =>
            {
                entity.ToTable("aspnetuserclaims");

                entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Aspnetuserclaims)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_AspNetUserClaims_AspNetUsers_UserId");
            });

            modelBuilder.Entity<Aspnetuserlogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("aspnetuserlogins");

                entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Aspnetuserlogins)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_AspNetUserLogins_AspNetUsers_UserId");
            });

            modelBuilder.Entity<Aspnetusertoken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

                entity.ToTable("aspnetusertokens");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Aspnetusertokens)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_AspNetUserTokens_AspNetUsers_UserId");
            });

            modelBuilder.Entity<Buycontract>(entity =>
            {
                entity.HasKey(e => e.IdBuyContract)
                    .HasName("PRIMARY");

                entity.ToTable("buycontract");

                entity.HasIndex(e => e.IdBuyer, "id_Buyer");

                entity.HasIndex(e => e.IdEst, "id_Est")
                    .IsUnique();

                entity.Property(e => e.IdBuyContract)
                    .HasColumnType("int(11)")
                    .HasColumnName("Id_BuyContract");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasColumnName("_Date");

                entity.Property(e => e.IdBuyer)
                    .HasColumnType("int(11)")
                    .HasColumnName("Id_Buyer");

                entity.Property(e => e.IdEst)
                    .HasColumnType("int(11)")
                    .HasColumnName("Id_Est");

                entity.Property(e => e.SalePrice).HasColumnType("int(11)");

                entity.HasOne(d => d.IdBuyerNavigation)
                    .WithMany(p => p.Buycontracts)
                    .HasForeignKey(d => d.IdBuyer)
                    .HasConstraintName("buycontract_ibfk_2");

                entity.HasOne(d => d.IdEstNavigation)
                    .WithOne(p => p.Buycontract)
                    .HasForeignKey<Buycontract>(d => d.IdEst)
                    .HasConstraintName("buycontract_ibfk_1");
            });

            modelBuilder.Entity<Buyer>(entity =>
            {
                entity.HasKey(e => e.IdBuyer)
                    .HasName("PRIMARY");

                entity.ToTable("buyer");

                entity.HasIndex(e => e.Dni, "DNI");

                entity.HasIndex(e => e.IdEstate, "Estate")
                    .IsUnique();

                entity.Property(e => e.IdBuyer)
                    .HasColumnType("int(11)")
                    .HasColumnName("Id_Buyer");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("_Address");

                entity.Property(e => e.Age).HasColumnType("int(11)");

                entity.Property(e => e.CellPhoneNumber).HasColumnType("bigint(20)");

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Dni)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("DNI");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.FirsName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.FirstSurName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.IdEstate)
                    .HasColumnType("int(11)")
                    .HasColumnName("Id_Estate");

                entity.Property(e => e.SecondName).HasMaxLength(250);

                entity.Property(e => e.SecondSurName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.IdEstateNavigation)
                    .WithOne(p => p.Buyer)
                    .HasForeignKey<Buyer>(d => d.IdEstate)
                    .HasConstraintName("buyer_ibfk_1");
            });

            modelBuilder.Entity<Efmigrationshistory>(entity =>
            {
                entity.HasKey(e => e.MigrationId)
                    .HasName("PRIMARY");

                entity.ToTable("__efmigrationshistory");

                entity.HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");

                entity.Property(e => e.MigrationId).HasMaxLength(150);

                entity.Property(e => e.ProductVersion)
                    .IsRequired()
                    .HasMaxLength(32);
            });

            modelBuilder.Entity<Estate>(entity =>
            {
                entity.HasKey(e => e.IdEstate)
                    .HasName("PRIMARY");

                entity.ToTable("estate");

                entity.HasIndex(e => e.Alias, "Alias");

                entity.HasIndex(e => e.IdUser, "Id_User_idx");

                entity.Property(e => e.IdEstate)
                    .HasColumnType("int(11)")
                    .HasColumnName("Id_Estate");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("_Address");

                entity.Property(e => e.Alias).HasMaxLength(250);

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.KmsGround).HasColumnType("int(11)");

                entity.Property(e => e.Rented).HasDefaultValueSql("'0'");

                entity.Property(e => e.Rooms).HasColumnType("int(11)");

                entity.Property(e => e.Sold).HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.Estates)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Id_User");
            });

            modelBuilder.Entity<Guarantor>(entity =>
            {
                entity.HasKey(e => e.IdGuarantor)
                    .HasName("PRIMARY");

                entity.ToTable("guarantor");

                entity.HasIndex(e => e.Dni, "DNI");

                entity.HasIndex(e => e.IdRenter, "Renter");

                entity.Property(e => e.IdGuarantor)
                    .HasColumnType("int(11)")
                    .HasColumnName("Id_Guarantor");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("_Address");

                entity.Property(e => e.Age).HasColumnType("int(11)");

                entity.Property(e => e.CellPhoneNumber).HasColumnType("bigint(20)");

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Dni)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("DNI");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.FirsName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.FirstSurName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.IdRenter)
                    .HasColumnType("int(11)")
                    .HasColumnName("Id_Renter");

                entity.Property(e => e.SecondName).HasMaxLength(250);

                entity.Property(e => e.SecondSurName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.IdRenterNavigation)
                    .WithMany(p => p.Guarantors)
                    .HasForeignKey(d => d.IdRenter)
                    .HasConstraintName("guarantor_ibfk_1");
            });

            modelBuilder.Entity<Mortgage>(entity =>
            {
                entity.HasKey(e => e.IdMortgage)
                    .HasName("PRIMARY");

                entity.ToTable("mortgage");

                entity.HasIndex(e => e.IdEstate, "Estate")
                    .IsUnique();

                entity.HasIndex(e => e.IdUser, "iduser_idx");

                entity.Property(e => e.IdMortgage)
                    .HasColumnType("int(11)")
                    .HasColumnName("Id_Mortgage");

                entity.Property(e => e.FeeValue).HasColumnType("int(11)");

                entity.Property(e => e.FeesNumber).HasColumnType("int(11)");

                entity.Property(e => e.IdEstate)
                    .HasColumnType("int(11)")
                    .HasColumnName("Id_Estate");

                entity.Property(e => e.IdUser)
                    .IsRequired()
                    .HasColumnName("Id_User");

                entity.Property(e => e.TotalValue).HasColumnType("int(11)");

                entity.HasOne(d => d.IdEstateNavigation)
                    .WithOne(p => p.Mortgage)
                    .HasForeignKey<Mortgage>(d => d.IdEstate)
                    .HasConstraintName("mortgage_ibfk_1");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.Mortgages)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("iduser");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.IdPayments)
                    .HasName("PRIMARY");

                entity.ToTable("payments");

                entity.HasIndex(e => e.IdMortgage, "Id_Mortage");

                entity.Property(e => e.IdPayments)
                    .HasColumnType("int(11)")
                    .HasColumnName("Id_Payments");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasColumnName("_Date");

                entity.Property(e => e.IdMortgage)
                    .HasColumnType("int(11)")
                    .HasColumnName("Id_Mortgage");

                entity.Property(e => e.Value)
                    .HasColumnType("int(11)")
                    .HasColumnName("_Value");

                entity.HasOne(d => d.IdMortgageNavigation)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.IdMortgage)
                    .HasConstraintName("payments_ibfk_1");
            });

            modelBuilder.Entity<Renter>(entity =>
            {
                entity.HasKey(e => e.IdRenter)
                    .HasName("PRIMARY");

                entity.ToTable("renter");

                entity.HasIndex(e => e.Dni, "DNI");

                entity.HasIndex(e => e.IdEstate, "Id_Est");

                entity.Property(e => e.IdRenter)
                    .HasColumnType("int(11)")
                    .HasColumnName("Id_Renter");

                entity.Property(e => e.Active).HasDefaultValueSql("'0'");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("_Address");

                entity.Property(e => e.Age).HasColumnType("int(11)");

                entity.Property(e => e.CellPhoneNumber).HasColumnType("bigint(20)");

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Dni)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("DNI");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.FirsName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.FirstSurName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.IdEstate)
                    .HasColumnType("int(11)")
                    .HasColumnName("Id_Estate");

                entity.Property(e => e.SecondName).HasMaxLength(250);

                entity.Property(e => e.SecondSurName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.IdEstateNavigation)
                    .WithMany(p => p.Renters)
                    .HasForeignKey(d => d.IdEstate)
                    .HasConstraintName("renter_ibfk_1");
            });

            modelBuilder.Entity<Rentingcontract>(entity =>
            {
                entity.HasKey(e => e.IdRentingContract)
                    .HasName("PRIMARY");

                entity.ToTable("rentingcontract");

                entity.HasIndex(e => e.IdEst, "id_Est");

                entity.HasIndex(e => e.IdRenter, "id_Renter");

                entity.Property(e => e.IdRentingContract)
                    .HasColumnType("int(11)")
                    .HasColumnName("Id_RentingContract");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasColumnName("_Date");

                entity.Property(e => e.IdEst)
                    .HasColumnType("int(11)")
                    .HasColumnName("Id_Est");

                entity.Property(e => e.IdRenter)
                    .HasColumnType("int(11)")
                    .HasColumnName("Id_Renter");

                entity.Property(e => e.Value)
                    .HasColumnType("int(11)")
                    .HasColumnName("_Value");

                entity.HasOne(d => d.IdEstNavigation)
                    .WithMany(p => p.Rentingcontracts)
                    .HasForeignKey(d => d.IdEst)
                    .HasConstraintName("rentingcontract_ibfk_1");

                entity.HasOne(d => d.IdRenterNavigation)
                    .WithMany(p => p.Rentingcontracts)
                    .HasForeignKey(d => d.IdRenter)
                    .HasConstraintName("rentingcontract_ibfk_2");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

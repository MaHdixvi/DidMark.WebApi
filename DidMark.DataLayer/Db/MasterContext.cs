using System;
using System.Collections.Generic;
using DidMark.DataLayer.Entities.Access;
using DidMark.DataLayer.Entities.Account;
using DidMark.DataLayer.Entities.Offers;
using DidMark.DataLayer.Entities.Orders;
using DidMark.DataLayer.Entities.Product;
using DidMark.DataLayer.Entities.Site;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DidMark.DataLayer.Db;

public partial class MasterContext : DbContext
{
    private MasterContext()
    {
    }

    public MasterContext(DbContextOptions<MasterContext> options)
        : base(options)
    {
    }

    #region Db Sets

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Slider> Sliders { get; set; }
    public DbSet<Contact> Contact { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategories> ProductCategories { get; set; }
    public DbSet<ProductGalleries> ProductGalleries { get; set; }
    public DbSet<ProductSelectedCategories> ProductSelectedCategories { get; set; }
    public DbSet<ProductVisit> ProductVisit { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<NewsletterSubscriber> NewsletterSubscribers { get; set; }
    public DbSet<SpecialOffer> SpecialOffers { get; set; }
    public DbSet<SpecialOfferProduct> SpecialOfferProducts { get; set; }

    public DbContextOptions<MasterContext> Options { get; }

    #endregion

    #region Fluent API Configuration

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // جلوگیری از حذف آبشاری
        var cascades = modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (var fk in cascades)
        {
            fk.DeleteBehavior = DeleteBehavior.Restrict;
        }

        // تعریف داده‌های اولیه
        var adminRoleId = 1;
        var userRoleId = 2;
        var adminUserId = 1;

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = adminRoleId,
                RoleName="Owner",
                RoleTitle = "Admin",
                CreateDate = DateTime.Now
            },
            new Role
            {
                Id = userRoleId,
                RoleName="Owner",
                RoleTitle = "User",
                CreateDate = DateTime.Now
            }
        );

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = adminUserId,
                Email = "admin@didmark.com",
                Password = "90-6B-98-8B-FC-DB-F7-30-F4-C6-A8-15-76-C2-88-54", // هش‌شده!
                //"46-CC-1A-A2-C2-86-07-50-4E-B8-CA-F1-62-CF-23-C4"
                FirstName = "Admin",
                LastName = "User",
                PhoneActiveCode ="1234",
                //NationalCode ="4271706248",
                EmailActiveCode =Guid.NewGuid().ToString(),
                IsActivated = true,
                CreateDate = DateTime.Now,
                PhoneNumber="09100295341",
                Username="Admin"
            }
        );

        modelBuilder.Entity<UserRole>().HasData(
            new UserRole
            {
                Id = 1,
                UserId = adminUserId,
                RoleId = adminRoleId,
                CreateDate = DateTime.Now
            },
            new UserRole
            {
                Id = 2,
                UserId = adminUserId,
                RoleId = userRoleId,
                CreateDate = DateTime.Now
            }
        );

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    #endregion

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        //=> optionsBuilder.UseSqlServer("Data Source=VICTUS-15\\SQLEXPRESS;Initial Catalog=ALAPARDB;Integrated Security=True;Trust Server Certificate=True");
        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=ALAPARDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

}
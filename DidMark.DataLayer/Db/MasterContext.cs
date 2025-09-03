using System;
using System.Collections.Generic;
using DidMark.DataLayer.Entities.Access;
using DidMark.DataLayer.Entities.Account;
using DidMark.DataLayer.Entities.Orders;
using DidMark.DataLayer.Entities.Product;
using DidMark.DataLayer.Entities.Site;
using Microsoft.EntityFrameworkCore;

namespace DidMark.DataLayer.Db;

public partial class MasterContext : DbContext
{
    public MasterContext()
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

    public DbSet<Product> ProductCategories { get; set; }

    public DbSet<Product> ProductGalleries { get; set; }

    public DbSet<Product> ProductSelectedCategories { get; set; }

    public DbSet<Product> ProductVisit { get; set; }

    public DbContextOptions<MasterContext> Options { get; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }

    #endregion




    #region disable cascading delete in database

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var cascades = modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (var fk in cascades)
        {
            fk.DeleteBehavior = DeleteBehavior.Restrict;
        }

        base.OnModelCreating(modelBuilder);
        OnModelCreatingPartial(modelBuilder);
    }

    #endregion


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-0K81KJT\\ALAPARDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

  

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GitCRUDAPI.Models;

public partial class PathrolContext : DbContext
{
    public PathrolContext()
    {
    }

    public PathrolContext(DbContextOptions<PathrolContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Login> Logins { get; set; }

   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07FBF99072");

            entity.ToTable("Employee");

            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(100);
        });

        modelBuilder.Entity<Login>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Login__3214EC079F88C666");

            entity.ToTable("Login");

            entity.HasIndex(e => e.Username, "UQ__Login__536C85E40DEB1CBF").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

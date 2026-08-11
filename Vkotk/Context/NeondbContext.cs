using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Vkotk.Models;

namespace Vkotk.Context;

public partial class NeondbContext : DbContext
{
    public NeondbContext()
    {
    }

    public NeondbContext(DbContextOptions<NeondbContext> options)
        : base(options)
    {
    }

    private static NeondbContext _context;
    
    public static NeondbContext GetContext()
    {
        if(_context == null)
            _context = new NeondbContext();
        return _context;
    }
    

    public virtual DbSet<Imageproduct> Imageproducts { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Productimage> Productimages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=ep-mute-surf-za0rncxc.c-2.eu-west-2.aws.neon.tech;Database=neondb;Username=neondb_owner;password=npg_A1lSRIuPYKD5");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Imageproduct>(entity =>
        {
            entity.HasKey(e => e.Idimage).HasName("imageproduct_pkey");

            entity.ToTable("imageproduct");

            entity.Property(e => e.Idimage).HasColumnName("idimage");
            entity.Property(e => e.Pathimage)
                .HasMaxLength(100)
                .HasColumnName("pathimage");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Idproduct).HasName("product_pkey");

            entity.ToTable("product");

            entity.Property(e => e.Idproduct).HasColumnName("idproduct");
            entity.Property(e => e.Codeproduct)
                .HasMaxLength(100)
                .HasColumnName("codeproduct");
            entity.Property(e => e.Commentproduct)
                .HasMaxLength(100)
                .HasColumnName("commentproduct");
            entity.Property(e => e.Countproduct).HasColumnName("countproduct");
            entity.Property(e => e.Datecheck).HasColumnName("datecheck");
            entity.Property(e => e.Nameproduct)
                .HasMaxLength(100)
                .HasColumnName("nameproduct");
        });

        modelBuilder.Entity<Productimage>(entity =>
        {
            entity.HasKey(e => e.Idproductimage).HasName("productimage_pkey");

            entity.ToTable("productimage");

            entity.Property(e => e.Idproductimage).HasColumnName("idproductimage");
            entity.Property(e => e.Idimage).HasColumnName("idimage");
            entity.Property(e => e.Idproduct).HasColumnName("idproduct");

            entity.HasOne(d => d.IdimageNavigation).WithMany(p => p.Productimages)
                .HasForeignKey(d => d.Idimage)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("productimage_idimage_fkey");

            entity.HasOne(d => d.IdproductNavigation).WithMany(p => p.Productimages)
                .HasForeignKey(d => d.Idproduct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("productimage_idproduct_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

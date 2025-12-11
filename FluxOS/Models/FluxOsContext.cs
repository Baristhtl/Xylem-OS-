using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FluxOS.Models;

public partial class FluxOsContext : DbContext
{
    public FluxOsContext()
    {
    }

    public FluxOsContext(DbContextOptions<FluxOsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<Reply> Replies { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=FluxOS;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A2BB254EF32");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.NewsId).HasName("PK__News__954EBDF30F702B0B");

            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Summary).HasMaxLength(500);
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        modelBuilder.Entity<Reply>(entity =>
        {
            entity.HasKey(e => e.ReplyId).HasName("PK__Replies__C25E46296E5EC555");

            entity.Property(e => e.ReplyId).HasColumnName("ReplyID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TopicId).HasColumnName("TopicID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Topic).WithMany(p => p.Replies)
                .HasForeignKey(d => d.TopicId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Replies__TopicID__45F365D3");

            entity.HasOne(d => d.User).WithMany(p => p.Replies)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Replies__UserID__46E78A0C");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.TopicId).HasName("PK__Topics__022E0F7DB22B330E");

            entity.Property(e => e.TopicId).HasColumnName("TopicID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsSolved).HasDefaultValue(false);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.ViewCount).HasDefaultValue(0);

            entity.HasOne(d => d.Category).WithMany(p => p.Topics)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Topics__Category__4222D4EF");

            entity.HasOne(d => d.User).WithMany(p => p.Topics)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Topics__UserID__412EB0B6");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACEBEEA72E");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.AvatarColor)
                .HasMaxLength(20)
                .HasDefaultValue("#6c63ff");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValue("User");
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PRN231_API.Models
{
    public partial class PRN231_DBContext : DbContext
    {
        public PRN231_DBContext()
        {
        }

        public PRN231_DBContext(DbContextOptions<PRN231_DBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Chapter> Chapters { get; set; }
        public virtual DbSet<Comic> Comics { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", true, true)
               .Build();
            optionsBuilder.UseSqlServer(config.GetConnectionString("Context"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CategoryId).HasColumnName("categoryId");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("categoryName");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("description");
            });

            modelBuilder.Entity<Chapter>(entity =>
            {
                entity.Property(e => e.ChapterId).HasColumnName("chapterId");

                entity.Property(e => e.ChapterNumber).HasColumnName("chapterNumber");

                entity.Property(e => e.ComicId).HasColumnName("comicId");

                entity.Property(e => e.PublicDate)
                    .HasColumnType("datetime")
                    .HasColumnName("publicDate");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.Comic)
                    .WithMany(p => p.Chapters)
                    .HasForeignKey(d => d.ComicId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Chapters_Comics");
            });

            modelBuilder.Entity<Comic>(entity =>
            {
                entity.Property(e => e.ComicId).HasColumnName("comicId");

                entity.Property(e => e.ComicName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("comicName");

                entity.Property(e => e.Image).HasColumnName("image");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.PublicDate)
                    .HasColumnType("datetime")
                    .HasColumnName("publicDate");

                entity.Property(e => e.Summary).HasColumnName("summary");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Comics)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Comics_Users");

                entity.HasMany(d => d.Categories)
                    .WithMany(p => p.Comics)
                    .UsingEntity<Dictionary<string, object>>(
                        "ComicCategory",
                        l => l.HasOne<Category>().WithMany().HasForeignKey("CategoryId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Comic_Category_Categories"),
                        r => r.HasOne<Comic>().WithMany().HasForeignKey("ComicId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Comic_Category_Comics"),
                        j =>
                        {
                            j.HasKey("ComicId", "CategoryId");

                            j.ToTable("Comic_Category");

                            j.IndexerProperty<int>("ComicId").HasColumnName("comicId");

                            j.IndexerProperty<int>("CategoryId").HasColumnName("categoryId");
                        });
            });

            modelBuilder.Entity<Page>(entity =>
            {
                entity.Property(e => e.PageId).HasColumnName("pageId");

                entity.Property(e => e.ChapterId).HasColumnName("chapterId");

                entity.Property(e => e.Image)
                    .IsRequired()
                    .HasColumnName("image");

                entity.Property(e => e.PageNumber).HasColumnName("pageNumber");

                entity.HasOne(d => d.Chapter)
                    .WithMany(p => p.Pages)
                    .HasForeignKey(d => d.ChapterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Pages_Chapters");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("role");

                entity.HasMany(d => d.ComicsNavigation)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "Favorite",
                        l => l.HasOne<Comic>().WithMany().HasForeignKey("ComicId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Favorites_Comics"),
                        r => r.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Favorites_Users"),
                        j =>
                        {
                            j.HasKey("UserId", "ComicId");

                            j.ToTable("Favorites");

                            j.IndexerProperty<int>("UserId").HasColumnName("userId");

                            j.IndexerProperty<int>("ComicId").HasColumnName("comicId");
                        });
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OnlineCourse.Models;

public partial class DataContext : IdentityDbContext<AppUserModel>
{
    public DataContext()
    {
    }

    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdminMenu> AdminMenus { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseCategory> CourseCategories { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Result> Results { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<Progresses> Progresses { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    public virtual DbSet<Chapter> Chapters { get; set; }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<BlogCategory> BlogCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(l => new { l.LoginProvider, l.ProviderKey });

        modelBuilder.Entity<AdminMenu>(entity =>
        {
            entity.ToTable("AdminMenu");

            entity.Property(e => e.AdminMenuId).HasColumnName("AdminMenuID");
            entity.Property(e => e.ActionName).HasMaxLength(50);
            entity.Property(e => e.AreaName).HasMaxLength(50);
            entity.Property(e => e.ControllerName).HasMaxLength(50);
            entity.Property(e => e.Icon).HasMaxLength(50);
            entity.Property(e => e.IdName).HasMaxLength(50);
            entity.Property(e => e.ItemName).HasMaxLength(50);
            entity.Property(e => e.ItemTarget).HasMaxLength(50);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Details).HasColumnType("ntext");
            entity.Property(e => e.Image).HasMaxLength(250);
            entity.Property(e => e.MetaTitle)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(250);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.PromotionPrice).HasColumnType("decimal(18, 0)");
        });

        modelBuilder.Entity<Chapter>(entity =>
        {
            entity.Property(e => e.ChapterId).HasColumnName("ChapterId");
            entity.Property(e => e.CourseId).HasColumnName("CourseId");
            entity.Property(e => e.Name).HasMaxLength(250);
            entity.Property(e => e.Order).HasColumnType("int");
            entity.Property(e => e.Status).HasColumnType("bit");

            entity.HasOne(d => d.Course).WithMany(p => p.Chapters)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_Chapters_Courses");
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.Property(e => e.LessonId).HasColumnName("LessonId");
            entity.Property(e => e.ChapterId).HasColumnName("ChapterId");
            entity.Property(e => e.Name).HasMaxLength(250);
            entity.Property(e => e.Order).HasColumnType("int");
            entity.Property(e => e.Details).HasColumnType("ntext");
            entity.Property(e => e.ContentType).HasMaxLength(250);
            entity.Property(e => e.VideoUrl).HasColumnType("ntext");
            entity.Property(e => e.FilePath).HasMaxLength(250);
            entity.Property(e => e.Status).HasColumnType("bit");

            entity.HasOne(d => d.Chapter).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.ChapterId)
                .HasConstraintName("FK_Lessons_Courses");
        });

        modelBuilder.Entity<CourseCategory>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(250);
            entity.Property(e => e.Description).HasColumnType("ntext");

        });

        modelBuilder.Entity<BlogCategory>(entity =>
        {
            entity.Property(e => e.BlogCategoryId).HasColumnName("BlogCategoryId");
            entity.Property(e => e.Name).HasMaxLength(250);
            entity.Property(e => e.Slug).HasMaxLength(250);
        });

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.Property(e => e.BlogId).HasColumnName("BlogId");
            entity.Property(e => e.BlogCategoryId).HasColumnName("BlogCategoryId");
            entity.Property(e => e.Title).HasMaxLength(250);
            entity.Property(e => e.Content).HasColumnType("ntext");
            entity.Property(e => e.Image).HasMaxLength(250);
            entity.Property(e => e.Author).HasMaxLength(250);
            entity.Property(e => e.Status).HasColumnType("bit");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.BlogCategory).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.BlogCategoryId)
                .HasConstraintName("FK_Blogs_BlogCategories");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Exam");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AnswersList)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.MetaTitle)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(250);
            entity.Property(e => e.QuestionsList)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.ScoreList).HasMaxLength(250);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.HasOne(d => d.Course).WithMany(p => p.Exams)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_Exams_Courses");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Course).WithMany(p => p.Questions)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_Questions_Courses");
        });

        modelBuilder.Entity<Result>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.ExamId });
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.ExamId).HasColumnName("ExamID");
            entity.Property(e => e.FinishTimeEssay).HasMaxLength(50);
            entity.Property(e => e.FinishTimeQuiz).HasMaxLength(50);
            entity.Property(e => e.ResultQuiz).HasMaxLength(250);
            entity.Property(e => e.Score)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StartDateEssay).HasColumnType("datetime");
            entity.Property(e => e.StartDateQuiz).HasMaxLength(250);
            entity.Property(e => e.StartTimeEssay).HasMaxLength(50);
            entity.Property(e => e.StartTimeQuiz).HasMaxLength(50);

            entity.HasOne(d => d.Exam).WithMany(p => p.Results)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Results_Exams");
        });

        // modelBuilder.Entity<Progresses>()
        //     .HasOne(p => p.Lesson)
        //     .WithMany(l => l.Progresses)
        //     .HasForeignKey(p => p.LessonId)
        //     .OnDelete(DeleteBehavior.Cascade);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

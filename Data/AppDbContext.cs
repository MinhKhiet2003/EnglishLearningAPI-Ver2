using EnglishLearningAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearningAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSet cho từng bảng (entity)
        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Vocabulary> Vocabularies { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<CourseProgress> CourseProgresses { get; set; }
        public DbSet<TopicProgress> TopicProgresses { get; set; }
        public DbSet<UserProgress> UserProgresses { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<FormSubmission> FormSubmissions { get; set; }

        // Cấu hình Fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Bảng User
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id); // Khóa chính
            modelBuilder.Entity<User>()
                .HasMany(u => u.CourseProgresses)
                .WithOne(cp => cp.User)
                .HasForeignKey(cp => cp.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa User sẽ xóa liên quan
            modelBuilder.Entity<User>()
                .HasMany(u => u.TopicProgresses)
                .WithOne(tp => tp.User)
                .HasForeignKey(tp => tp.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>()
                .HasMany(u => u.UserProgresses)
                .WithOne(up => up.User)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>()
                .HasMany(u => u.Payments)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);



            // Bảng Course
            modelBuilder.Entity<Course>()
                .HasKey(c => c.Id);
            modelBuilder.Entity<Course>()
                .HasMany(c => c.Topics)
                .WithOne(t => t.Course)
                .HasForeignKey(t => t.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Course>()
                .HasMany(c => c.CourseProgresses)
                .WithOne(cp => cp.Course)
                .HasForeignKey(cp => cp.CourseId)
                .OnDelete(DeleteBehavior.Cascade);


            // Bảng Topic
            modelBuilder.Entity<Topic>()
                .HasKey(t => t.Id);
            modelBuilder.Entity<Topic>()
                .HasMany(t => t.Vocabularies)
                .WithOne(v => v.Topic)
                .HasForeignKey(v => v.TopicId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Topic>()
                .HasMany(t => t.TopicProgresses)
                .WithOne(tp => tp.Topic)
                .HasForeignKey(tp => tp.TopicId)
                .OnDelete(DeleteBehavior.Cascade);


            // Bảng Vocabulary
            modelBuilder.Entity<Vocabulary>()
                .HasKey(v => v.Id);
            modelBuilder.Entity<Vocabulary>()
                .HasMany(v => v.UserProgresses)
                .WithOne(up => up.Vocabulary)
                .HasForeignKey(up => up.VocabularyId)
                .OnDelete(DeleteBehavior.Cascade);


            // Bảng CourseProgress
            modelBuilder.Entity<CourseProgress>()
                .HasKey(cp => cp.Id);

            modelBuilder.Entity<CourseProgress>()
            .HasOne(cp => cp.User)
            .WithMany(u => u.CourseProgresses)
            .HasForeignKey(cp => cp.Id)
            .HasPrincipalKey(u => u.Id); // Ensure this matches the type of Id in User

            // Bảng TopicProgress
            modelBuilder.Entity<TopicProgress>()
                .HasKey(tp => tp.Id);
            modelBuilder.Entity<TopicProgress>()
            .HasOne(cp => cp.User)
            .WithMany(u => u.TopicProgresses)
            .HasForeignKey(cp => cp.Id)
            .HasPrincipalKey(u => u.Id);
            // Bảng UserProgress
            modelBuilder.Entity<UserProgress>()
                .HasKey(up => up.Id);
            modelBuilder.Entity<UserProgress>()
                .HasOne(cp => cp.User)
                .WithMany(u => u.UserProgresses)
                .HasForeignKey(cp => cp.Id)
                .HasPrincipalKey(u => u.Id);
            // Bảng Payment
            modelBuilder.Entity<Payment>()
                .HasKey(p => p.Id);
            modelBuilder.Entity<Payment>()
                .HasOne(fs => fs.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(fs => fs.UserId)
                .HasPrincipalKey(u => u.Id)
                .OnDelete(DeleteBehavior.Cascade);
            // Bảng FormSubmission
            modelBuilder.Entity<FormSubmission>()
                .HasKey(fs => fs.Id);
            modelBuilder.Entity<FormSubmission>()
                .HasOne(fs => fs.User)
                .WithMany(u => u.FormSubmissions)
                .HasForeignKey(fs => fs.UserId)
                .HasPrincipalKey(u => u.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Ràng buộc Unique và các cột không Null
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique(); // Email phải unique
            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired(); // Không cho phép null
            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .IsRequired(); // Không cho phép null
        }
    }
}

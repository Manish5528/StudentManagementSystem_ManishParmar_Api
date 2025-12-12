using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Domain.Entities;

namespace StudentManagementSystem.Infrastructure.Data;

   public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
    public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<StudentCourse> StudentCourses { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Student
            modelBuilder.Entity<Student>(b =>
            {
                b.ToTable("Students");
                b.HasKey(x => x.Id);

                b.Property(x => x.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                b.Property(x => x.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                b.Property(x => x.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(10);

                b.Property(x => x.EmailId)
                    .IsRequired()
                    .HasMaxLength(256);

                b.HasIndex(x => x.PhoneNumber).IsUnique();
                b.HasIndex(x => x.EmailId).IsUnique();

                b.HasCheckConstraint("CK_Students_PhoneDigitsOnly", "PhoneNumber NOT LIKE '%[^0-9]%' AND LEN(PhoneNumber) <= 10");
            });

            // ClassEntity
            modelBuilder.Entity<Course>(b =>
            {
                b.ToTable("Courses");
                b.HasKey(x => x.Id);

                b.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                b.Property(x => x.Description)
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<StudentCourse>(b =>
            {
                b.ToTable("StudentCourses");

                b.HasKey(sc => new { sc.StudentId, sc.CourseId });

                b.HasOne(sc => sc.Student)
                    .WithMany(s => s.StudentCourses)
                    .HasForeignKey(sc => sc.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(sc => sc.Course)
                    .WithMany(c => c.StudentCourses)
                    .HasForeignKey(sc => sc.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.Property(sc => sc.EnrolledOn)
                    .HasDefaultValueSql("getutcdate()");
            });
        }
    }

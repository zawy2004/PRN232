using Microsoft.EntityFrameworkCore;
using Q1.Models;

namespace Q1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseAssignment> CourseAssignments { get; set; }
        public DbSet<ClassSection> ClassSections { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Instructor>().ToTable("Instructors");
            modelBuilder.Entity<Course>().ToTable("Courses");
            modelBuilder.Entity<Student>().ToTable("Students");

            modelBuilder.Entity<ClassSection>(entity =>
            {
                entity.ToTable("ClassSections");
                entity.HasKey(e => e.SectionID);
                entity.HasOne(e => e.Course)
                      .WithMany(c => c.ClassSections)
                      .HasForeignKey(e => e.CourseID);
            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.ToTable("Enrollments");
                entity.HasKey(e => e.EnrollmentID);
                entity.HasOne(e => e.Student)
                      .WithMany(s => s.Enrollments)
                      .HasForeignKey(e => e.StudentID);
                entity.HasOne(e => e.Section)
                      .WithMany(s => s.Enrollments)
                      .HasForeignKey(e => e.SectionID);
            });

            modelBuilder.Entity<CourseAssignment>(entity =>
            {
                entity.ToTable("CourseAssignments");
                entity.HasKey(e => new { e.CourseID, e.InstructorID });
                entity.HasOne(e => e.Course)
                      .WithMany(c => c.CourseAssignments)
                      .HasForeignKey(e => e.CourseID);
                entity.HasOne(e => e.Instructor)
                      .WithMany(i => i.CourseAssignments)
                      .HasForeignKey(e => e.InstructorID);
            });
        }
    }
}

using FUCourseManagementSystem.BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace FUCourseManagementSystem.DataAccess;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<SystemAccount> SystemAccounts { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Lecturer> Lecturers { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<ClassSection> ClassSections { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Map to singular table names (matching the SQL script)
        modelBuilder.Entity<SystemAccount>().ToTable("SystemAccount");
        modelBuilder.Entity<Department>().ToTable("Department");
        modelBuilder.Entity<Course>().ToTable("Course");
        modelBuilder.Entity<Lecturer>().ToTable("Lecturer");
        modelBuilder.Entity<Student>().ToTable("Student");
        modelBuilder.Entity<ClassSection>().ToTable("ClassSection");
        modelBuilder.Entity<Enrollment>().ToTable("Enrollment");

        modelBuilder.Entity<SystemAccount>(e =>
        {
            e.HasIndex(a => a.AccountEmail).IsUnique();
        });

        modelBuilder.Entity<Department>(e =>
        {
            e.HasIndex(d => d.DepartmentCode).IsUnique();
        });

        modelBuilder.Entity<Course>(e =>
        {
            e.HasIndex(c => c.CourseCode).IsUnique();
            e.HasOne(c => c.Department)
             .WithMany(d => d.Courses)
             .HasForeignKey(c => c.DepartmentID)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Lecturer>(e =>
        {
            e.HasOne(l => l.Department)
             .WithMany(d => d.Lecturers)
             .HasForeignKey(l => l.DepartmentID)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(l => l.Account)
             .WithOne(a => a.Lecturer)
             .HasForeignKey<Lecturer>(l => l.AccountID)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Student>(e =>
        {
            e.HasIndex(s => s.StudentCode).IsUnique();
            e.HasOne(s => s.Account)
             .WithOne(a => a.Student)
             .HasForeignKey<Student>(s => s.AccountID)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ClassSection>(e =>
        {
            e.HasOne(cs => cs.Course)
             .WithMany(c => c.ClassSections)
             .HasForeignKey(cs => cs.CourseID)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(cs => cs.Lecturer)
             .WithMany(l => l.ClassSections)
             .HasForeignKey(cs => cs.LecturerID)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Enrollment>(e =>
        {
            // Store enum as string to match DB nvarchar(30): 'Pending','Approved','Rejected','Cancelled'
            e.Property(en => en.EnrollmentStatus)
             .HasConversion<string>()
             .HasMaxLength(30);

            e.HasIndex(en => new { en.StudentID, en.ClassID }).IsUnique();
            e.HasOne(en => en.Student)
             .WithMany(s => s.Enrollments)
             .HasForeignKey(en => en.StudentID)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(en => en.ClassSection)
             .WithMany(cs => cs.Enrollments)
             .HasForeignKey(en => en.ClassID)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

using Microsoft.EntityFrameworkCore;
using SchoolCalendarSystem.Models;

public class SchoolContext : DbContext
{
    public SchoolContext(DbContextOptions<SchoolContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<ClassSubject> ClassSubjects { get; set; }
    public DbSet<TeacherSubject> TeacherSubjects { get; set; }
    public DbSet<ScheduleEntry> ScheduleEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure auto-generated primary keys for each entity
        modelBuilder.Entity<Student>()
            .HasKey(s => s.StudentId);
        modelBuilder.Entity<Student>()
            .Property(s => s.StudentId)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Class>()
            .HasKey(c => c.ClassId);
        modelBuilder.Entity<Class>()
            .Property(c => c.ClassId)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Teacher>()
            .HasKey(t => t.TeacherId);
        modelBuilder.Entity<Teacher>()
            .Property(t => t.TeacherId)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Subject>()
            .HasKey(s => s.SubjectId);
        modelBuilder.Entity<Subject>()
            .Property(s => s.SubjectId)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<ClassSubject>()
            .HasKey(cs => new { cs.ClassId, cs.SubjectId });

        modelBuilder.Entity<TeacherSubject>()
            .HasKey(ts => new { ts.TeacherId, ts.SubjectId });

        modelBuilder.Entity<ScheduleEntry>()
            .HasKey(se => se.ScheduleEntryId);
        modelBuilder.Entity<ScheduleEntry>()
            .Property(se => se.ScheduleEntryId)
            .ValueGeneratedOnAdd();

        // Configure many-to-many relationship between Subject and Class
        modelBuilder.Entity<ClassSubject>()
            .HasOne(cs => cs.Class)
            .WithMany(c => c.ClassSubjects)
            .HasForeignKey(cs => cs.ClassId);

        modelBuilder.Entity<ClassSubject>()
            .HasOne(cs => cs.Subject)
            .WithMany(s => s.ClassSubjects)
            .HasForeignKey(cs => cs.SubjectId);

        // Configure many-to-many relationship between Subject and Teacher
        modelBuilder.Entity<TeacherSubject>()
            .HasOne(ts => ts.Teacher)
            .WithMany(t => t.TeacherSubjects)
            .HasForeignKey(ts => ts.TeacherId);

        modelBuilder.Entity<TeacherSubject>()
            .HasOne(ts => ts.Subject)
            .WithMany(s => s.TeacherSubjects)
            .HasForeignKey(ts => ts.SubjectId);

        // Configure one-to-many relationship between Subject and ScheduleEntry
        modelBuilder.Entity<ScheduleEntry>()
            .HasOne(se => se.Subject)
            .WithMany(s => s.ScheduleEntries)
            .HasForeignKey(se => se.SubjectId);

        // Configure one-to-many relationship between Class and ScheduleEntry
        modelBuilder.Entity<ScheduleEntry>()
            .HasOne(se => se.Class)
            .WithMany(c => c.ScheduleEntries)
            .HasForeignKey(se => se.ClassId);

        // Configure one-to-many relationship between Teacher and ScheduleEntry
        modelBuilder.Entity<ScheduleEntry>()
            .HasOne(se => se.Teacher)
            .WithMany(t => t.ScheduleEntries)
            .HasForeignKey(se => se.TeacherId);

        // Configure one-to-many relationship between Student and Class
        modelBuilder.Entity<Student>()
            .HasOne(s => s.Class)
            .WithMany(c => c.Students)
            .HasForeignKey(s => s.ClassId);
    }
}

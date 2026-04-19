using Canvas.Library.Model;
using Microsoft.EntityFrameworkCore;

namespace Canvas.API.Data
{
    public class CanvasDbContext : DbContext
    {
        public CanvasDbContext(DbContextOptions<CanvasDbContext> options) : base(options) { }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssignmentGroup> AssignmentGroups { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<ModuleContent> ModuleContents { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Announcement> Announcements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ── USER INHERITANCE (TPH) ──
            // Student and Instructor share a single "Users" table
            // with a "UserType" discriminator column
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<Student>("Student")
                .HasValue<Instructor>("Instructor");

            // ── MODULECONTENT INHERITANCE (TPH) ──
            // PageContent, FileContent, AssignmentContent share "ModuleContents" table
            // with a "ContentType" discriminator — matches your existing contentType field
            modelBuilder.Entity<ModuleContent>()
                .HasDiscriminator<string>("ContentType")
                .HasValue<PageContent>("Page")
                .HasValue<FileContent>("File")
                .HasValue<AssignmentContent>("Assignment");

            // ── SEMESTER (owned entity) ──
            // Stored as two columns on the Course table: SemesterTaught_Year, SemesterTaught_Session
            modelBuilder.Entity<Course>()
                .OwnsOne(c => c.SemesterTaught);

            // ── ASSIGNMENTGROUP.ASSIGNMENTIDS (List<int>) ──
            // Stored as a JSON column in Postgres
            modelBuilder.Entity<AssignmentGroup>()
                .Property(g => g.AssignmentIds)
                .HasColumnType("jsonb");

            // ── COURSE RELATIONSHIPS ──
            modelBuilder.Entity<Course>()
                .HasMany(c => c.Assignments)
                .WithOne()
                .HasForeignKey("CourseId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Modules)
                .WithOne()
                .HasForeignKey("CourseId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Announcements)
                .WithOne()
                .HasForeignKey("CourseId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.AssignmentGroups)
                .WithOne()
                .HasForeignKey(g => g.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── MODULE RELATIONSHIPS ──
            modelBuilder.Entity<Module>()
                .HasMany(m => m.ModuleContents)
                .WithOne()
                .HasForeignKey("ModuleId")
                .OnDelete(DeleteBehavior.Cascade);

            // ── ASSIGNMENT RELATIONSHIPS ──
            modelBuilder.Entity<Assignment>()
                .HasMany(a => a.Submissions)
                .WithOne()
                .HasForeignKey("AssignmentId")
                .OnDelete(DeleteBehavior.Cascade);

            // ── COURSE/STUDENT ROSTER (many-to-many) ──
            modelBuilder.Entity<Course>()
                .HasMany(c => c.Roster)
                .WithMany()
                .UsingEntity("CourseStudents");

            // ── COURSE/INSTRUCTOR (many-to-many) ──
            modelBuilder.Entity<Course>()
                .HasMany(c => c.Instructors)
                .WithMany()
                .UsingEntity("CourseInstructors");

            // ── IGNORE Module.Content (List<string> field, CLI only) ──
            modelBuilder.Entity<Module>()
                .Ignore("Content");
        }
    }
}
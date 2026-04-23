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
        public DbSet<PageContent> PageContents { get; set; }
        public DbSet<FileContent> FileContents { get; set; }
        public DbSet<AssignmentContent> AssignmentContents { get; set; }

        // ── QUIZ TABLES ──
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<QuizQuestionOption> QuizQuestionOptions { get; set; }
        public DbSet<LetterGrade> LetterGrades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ── USER INHERITANCE (TPH) ──
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<Student>("Student")
                .HasValue<Instructor>("Instructor");

            // ── MODULECONTENT INHERITANCE (TPH) ──
            modelBuilder.Entity<ModuleContent>(b =>
            {
                b.HasDiscriminator<string>("Discriminator")
                    .HasValue<PageContent>("Page")
                    .HasValue<FileContent>("File")
                    .HasValue<AssignmentContent>("Assignment");
                b.Ignore(m => m.ContentType);
            });

            // ── SEMESTER (owned entity) ──
            modelBuilder.Entity<Course>()
                .OwnsOne(c => c.SemesterTaught);

            // ── ASSIGNMENTGROUP.ASSIGNMENTIDS (List<int>) ──
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

            // ── QUIZ RELATIONSHIPS ──
            // Quiz is 1-to-1 with Assignment (one Quiz per quiz-type assignment)
            modelBuilder.Entity<Quiz>()
                .HasOne<Assignment>()
                .WithOne()
                .HasForeignKey<Quiz>(q => q.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quiz has many QuizQuestions
            modelBuilder.Entity<QuizQuestion>()
                .HasOne<Quiz>()
                .WithMany(q => q.Questions)
                .HasForeignKey(qq => qq.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            // QuizQuestion has many QuizQuestionOptions
            modelBuilder.Entity<QuizQuestionOption>()
                .HasOne<QuizQuestion>()
                .WithMany(q => q.Options)
                .HasForeignKey(o => o.QuizQuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Letter Grade Scale builder
            modelBuilder.Entity<Course>()
                .HasMany(c => c.GradeScale)
                .WithOne()
                .HasForeignKey(g => g.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
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
        public DbSet<AssignmentComment> AssignmentComments { get; set; }

        // ── QUIZ TABLES ──
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<QuizQuestionOption> QuizQuestionOptions { get; set; }
        public DbSet<LetterGrade> LetterGrades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<Student>("Student")
                .HasValue<Instructor>("Instructor");

            modelBuilder.Entity<ModuleContent>(b =>
            {
                b.HasDiscriminator<string>("Discriminator")
                    .HasValue<PageContent>("Page")
                    .HasValue<FileContent>("File")
                    .HasValue<AssignmentContent>("Assignment");
                b.Ignore(m => m.ContentType);
            });

            modelBuilder.Entity<Course>()
                .OwnsOne(c => c.SemesterTaught);

            modelBuilder.Entity<AssignmentGroup>()
                .Property(g => g.AssignmentIds)
                .HasColumnType("jsonb");

            modelBuilder.Entity<Submission>()
                .HasMany(s => s.Comments)
                .WithOne(c => c.Submission)
                .HasForeignKey(c => c.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

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

            modelBuilder.Entity<Module>()
                .HasMany(m => m.ModuleContents)
                .WithOne()
                .HasForeignKey("ModuleId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Assignment>()
                .HasMany(a => a.Submissions)
                .WithOne()
                .HasForeignKey("AssignmentId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Roster)
                .WithMany()
                .UsingEntity("CourseStudents");

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Instructors)
                .WithMany()
                .UsingEntity("CourseInstructors");

            modelBuilder.Entity<Module>()
                .Ignore("Content");

            modelBuilder.Entity<Quiz>()
                .HasOne<Assignment>()
                .WithOne()
                .HasForeignKey<Quiz>(q => q.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QuizQuestion>()
                .HasOne<Quiz>()
                .WithMany(q => q.Questions)
                .HasForeignKey(qq => qq.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QuizQuestionOption>()
                .HasOne<QuizQuestion>()
                .WithMany(q => q.Options)
                .HasForeignKey(o => o.QuizQuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.GradeScale)
                .WithOne()
                .HasForeignKey(g => g.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
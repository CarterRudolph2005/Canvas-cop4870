using System.Text;
using Canvas.Library.Model;
using Canvas.API.Data;
using Canvas.API.Services;
using Microsoft.EntityFrameworkCore;

namespace Canvas.API.Enterprise
{
    public class CourseEC
    {
        private readonly CanvasDbContext _context;

        private readonly EmailService _emailService;

        public CourseEC(CanvasDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
}

        private IQueryable<Course> CoursesWithAll =>
            _context.Courses
                .Include(c => c.Assignments).ThenInclude(a => a.Submissions)
                .Include(c => c.Modules).ThenInclude(m => m.ModuleContents)
                .Include(c => c.Announcements)
                .Include(c => c.AssignmentGroups)
                .Include(c => c.Roster)
                .Include(c => c.Instructors)
                .Include(c => c.GradeScale);

        public IEnumerable<Course> GetAll() => CoursesWithAll.ToList();

        public Course? GetById(int id) => CoursesWithAll.FirstOrDefault(c => c.Id == id);

        public Course? Create(Course course)
        {
            if (course == null) return null;
            if (course.Id == 0)
            {
                if (course.Instructors != null)
                {
                    for (int i = 0; i < course.Instructors.Count; i++)
                    {
                        var existing = _context.Instructors
                            .FirstOrDefault(ins => ins.Id == course.Instructors[i].Id);
                        if (existing != null)
                            course.Instructors[i] = existing;
                    }
                }

                course.Id = NextKey;
                _context.Courses.Add(course);
                _context.SaveChanges();
            }
            return course;
        }

        public Course? Update(Course course)
        {
            if (course == null) return null;
            var existing = _context.Courses.FirstOrDefault(c => c.Id == course.Id);
            if (existing == null) return null;
            existing.Name = course.Name;
            existing.Code = course.Code;
            existing.Description = course.Description;
            _context.SaveChanges();
            return existing;
        }

        public Course? Delete(int id)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Id == id);
            if (course == null) return null;
            _context.Courses.Remove(course);
            _context.SaveChanges();
            return course;
        }

        public int NextKey
        {
            get
            {
                if (_context.Courses.Any())
                    return _context.Courses.Select(i => i.Id).Max() + 1;
                return 1;
            }
        }

        public Course? UpdateSemesterDates(int courseId, DateTime? startDate, DateTime? endDate)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return null;

            course.SemesterTaught.StartDate = startDate.HasValue
                ? startDate.Value.ToUniversalTime()
                : null;
            course.SemesterTaught.EndDate = endDate.HasValue
                ? endDate.Value.ToUniversalTime()
                : null;

            _context.SaveChanges();
            return course;
        }

        public async Task<Assignment?> AddOrUpdateAssignment(int courseId, Assignment assignment)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return null;
            course.Assignments ??= new List<Assignment>();
            if (assignment.Id == 0)
            {
                assignment.DueDate = assignment.DueDate.ToUniversalTime();
                course.Assignments.Add(assignment);
                _context.SaveChanges();

                foreach (var student in course.Roster?.Where(s => !string.IsNullOrEmpty(s.Email)) ?? Enumerable.Empty<Student>())
                {
                    await _emailService.SendNewAssignmentEmailAsync(
                        student.Email,
                        student.Name,
                        assignment.Name,
                        course.Name
                    );
                }
            }
            else
            {
                var existing = course.Assignments.FirstOrDefault(a => a.Id == assignment.Id);
                if (existing == null) return null;
                existing.Name = assignment.Name;
                existing.Description = assignment.Description;
                existing.AvailablePoints = assignment.AvailablePoints;
                existing.DueDate = assignment.DueDate;
                existing.GroupId = assignment.GroupId;
                _context.SaveChanges();
            }
            return assignment;
        }

        public Assignment? GetAssignmentById(int courseId, int assignmentId)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            return course?.Assignments?.FirstOrDefault(a => a.Id == assignmentId);
        }

        public bool DeleteAssignment(int courseId, int assignmentId)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            if (course == null || course.Assignments == null) return false;
            var assignment = course.Assignments.FirstOrDefault(a => a.Id == assignmentId);
            if (assignment == null) return false;
            if (assignment.GroupId != 0)
                RemoveAssignmentFromGroup(courseId, assignment.GroupId, assignmentId);
            assignment.Submissions?.Clear();
            course.Assignments.Remove(assignment);
            foreach (var module in course.Modules ?? new List<Module>())
                module.ModuleContents?.RemoveAll(c => c is AssignmentContent ac && ac.AssignmentId == assignmentId);
            _context.SaveChanges();
            return true;
        }

        public void CopyAssignmentToCourse(int assignmentId, int sourceCourseId, int targetCourseId)
        {
            var source = CoursesWithAll.FirstOrDefault(c => c.Id == sourceCourseId);
            if (source == null) return;
            var assignment = source.Assignments?.FirstOrDefault(a => a.Id == assignmentId);
            if (assignment == null) return;
            var copy = new Assignment
            {
                Id = 0,
                Name = assignment.Name,
                Description = assignment.Description,
                AvailablePoints = assignment.AvailablePoints,
                DueDate = assignment.DueDate,
                Submissions = new List<Submission>(),
                GroupId = 0
            };
            AddOrUpdateAssignment(targetCourseId, copy);
        }

        private int AssignmentNextKey(Course course)
        {
            if (course.Assignments != null && course.Assignments.Any())
                return course.Assignments.Max(a => a.Id) + 1;
            return 1;
        }

        // ── MODULES ──

        public void AddModule(int courseId, string moduleName)
        {
            if (moduleName == null) return;
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return;
            course.Modules ??= new List<Module>();
            var module = new Module { ModuleName = moduleName, Id = 0 };
            course.Modules.Add(module);
            _context.SaveChanges();
        }

        public void DeleteModule(int courseId, int moduleId)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            var module = course?.Modules?.FirstOrDefault(m => m.Id == moduleId);
            if (module != null)
            {
                course.Modules.Remove(module);
                _context.SaveChanges();
            }
        }

        public bool AddModuleContent(int courseId, int moduleId, string newContent)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            var module = course?.Modules?.FirstOrDefault(m => m.Id == moduleId);
            if (module == null) return false;
            module.Content ??= new List<string>();
            module.Content.Add(newContent);
            _context.SaveChanges();
            return true;
        }

        public void UpdateModuleContent(int courseId, int moduleId, int contentIndex, string newContent)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            var module = course?.Modules?.FirstOrDefault(m => m.Id == moduleId);
            if (module != null && module.Content != null)
                if (contentIndex >= 0 && contentIndex < module.Content.Count)
                {
                    module.Content[contentIndex] = newContent;
                    _context.SaveChanges();
                }
        }

        public void DeleteModuleContent(int courseId, int moduleId, int contentIndex)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            var module = course?.Modules?.FirstOrDefault(m => m.Id == moduleId);
            if (module == null || module.Content == null) return;
            if (contentIndex >= 0 && contentIndex < module.Content.Count)
            {
                module.Content.RemoveAt(contentIndex);
                _context.SaveChanges();
            }
        }

        public bool AddModuleContents(int courseId, int moduleId, ModuleContent content)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return false;
            var module = course.Modules?.FirstOrDefault(m => m.Id == moduleId);
            if (module == null) return false;
            module.ModuleContents ??= new List<ModuleContent>();
            var allContents = course.Modules
                .SelectMany(m => m.ModuleContents ?? new List<ModuleContent>())
                .ToList();
            content.Id = 0;//allContents.Any() ? allContents.Max(c => c.Id) + 1 : 1;
            module.ModuleContents.Add(content);
            _context.SaveChanges();
            return true;
        }

        public void DeleteModuleContents(int courseId, int moduleId, int contentId)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            var module = course?.Modules?.FirstOrDefault(m => m.Id == moduleId);
            if (module == null) return;
            var content = module.ModuleContents?.FirstOrDefault(i => i.Id == contentId);
            if (content != null)
            {
                module.ModuleContents.Remove(content);
                _context.SaveChanges();
            }
        }

        public void EnrollStudent(int studentId, int courseId)
        {
            var student = _context.Students.FirstOrDefault(s => s.Id == studentId);
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            if (student == null || course == null) return;
            if (course.Roster?.Any(s => s.Id == studentId) == true) return;
            course.Roster ??= new List<Student>();
            course.Roster.Add(student);
            _context.SaveChanges();
        }

        public void UnenrollStudent(int courseId, int studentId)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return;
            course.Assignments?.ForEach(a =>
            {
                var subs = a.Submissions?.Where(s => s.StudentId == studentId).ToList();
                subs?.ForEach(s => a.Submissions.Remove(s));
            });
            var student = course.Roster?.FirstOrDefault(s => s.Id == studentId);
            if (student != null)
                course.Roster.Remove(student);
            _context.SaveChanges();
        }

        public bool UnenrollStudentFromAllCourses(int studentId)
        {
            DeleteAllStudentsSubmissions(studentId);
            bool found = false;
            foreach (var course in CoursesWithAll.ToList())
            {
                var student = course.Roster?.FirstOrDefault(s => s.Id == studentId);
                if (student != null)
                {
                    course.Roster.Remove(student);
                    found = true;
                }
            }
            _context.SaveChanges();
            return found;
        }

        public void DeleteAllStudentsSubmissions(int studentId)
        {
            var submissions = _context.Submissions.Where(s => s.StudentId == studentId).ToList();
            _context.Submissions.RemoveRange(submissions);
            _context.SaveChanges();
        }

        public List<Course> GetCoursesForStudent(int studentId)
        {
            return CoursesWithAll
                .ToList()
                .Where(c => c.Roster != null && c.Roster.Any(s => s.Id == studentId))
                .ToList();
        }

        public string ExportRoster(int courseId)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return string.Empty;
            var sb = new StringBuilder();
            sb.AppendLine("StudentCode");
            foreach (var student in course.Roster ?? new List<Student>())
                sb.AppendLine(student.Code);
            return sb.ToString();
        }

        public (int added, int skipped, int notFound) ImportRoster(int courseId, string csvContent)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return (0, 0, 0);
            course.Roster ??= new List<Student>();
            int added = 0, skipped = 0, notFound = 0;
            var lines = csvContent
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Skip(1).ToList();
            foreach (var code in lines)
            {
                var student = _context.Students.FirstOrDefault(s => s.Code == code);
                if (student == null) { notFound++; continue; }
                if (course.Roster.Any(s => s.Code == code)) { skipped++; continue; }
                course.Roster.Add(student);
                added++;
            }
            _context.SaveChanges();
            return (added, skipped, notFound);
        }

        public List<Submission> GetStudentSubmissions(int courseId, int studentId)
        {
            return CoursesWithAll.FirstOrDefault(c => c.Id == courseId)
                ?.Assignments
                ?.SelectMany(a => a.Submissions)
                .Where(s => s.StudentId == studentId)
                .ToList() ?? new List<Submission>();
        }

        public void SubmitAssignment(int courseId, Submission submission)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            var assignment = course?.Assignments?.FirstOrDefault(a => a.Id == submission.AssignmentId);
            if (assignment == null) return;
            assignment.Submissions ??= new List<Submission>();
            int nextId = assignment.Submissions.Any()
                ? assignment.Submissions.Max(s => s.Id) + 1
                : 1;
            submission.Id = 0;//nextId;
            assignment.Submissions.Add(submission);
            _context.SaveChanges();
        }

        public void GradeSubmission(int courseId, int assignmentId, int submissionId, int points)
        {
            var submission = CoursesWithAll
                .FirstOrDefault(c => c.Id == courseId)
                ?.Assignments?.FirstOrDefault(a => a.Id == assignmentId)
                ?.Submissions?.FirstOrDefault(s => s.Id == submissionId);
            if (submission == null) return;
            submission.PointsAwarded = points;
            _context.SaveChanges();
        }

        public double CalculateGrade(int courseId, int studentId)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return 0;
            if (course.Assignments == null || !course.Assignments.Any()) return 0;
            double totalEarned = 0, totalAvailable = 0;
            foreach (var group in course.AssignmentGroups ?? new List<AssignmentGroup>())
            {
                var groupAssignments = course.Assignments
                    .Where(a => group.AssignmentIds.Contains(a.Id)).ToList();
                var submittedInGroup = groupAssignments
                    .Where(a => a.Submissions != null && a.Submissions.Any(s => s.StudentId == studentId)).ToList();
                if (!submittedInGroup.Any()) continue;
                var groupAvailable = submittedInGroup.Sum(a => a.AvailablePoints);
                var groupEarned = submittedInGroup
                    .SelectMany(a => a.Submissions)
                    .Where(s => s.StudentId == studentId)
                    .Sum(s => s.PointsAwarded ?? 0);
                if (groupAvailable > 0)
                {
                    totalEarned += (double)groupEarned / groupAvailable * group.TotalPoints;
                    totalAvailable += group.TotalPoints;
                }
            }
            var ungrouped = course.Assignments
                .Where(a => a.GroupId == 0 && a.Submissions != null && a.Submissions.Any(s => s.StudentId == studentId))
                .ToList();
            totalAvailable += ungrouped.Sum(a => a.AvailablePoints);
            totalEarned += ungrouped.SelectMany(a => a.Submissions)
                .Where(s => s.StudentId == studentId)
                .Sum(s => s.PointsAwarded ?? 0);
            return totalAvailable > 0 ? totalEarned / totalAvailable * 100 : 0;
        }

        public void AddAnnouncement(int courseId, Announcement announcement)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return;
            course.Announcements ??= new List<Announcement>();
            announcement.Id = 0;
            announcement.PostedDate = announcement.PostedDate.ToUniversalTime();
            course.Announcements.Add(announcement);
            _context.SaveChanges();
        }

        public void UpdateAnnouncement(int courseId, Announcement updated)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            var announcement = course?.Announcements?.FirstOrDefault(a => a.Id == updated.Id);
            if (announcement == null) return;
            announcement.Title = updated.Title;
            announcement.Body = updated.Body;
            _context.SaveChanges();
        }

        public void DeleteAnnouncement(int courseId, int announcementId)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            var announcement = course?.Announcements?.FirstOrDefault(a => a.Id == announcementId);
            if (announcement != null)
            {
                course.Announcements.Remove(announcement);
                _context.SaveChanges();
            }
        }

        public AssignmentGroup? AddOrUpdateAssignmentGroup(int courseId, AssignmentGroup group)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return null;
            course.AssignmentGroups ??= new List<AssignmentGroup>();
            if (group.Id == 0)
            {
                group.Id = 0; 
                group.CourseId = courseId;
                course.AssignmentGroups.Add(group);
            }
            else
            {
                var existing = course.AssignmentGroups.FirstOrDefault(g => g.Id == group.Id);
                if (existing == null) return null;
                existing.Name = group.Name;
                existing.TotalPoints = group.TotalPoints;
            }
            _context.SaveChanges();
            return group;
        }

        public bool DeleteAssignmentGroup(int courseId, int groupId)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return false;
            var group = course.AssignmentGroups?.FirstOrDefault(g => g.Id == groupId);
            if (group == null) return false;
            foreach (var assignmentId in group.AssignmentIds)
            {
                var assignment = course.Assignments?.FirstOrDefault(a => a.Id == assignmentId);
                if (assignment != null)
                    assignment.GroupId = 0;
            }
            course.AssignmentGroups.Remove(group);
            _context.SaveChanges();
            return true;
        }

        public bool AddAssignmentToGroup(int courseId, int groupId, int assignmentId)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return false;
            var group = course.AssignmentGroups?.FirstOrDefault(g => g.Id == groupId);
            var assignment = course.Assignments?.FirstOrDefault(a => a.Id == assignmentId);
            if (group == null || assignment == null) return false;
            if (assignment.GroupId != 0)
                RemoveAssignmentFromGroup(courseId, assignment.GroupId, assignmentId);
            assignment.GroupId = groupId;
            if (!group.AssignmentIds.Contains(assignmentId))
                group.AssignmentIds.Add(assignmentId);
            _context.SaveChanges();
            return true;
        }

        public bool RemoveAssignmentFromGroup(int courseId, int groupId, int assignmentId)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return false;
            var group = course.AssignmentGroups?.FirstOrDefault(g => g.Id == groupId);
            var assignment = course.Assignments?.FirstOrDefault(a => a.Id == assignmentId);
            if (group == null || assignment == null) return false;
            assignment.GroupId = 0;
            group.AssignmentIds.Remove(assignmentId);
            _context.SaveChanges();
            return true;
        }

        public List<AssignmentGroup> GetAssignmentGroups(int courseId)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            return course?.AssignmentGroups ?? new List<AssignmentGroup>();
        }

        public AssignmentGroup? GetAssignmentGroupById(int courseId, int groupId)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            return course?.AssignmentGroups?.FirstOrDefault(g => g.Id == groupId);
        }

        private int AssignmentGroupNextKey(Course course)
        {
            if (course.AssignmentGroups != null && course.AssignmentGroups.Any())
                return course.AssignmentGroups.Max(g => g.Id) + 1;
            return 1;
        }


        public Course? CopyCourse(int sourceCourseId, int sectionNumber, int year, SemesterType semester, int instructorId)
        {
            var source = CoursesWithAll.FirstOrDefault(c => c.Id == sourceCourseId);
            if (source == null) return null;

            var copy = new Course
            {
                Id = 0,
                Name = source.Name,
                Code = source.Code,
                Description = source.Description,
                SectionNumber = sectionNumber,
                SemesterTaught = new Semester(year, semester),
                Instructors = new List<Instructor>(),
                Assignments = source.Assignments?.Select(a => new Assignment
                {
                    Id = 0,
                    Name = a.Name,
                    Description = a.Description,
                    AvailablePoints = a.AvailablePoints,
                    DueDate = a.DueDate,
                    GroupId = 0,
                    Submissions = new List<Submission>()
                }).ToList() ?? new List<Assignment>(),
                AssignmentGroups = new List<AssignmentGroup>(),
                Modules = source.Modules?.Select(m => new Module
                {
                    Id = 0,
                    ModuleName = m.ModuleName,
                    ModuleContents = m.ModuleContents?.Select(c => c switch
                    {
                        AssignmentContent ac => (ModuleContent)new AssignmentContent { Id = 0, AssignmentId = ac.AssignmentId, Name = ac.Name },
                        FileContent fc => new FileContent { Id = 0, Name = fc.Name, FilePath = fc.FilePath ?? string.Empty, MimeType = fc.MimeType ?? string.Empty },
                        PageContent pc => new PageContent { Id = 0, Name = pc.Name, Body = pc.Body ?? string.Empty },
                        _ => null
                    }).Where(c => c != null).ToList() ?? new List<ModuleContent>()
                }).ToList() ?? new List<Module>(),
                Announcements = source.Announcements?.Select(a => new Announcement
                {
                    Id = 0,
                    Title = a.Title,
                    Body = a.Body,
                    PostedDate = a.PostedDate.ToUniversalTime()
                }).ToList() ?? new List<Announcement>(),
                Roster = new List<Student>(),
                GradeScale = new List<LetterGrade>()
            };

            _context.ChangeTracker.Clear();

            var instructor = _context.Instructors.FirstOrDefault(i => i.Id == instructorId);
            if (instructor != null)
                copy.Instructors.Add(instructor);

            _context.Courses.Add(copy);
            _context.SaveChanges();
            return copy;
        }

        public Submission? GetStudentSubmission(int courseId, int assignmentId, int studentId)
        {
            return CoursesWithAll
                .FirstOrDefault(c => c.Id == courseId)
                ?.Assignments?.FirstOrDefault(a => a.Id == assignmentId)
                ?.Submissions?.FirstOrDefault(s => s.StudentId == studentId);
        }

        public Quiz? GetQuiz(int courseId, int assignmentId)
        {
            // Verify the assignment belongs to this course
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            if (course?.Assignments?.Any(a => a.Id == assignmentId) != true) return null;

            return _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(qq => qq.Options)
                .FirstOrDefault(q => q.AssignmentId == assignmentId);
        }

        public Quiz? CreateQuiz(int courseId, int assignmentId, int? timeLimitMinutes, int allowedAttempts)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            var assignment = course?.Assignments?.FirstOrDefault(a => a.Id == assignmentId);
            if (assignment == null) return null;

            var existing = _context.Quizzes
                .Include(q => q.Questions).ThenInclude(qq => qq.Options)
                .FirstOrDefault(q => q.AssignmentId == assignmentId);
            if (existing != null) return existing;

            assignment.IsQuiz = true;
            var quiz = new Quiz
            {
                Id = 0,
                AssignmentId = assignmentId,
                TimeLimitMinutes = timeLimitMinutes,
                AllowedAttempts = allowedAttempts < 1 ? 1 : allowedAttempts,
                Questions = new List<QuizQuestion>()
            };
            _context.Quizzes.Add(quiz);
            _context.SaveChanges();
            return quiz;
        }

        public Quiz? UpdateQuiz(int courseId, int assignmentId, int? timeLimitMinutes, int allowedAttempts)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            if (course?.Assignments?.Any(a => a.Id == assignmentId) != true) return null;

            var quiz = _context.Quizzes
                .Include(q => q.Questions).ThenInclude(qq => qq.Options)
                .FirstOrDefault(q => q.AssignmentId == assignmentId);
            if (quiz == null) return null;

            quiz.TimeLimitMinutes = timeLimitMinutes;
            quiz.AllowedAttempts = allowedAttempts < 1 ? 1 : allowedAttempts;
            _context.SaveChanges();
            return quiz;
        }

        public bool DeleteQuiz(int courseId, int assignmentId)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            var assignment = course?.Assignments?.FirstOrDefault(a => a.Id == assignmentId);
            if (assignment == null) return false;

            var quiz = _context.Quizzes.FirstOrDefault(q => q.AssignmentId == assignmentId);
            if (quiz == null) return false;

            assignment.IsQuiz = false;
            _context.Quizzes.Remove(quiz);
            _context.SaveChanges();
            return true;
        }

        public QuizQuestion? AddQuestion(int courseId, int assignmentId, QuizQuestion question)
        {
            var quiz = GetQuiz(courseId, assignmentId);
            if (quiz == null) return null;

            // Separate options out before the first save so we can assign
            // the real QuizQuestionId (generated by Postgres) to each option.
            var incomingOptions = question.Options ?? new List<QuizQuestionOption>();

            question.Id = 0;
            question.QuizId = quiz.Id;
            question.Options = new List<QuizQuestionOption>(); // save question first with no options

            _context.QuizQuestions.Add(question);
            _context.SaveChanges(); // question.Id is now set by Postgres

            // Now add options with the real question ID
            foreach (var opt in incomingOptions)
            {
                opt.Id = 0;
                opt.QuizQuestionId = question.Id;
                _context.QuizQuestionOptions.Add(opt);
            }
            _context.SaveChanges();

            question.Options = incomingOptions; // reattach for return
            return question;
        }

        public QuizQuestion? UpdateQuestion(int courseId, int assignmentId, int questionId, string questionText, int points)
        {
            var quiz = GetQuiz(courseId, assignmentId);
            if (quiz == null) return null;

            var question = quiz.Questions.FirstOrDefault(q => q.Id == questionId);
            if (question == null) return null;

            question.QuestionText = questionText;
            question.Points = points;
            _context.SaveChanges();
            return question;
        }

        public bool DeleteQuestion(int courseId, int assignmentId, int questionId)
        {
            var quiz = GetQuiz(courseId, assignmentId);
            if (quiz == null) return false;

            var question = quiz.Questions.FirstOrDefault(q => q.Id == questionId);
            if (question == null) return false;

            _context.QuizQuestions.Remove(question);
            _context.SaveChanges();
            return true;
        }

        public QuizQuestionOption? AddOption(int courseId, int assignmentId, int questionId, QuizQuestionOption option)
        {
            var quiz = GetQuiz(courseId, assignmentId);
            var question = quiz?.Questions.FirstOrDefault(q => q.Id == questionId);
            if (question == null) return null;

            option.Id = 0;
            option.QuizQuestionId = questionId;
            _context.QuizQuestionOptions.Add(option);
            _context.SaveChanges();
            return option;
        }

        public QuizQuestionOption? UpdateOption(int courseId, int assignmentId, int questionId, int optionId, string optionText, bool isCorrect)
        {
            var quiz = GetQuiz(courseId, assignmentId);
            var question = quiz?.Questions.FirstOrDefault(q => q.Id == questionId);
            var option = question?.Options.FirstOrDefault(o => o.Id == optionId);
            if (option == null) return null;

            option.OptionText = optionText;
            option.IsCorrect = isCorrect;
            _context.SaveChanges();
            return option;
        }

        public async Task<List<Assignment>> ImportAssignmentsAsync(int courseId, List<Assignment> assignments)
        {
            var course = await _context.Courses
                .Include(c => c.Assignments)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null) return [];

            foreach (var assignment in assignments)
            {
                assignment.Id = 0; // let Postgres auto-generate
                course.Assignments.Add(assignment); // EF tracks the relationship via the parent
            }

            await _context.SaveChangesAsync();
            return assignments;
        }

        public bool DeleteOption(int courseId, int assignmentId, int questionId, int optionId)
        {
            var quiz = GetQuiz(courseId, assignmentId);
            var question = quiz?.Questions.FirstOrDefault(q => q.Id == questionId);
            var option = question?.Options.FirstOrDefault(o => o.Id == optionId);
            if (option == null) return false;

            _context.QuizQuestionOptions.Remove(option);
            _context.SaveChanges();
            return true;
        }

        public List<LetterGrade> GetGradeScale(int courseId)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            return course?.GradeScale ?? new List<LetterGrade>();
        }

        public LetterGrade? AddOrUpdateLetterGrade(int courseId, LetterGrade grade)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return null;
            course.GradeScale ??= new List<LetterGrade>();

            if (grade.Id == 0)
            {
                grade.CourseId = courseId;
                course.GradeScale.Add(grade);
            }
            else
            {
                var existing = course.GradeScale.FirstOrDefault(g => g.Id == grade.Id);
                if (existing == null) return null;
                existing.MinPercentage = grade.MinPercentage;
                existing.MaxPercentage = grade.MaxPercentage;
                existing.HexColor = grade.HexColor;
            }
            _context.SaveChanges();
            return grade;
        }

        public bool DeleteLetterGrade(int courseId, int gradeId)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            var grade = course?.GradeScale?.FirstOrDefault(g => g.Id == gradeId);
            if (grade == null) return false;
            course.GradeScale.Remove(grade);
            _context.SaveChanges();
            return true;
        }

        public string GetLetterGradeForScore(int courseId, double percentage)
        {
            var course = CoursesWithAll.FirstOrDefault(c => c.Id == courseId);
            if (course?.GradeScale == null || !course.GradeScale.Any()) return "N/A";
            var match = course.GradeScale
                .FirstOrDefault(g => percentage >= g.MinPercentage && percentage <= g.MaxPercentage);
            return match?.Letter ?? "N/A";
        }
        // ── COMMENTS ──

        public List<AssignmentComment> GetCommentsBySubmission(int submissionId)
        {
            return _context.AssignmentComments
                .Where(c => c.SubmissionId == submissionId)
                .OrderBy(c => c.CreatedAt)
                .ToList();
        }

        public AssignmentComment? AddComment(AssignmentComment comment)
        {
            comment.Id = 0;
            comment.CreatedAt = DateTime.UtcNow;
            _context.AssignmentComments.Add(comment);
            _context.SaveChanges();
            return comment;
        }

        public bool DeleteComment(int commentId)
        {
            var comment = _context.AssignmentComments.FirstOrDefault(c => c.Id == commentId);
            if (comment == null) return false;
            _context.AssignmentComments.Remove(comment);
            _context.SaveChanges();
            return true;
        }
    
    }
}
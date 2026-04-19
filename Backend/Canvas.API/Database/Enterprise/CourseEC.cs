using System.Text;
using Canvas.Library.Model;

namespace Canvas.API.Enterprise
{
    public class CourseEC
    {
        public IEnumerable<Course> GetAll()
        {
            return FakeDatabase.Courses;
        }

        public Course? GetById(int id)
        {
            return FakeDatabase.Courses.FirstOrDefault(c => c.Id == id);
        }

        public Course? Create(Course course)
        {
            if (course == null) return null;
            if (course.Id == 0)
            {
                course.Id = NextKey;
                FakeDatabase.Courses.Add(course);
            }
            return course;
        }

        public Course? Update(Course course)
        {
            if (course == null) return null;
            var existing = FakeDatabase.Courses.FirstOrDefault(c => c.Id == course.Id);
            if (existing == null) return null;
            existing.Name = course.Name;
            existing.Code = course.Code;
            existing.Description = course.Description;
            return existing;
        }

        public Course? Delete(int id)
        {
            var course = FakeDatabase.Courses.FirstOrDefault(c => c.Id == id);
            if (course == null) return null;
            FakeDatabase.Courses.Remove(course);
            return course;
        }

        public int NextKey
        {
            get
            {
                if (FakeDatabase.Courses.Any())
                    return FakeDatabase.Courses.Select(i => i.Id).Max() + 1;
                return 1;
            }
        }

        // ── ASSIGNMENTS ──

        public Assignment? AddOrUpdateAssignment(int courseId, Assignment assignment)
        {
            var course = FakeDatabase.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return null;
            course.Assignments ??= new List<Assignment>();
            if (assignment.Id == 0)
            {
                assignment.Id = AssignmentNextKey(course);
                course.Assignments.Add(assignment);
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
            }
            return assignment;
        }

        public Assignment? GetAssignmentById(int courseId, int assignmentId)
        {
            var course = FakeDatabase.Courses.FirstOrDefault(c => c.Id == courseId);
            return course?.Assignments?.FirstOrDefault(a => a.Id == assignmentId);
        }

        public bool DeleteAssignment(int courseId, int assignmentId)
        {
            var course = FakeDatabase.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null || course.Assignments == null) return false;
            var assignment = course.Assignments.FirstOrDefault(a => a.Id == assignmentId);
            if (assignment == null) return false;
            if (assignment.GroupId != 0)
                RemoveAssignmentFromGroup(courseId, assignment.GroupId, assignmentId);
            assignment.Submissions?.Clear();
            course.Assignments.Remove(assignment);
            if (course.Modules == null) return true;
            foreach (var module in course.Modules)
                module.ModuleContents?.RemoveAll(c => c is AssignmentContent ac && ac.AssignmentId == assignmentId);
            return true;
        }

        public void CopyAssignmentToCourse(int assignmentId, int sourceCourseId, int targetCourseId)
        {
            var source = FakeDatabase.Courses.FirstOrDefault(c => c.Id == sourceCourseId);
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
            var course = FakeDatabase.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return;
            course.Modules ??= new List<Module>();
            int nextModuleKey = course.Modules.Any()
                ? course.Modules.Select(i => i.Id).Max() + 1
                : 1;
            var module = new Module { ModuleName = moduleName, Id = nextModuleKey };
            course.Modules.Add(module);
        }

        public void DeleteModule(int courseId, int moduleId)
        {
            var course = FakeDatabase.Courses.FirstOrDefault(c => c.Id == courseId);
            var module = course?.Modules?.FirstOrDefault(m => m.Id == moduleId);
            if (module != null)
                course.Modules.Remove(module);
        }

        public bool AddModuleContent(int courseId, int moduleId, string newContent)
        {
            var module = FakeDatabase.Courses.FirstOrDefault(c => c.Id == courseId)
                        ?.Modules?.FirstOrDefault(m => m.Id == moduleId);
            if (module == null) return false;
            module.Content ??= new List<string>();
            module.Content.Add(newContent);
            return true;
        }

        public void UpdateModuleContent(int courseId, int moduleId, int contentIndex, string newContent)
        {
            var course = FakeDatabase.Courses.FirstOrDefault(c => c.Id == courseId);
            var module = course?.Modules?.FirstOrDefault(m => m.Id == moduleId);
            if (module != null && module.Content != null)
                if (contentIndex >= 0 && contentIndex < module.Content.Count)
                    module.Content[contentIndex] = newContent;
        }

        public void DeleteModuleContent(int courseId, int moduleId, int contentIndex)
        {
            var course = FakeDatabase.Courses.FirstOrDefault(c => c.Id == courseId);
            var module = course?.Modules?.FirstOrDefault(m => m.Id == moduleId);
            if (module == null || module.Content == null) return;
            if (contentIndex >= 0 && contentIndex < module.Content.Count)
                module.Content.RemoveAt(contentIndex);
        }

        public bool AddModuleContents(int courseId, int moduleId, ModuleContent content)
        {
            var course = FakeDatabase.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return false;
            var module = course.Modules?.FirstOrDefault(m => m.Id == moduleId);
            if (module == null) return false;
            module.ModuleContents ??= new List<ModuleContent>();
            var allContents = course.Modules
                .SelectMany(m => m.ModuleContents ?? new List<ModuleContent>())
                .ToList();
            content.Id = allContents.Any() ? allContents.Max(c => c.Id) + 1 : 1;
            module.ModuleContents.Add(content);
            return true;
        }

        public void DeleteModuleContents(int courseId, int moduleId, int contentId)
        {
            var course = FakeDatabase.Courses.FirstOrDefault(c => c.Id == courseId);
            var module = course?.Modules?.FirstOrDefault(m => m.Id == moduleId);
            if (module == null) return;
            var content = module.ModuleContents?.FirstOrDefault(i => i.Id == contentId);
            if (content != null)
                module.ModuleContents.Remove(content);
        }

        // ── ROSTER ──

        public void EnrollStudent(int studentId, int courseId)
        {
            var student = FakeDatabase.Students.FirstOrDefault(s => s.Id == studentId);
            var course = FakeDatabase.Courses.FirstOrDefault(c => c.Id == courseId);
            if (student == null || course == null) return;
            if (course.Roster?.Any(s => s.Id == studentId) == true) return;
            course.Roster?.Add(student);
        }

        public void UnenrollStudent(int courseId, int studentId)
        {
            var course = FakeDatabase.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return;

            // clean up submissions for this course only
            course.Assignments?.ForEach(a =>
            {
                var index = a.Submissions?.FindIndex(s => s.StudentId == studentId) ?? -1;
                if (index >= 0)
                    a.Submissions.RemoveAt(index);
            });

            var student = course.Roster?.FirstOrDefault(s => s.Id == studentId);
            if (student != null)
                course.Roster.Remove(student);
        }

        public bool UnenrollStudentFromAllCourses(int studentId)
        {
            DeleteAllStudentsSubmissions(studentId);
            bool found = false;
            foreach (var course in FakeDatabase.Courses)
            {
                var student = course.Roster?.FirstOrDefault(s => s.Id == studentId);
                if (student != null)
                {
                    course.Roster.Remove(student);
                    found = true;
                }
            }
            return found;
        }

        public void DeleteAllStudentsSubmissions(int studentId)
        {
            FakeDatabase.Courses.ForEach(c =>
            {
                c.Assignments?.ForEach(a =>
                {
                    var index = a.Submissions?.FindIndex(s => s.StudentId == studentId) ?? -1;
                    if (index >= 0)
                        a.Submissions.RemoveAt(index);
                });
            });
        }

        public List<Course> GetCoursesForStudent(int studentId)
        {
            return FakeDatabase.Courses
                .Where(c => c.Roster != null && c.Roster.Any(s => s != null && s.Id == studentId))
                .ToList();
        }

        public string ExportRoster(int courseId)
        {
            var course = FakeDatabase.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return string.Empty;
            var sb = new StringBuilder();
            sb.AppendLine("StudentCode");
            foreach (var student in course.Roster ?? new List<Student>())
                sb.AppendLine(student.Code);
            return sb.ToString();
        }

        public (int added, int skipped, int notFound) ImportRoster(int courseId, string csvContent)
        {
            var course = FakeDatabase.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return (0, 0, 0);
            course.Roster ??= new List<Student>();
            int added = 0, skipped = 0, notFound = 0;
            var lines = csvContent
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Skip(1)
                .ToList();
            foreach (var code in lines)
            {
                var student = FakeDatabase.Students.FirstOrDefault(s => s.Code == code);
                if (student == null) { notFound++; continue; }
                if (course.Roster.Any(s => s.Code == code)) { skipped++; continue; }
                course.Roster.Add(student);
                added++;
            }
            return (added, skipped, notFound);
        }

        // ── GRADES & SUBMISSIONS ──

        public List<Submission> GetStudentSubmissions(int courseId, int studentId)
        {
            return FakeDatabase.Courses.FirstOrDefault(c => c.Id == courseId)
                ?.Assignments
                ?.SelectMany(a => a.Submissions)
                .Where(s => s.StudentId == studentId)
                .ToList() ?? new List<Submission>();
        }

        public void SubmitAssignment(int courseId, Submission submission)
        {
            var course = FakeDatabase.Courses.FirstOrDefault(c => c.Id == courseId);
            var assignment = course?.Assignments?.FirstOrDefault(a => a.Id == submission.AssignmentId);
            if (assignment == null) return;
            assignment.Submissions ??= new List<Submission>();
            int nextId = assignment.Submissions.Any()
                ? assignment.Submissions.Max(s => s.Id) + 1
                : 1;
            submission.Id = nextId;
            assignment.Submissions.Add(submission);
        }

public void GradeSubmission(int courseId, int assignmentId, int submissionId, int points)
{
    FakeDatabase.Courses
        .FirstOrDefault(c => c.Id == courseId)
        ?.Assignments?.FirstOrDefault(a => a.Id == assignmentId)
        ?.Submissions?.FirstOrDefault(s => s.Id == submissionId)
        .PointsAwarded = points; // you already have this logic
}

        public double CalculateGrade(int courseId, int studentId)
        {
            var course = FakeDatabase.Courses.FirstOrDefault(c => c.Id == courseId);
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

        // ── ANNOUNCEMENTS ──

        public void AddAnnouncement(int courseId, Announcement announcement)
        {
            var course = FakeDatabase.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return;
            course.Announcements ??= new List<Announcement>();
            announcement.Id = course.Announcements.Any()
                ? course.Announcements.Max(a => a.Id) + 1
                : 1;
            course.Announcements.Add(announcement);
        }

        public void UpdateAnnouncement(int courseId, Announcement updated)
        {
            var course = FakeDatabase.Courses.FirstOrDefault(c => c.Id == courseId);
            var announcement = course?.Announcements?.FirstOrDefault(a => a.Id == updated.Id);
            if (announcement == null) return;
            announcement.Title = updated.Title;
            announcement.Body = updated.Body;
        }

        public void DeleteAnnouncement(int courseId, int announcementId)
        {
            var course = FakeDatabase.Courses.FirstOrDefault(c => c.Id == courseId);
            var announcement = course?.Announcements?.FirstOrDefault(a => a.Id == announcementId);
            if (announcement != null)
                course.Announcements.Remove(announcement);
        }

        // ── ASSIGNMENT GROUPS ──

        public AssignmentGroup? AddOrUpdateAssignmentGroup(int courseId, AssignmentGroup group)
        {
            var course = FakeDatabase.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return null;
            course.AssignmentGroups ??= new List<AssignmentGroup>();
            if (group.Id == 0)
            {
                group.Id = AssignmentGroupNextKey(course);
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
            return group;
        }

        public bool DeleteAssignmentGroup(int courseId, int groupId)
        {
            var course = FakeDatabase.Courses.FirstOrDefault(c => c.Id == courseId);
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
            return true;
        }

        public bool AddAssignmentToGroup(int courseId, int groupId, int assignmentId)
        {
            var course = FakeDatabase.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return false;
            var group = course.AssignmentGroups?.FirstOrDefault(g => g.Id == groupId);
            var assignment = course.Assignments?.FirstOrDefault(a => a.Id == assignmentId);
            if (group == null || assignment == null) return false;
            if (assignment.GroupId != 0)
                RemoveAssignmentFromGroup(courseId, assignment.GroupId, assignmentId);
            assignment.GroupId = groupId;
            if (!group.AssignmentIds.Contains(assignmentId))
                group.AssignmentIds.Add(assignmentId);
            return true;
        }

        public bool RemoveAssignmentFromGroup(int courseId, int groupId, int assignmentId)
        {
            var course = FakeDatabase.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return false;
            var group = course.AssignmentGroups?.FirstOrDefault(g => g.Id == groupId);
            var assignment = course.Assignments?.FirstOrDefault(a => a.Id == assignmentId);
            if (group == null || assignment == null) return false;
            assignment.GroupId = 0;
            group.AssignmentIds.Remove(assignmentId);
            return true;
        }

        public List<AssignmentGroup> GetAssignmentGroups(int courseId)
        {
            var course = FakeDatabase.Courses.FirstOrDefault(c => c.Id == courseId);
            return course?.AssignmentGroups ?? new List<AssignmentGroup>();
        }

        public AssignmentGroup? GetAssignmentGroupById(int courseId, int groupId)
        {
            var course = FakeDatabase.Courses.FirstOrDefault(c => c.Id == courseId);
            return course?.AssignmentGroups?.FirstOrDefault(g => g.Id == groupId);
        }

        private int AssignmentGroupNextKey(Course course)
        {
            if (course.AssignmentGroups != null && course.AssignmentGroups.Any())
                return course.AssignmentGroups.Max(g => g.Id) + 1;
            return 1;
        }

        // ── COPY COURSE ──

        public Course? CopyCourse(int sourceCourseId, int sectionNumber, int year, SemesterType semester)
        {
            var source = FakeDatabase.Courses.FirstOrDefault(c => c.Id == sourceCourseId);
            if (source == null) return null;
            var newId = NextKey;
            var copy = new Course
            {
                Id = newId,
                Name = source.Name,
                Code = source.Code,
                Description = source.Description,
                SectionNumber = sectionNumber,
                SemesterTaught = new Semester(year, semester),
                Assignments = source.Assignments?.Select(a => new Assignment
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    AvailablePoints = a.AvailablePoints,
                    DueDate = a.DueDate,
                    GroupId = a.GroupId,
                    Submissions = new List<Submission>()
                }).ToList() ?? new List<Assignment>(),
                AssignmentGroups = source.AssignmentGroups?.Select(g => new AssignmentGroup
                {
                    Id = g.Id,
                    CourseId = newId,
                    Name = g.Name,
                    TotalPoints = g.TotalPoints,
                    AssignmentIds = new List<int>(g.AssignmentIds)
                }).ToList() ?? new List<AssignmentGroup>(),
                Modules = source.Modules?.Select(m => new Module
                {
                    Id = m.Id,
                    ModuleName = m.ModuleName,
                    Content = m.Content != null ? new List<string>(m.Content) : new List<string>(),
                    ModuleContents = m.ModuleContents?.Select(c => c switch
                    {
                        AssignmentContent ac => (ModuleContent)new AssignmentContent { Id = ac.Id, AssignmentId = ac.AssignmentId, Name = ac.Name },
                        FileContent fc => new FileContent { Id = fc.Id, Name = fc.Name, FilePath = fc.FilePath },
                        PageContent pc => new PageContent { Id = pc.Id, Name = pc.Name },
                        _ => null
                    }).Where(c => c != null).ToList() ?? new List<ModuleContent>()
                }).ToList() ?? new List<Module>(),
                Announcements = source.Announcements?.Select(a => new Announcement
                {
                    Id = a.Id,
                    Title = a.Title,
                    Body = a.Body,
                    PostedDate = a.PostedDate
                }).ToList() ?? new List<Announcement>(),
                Roster = new List<Student>()
            };
            FakeDatabase.Courses.Add(copy);
            return copy;
        }

        public Submission? GetStudentSubmission(int courseId, int assignmentId, int studentId)
        {
            return FakeDatabase.Courses.FirstOrDefault(i => i.Id == courseId).Assignments.FirstOrDefault(i => i.Id == assignmentId).Submissions?
                .FirstOrDefault(s => s.StudentId == studentId);
        }
    }
}
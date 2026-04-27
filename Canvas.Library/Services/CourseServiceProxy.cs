using Canvas.Library.Model;
using Newtonsoft.Json;
using Canvas.Library.Utilities;

namespace Canvas.Library.Services
{
    public class CourseServiceProxy
    {
        private List<Course>? _courses;
        private WebRequestHandler _handler = new WebRequestHandler();

        public List<Course> Courses
        {
            get
            {
                if (_courses == null)
                {
                    var result = _handler.Get("/course").Result;
                    var settings = new JsonSerializerSettings();
                    settings.Converters.Add(new ModuleContentConverter());
                    _courses = JsonConvert.DeserializeObject<List<Course>>(result, settings)
                            ?? new List<Course>();
                }
                return _courses;
            }
        }

        private static CourseServiceProxy? instance;
        private static object instanceLock = new object();

        public static CourseServiceProxy Current
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                        instance = new CourseServiceProxy();
                }
                return instance;
            }
        }

        // ── SEMESTER ──

        public Course? UpdateSemesterDates(int courseId, DateTime? startDate, DateTime? endDate)
        {
            var body = new { StartDate = startDate, EndDate = endDate };
            var result = _handler.Put($"/course/{courseId}/semester-dates", body).Result;
            if (result == "ERROR" || string.IsNullOrWhiteSpace(result)) return null;
            _courses = null;
            return JsonConvert.DeserializeObject<Course>(result);
        }

        public List<Assignment> ImportAssignments(int courseId, List<Assignment> assignments)
        {
            var result = _handler.Post($"/course/{courseId}/assignments/import", assignments).Result;
            if (result == "ERROR" || string.IsNullOrWhiteSpace(result)) return new List<Assignment>();
            _courses = null;
            return JsonConvert.DeserializeObject<List<Assignment>>(result) ?? new List<Assignment>();
        }

        public string AddOrUpdate(Course? course)
        {
            if (course == null) return null;
            var result = _handler.Post("/course", course).Result;
            if (result == "ERROR" || string.IsNullOrWhiteSpace(result)) return result;
            var updated = JsonConvert.DeserializeObject<Course>(result);
            _courses = null;
            return result;
        }

        public Course? Delete(Course? course)
        {
            if (course == null) return null;
            _handler.Delete($"/course/{course.Id}").Wait();
            _courses = null;
            return course;
        }

        public void InvalidateCache()
        {
            _courses = null;
        }

        public void AddModule(int courseId, string moduleName)
        {
            if (moduleName == null) return;
            _handler.Post($"/course/{courseId}/modules", moduleName).Wait();
            _courses = null;
        }

        public bool AddModuleContent(int courseId, int moduleId, string newContent)
        {
            var result = _handler.Post($"/course/{courseId}/modules/{moduleId}/content", newContent).Result;
            _courses = null;
            return result != "ERROR";
        }

        public bool AddModuleContents(int courseId, int moduleId, ModuleContent content)
        {
            var result = _handler.Post($"/course/{courseId}/modules/{moduleId}/contents", content).Result;
            _courses = null;
            return result != "ERROR";
        }

        public void UpdateModuleContent(int courseId, int moduleId, int contentIndex, string newContent)
        {
            var request = new { Index = contentIndex, Content = newContent };
            _handler.Post($"/course/{courseId}/modules/{moduleId}/content/update", request).Wait();
            _courses = null;
        }

        public void DeleteModuleContent(int courseId, int moduleId, int contentIndex)
        {
            _handler.Delete($"/course/{courseId}/modules/{moduleId}/content/{contentIndex}").Wait();
            _courses = null;
        }

        public void DeleteModuleContents(int courseId, int moduleId, int contentId)
        {
            _handler.Delete($"/course/{courseId}/modules/{moduleId}/contents/{contentId}").Wait();
            _courses = null;
        }

        public void DeleteModule(int courseId, int moduleId)
        {
            _handler.Delete($"/course/{courseId}/modules/{moduleId}").Wait();
            _courses = null;
        }

        public void UnenrollStudent(int courseId, int studentId)
        {
            _handler.Delete($"/course/{courseId}/roster/{studentId}").Wait();
            _courses = null;
        }

        public void DeleteAllStudentsSubmissions(int studentId)
        {
            _handler.Delete($"/course/submissions/{studentId}").Wait();
            _courses = null;
        }

        public List<Course> GetCoursesForStudent(int studentId)
        {
            _courses = null;
            var result = _handler.Get($"/course/student/{studentId}").Result;
            if (result == null) return new List<Course>();
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new ModuleContentConverter());
            return JsonConvert.DeserializeObject<List<Course>>(result, settings) ?? new List<Course>();
        }

        public void EnrollStudent(int studentId, int courseId)
        {
            _handler.Post($"/course/{courseId}/roster/{studentId}", new { }).Wait();
            _courses = null;
        }

        public Assignment AddOrUpdateAssignment(int courseId, Assignment assignment)
        {
            var result = _handler.Post($"/course/{courseId}/assignments", assignment).Result;
            _courses = null;
            return JsonConvert.DeserializeObject<Assignment>(result);
        }

        public bool DeleteAssignment(int courseId, int assignmentId)
        {
            var result = _handler.Delete($"/course/{courseId}/assignments/{assignmentId}").Result;
            _courses = null;
            return result != "ERROR";
        }

        public void CopyAssignmentToCourse(int assignmentId, int sourceCourseId, int targetCourseId)
        {
            var request = new { AssignmentId = assignmentId, SourceCourseId = sourceCourseId };
            _handler.Post($"/course/{targetCourseId}/assignments/copy", request).Wait();
            _courses = null;
        }

        public Assignment GetAssignmentById(int courseId, int assignmentId)
        {
            var result = _handler.Get($"/course/{courseId}/assignments/{assignmentId}").Result;
            return JsonConvert.DeserializeObject<Assignment>(result);
        }

        public List<Submission> GetStudentSubmissions(int courseId, int studentId)
        {
            var result = _handler.Get($"/course/{courseId}/submissions/{studentId}").Result;
            return JsonConvert.DeserializeObject<List<Submission>>(result) ?? new List<Submission>();
        }

        public Submission? GetStudentSubmission(int courseId, int assignmentId, int studentId)
        {
            var result = _handler.Get($"/course/{courseId}/submission/{assignmentId}/student/{studentId}").Result;
            return JsonConvert.DeserializeObject<Submission>(result);
        }

        public double CalculateGrade(int courseId, int studentId)
        {
            var result = _handler.Get($"/course/{courseId}/grades/{studentId}").Result;
            return JsonConvert.DeserializeObject<double>(result);
        }

        public string SubmitAssignment(int courseId, Submission submission)
        {
            var result = _handler.Post($"/course/{courseId}/submissions", submission).Result;
            _courses = null;
            return result;
        }

        public void GradeSubmission(int courseId, int assignmentId, int submissionId, int points)
        {
            _handler.Post($"/course/{courseId}/assignments/{assignmentId}/submissions/{submissionId}/grade", points).Wait();
            _courses = null;
        }

        public void AddAnnouncement(int courseId, Announcement announcement)
        {
            _handler.Post($"/course/{courseId}/announcements", announcement).Wait();
            _courses = null;
        }

        public void UpdateAnnouncement(int courseId, Announcement updated)
        {
            _handler.Post($"/course/{courseId}/announcements", updated).Wait();
            _courses = null;
        }

        public void DeleteAnnouncement(int courseId, int announcementId)
        {
            _handler.Delete($"/course/{courseId}/announcements/{announcementId}").Wait();
            _courses = null;
        }

        public string ExportRoster(int courseId)
        {
            return _handler.Get($"/course/{courseId}/roster/export").Result ?? string.Empty;
        }

        public (int added, int skipped, int notFound) ImportRoster(int courseId, string csvContent)
        {
            var result = _handler.Post($"/course/{courseId}/roster/import", csvContent).Result;
            var parsed = JsonConvert.DeserializeObject<ImportRosterResult>(result);
            return (parsed.Added, parsed.Skipped, parsed.NotFound);
        }

        private class ImportRosterResult
        {
            public int Added { get; set; }
            public int Skipped { get; set; }
            public int NotFound { get; set; }
        }

        public AssignmentGroup AddOrUpdateAssignmentGroup(int courseId, AssignmentGroup group)
        {
            var result = _handler.Post($"/course/{courseId}/groups", group).Result;
            _courses = null;
            return JsonConvert.DeserializeObject<AssignmentGroup>(result);
        }

        public bool DeleteAssignmentGroup(int courseId, int groupId)
        {
            var result = _handler.Delete($"/course/{courseId}/groups/{groupId}").Result;
            _courses = null;
            return result != "ERROR";
        }

        public bool AddAssignmentToGroup(int courseId, int groupId, int assignmentId)
        {
            var result = _handler.Post($"/course/{courseId}/groups/{groupId}/assignments/{assignmentId}", new { }).Result;
            _courses = null;
            return result != "ERROR";
        }

        public bool RemoveAssignmentFromGroup(int courseId, int groupId, int assignmentId)
        {
            var result = _handler.Delete($"/course/{courseId}/groups/{groupId}/assignments/{assignmentId}").Result;
            _courses = null;
            return result != "ERROR";
        }

        public List<AssignmentGroup> GetAssignmentGroups(int courseId)
        {
            var result = _handler.Get($"/course/{courseId}/groups").Result;
            return JsonConvert.DeserializeObject<List<AssignmentGroup>>(result) ?? new List<AssignmentGroup>();
        }

        public AssignmentGroup GetAssignmentGroupById(int courseId, int groupId)
        {
            var result = _handler.Get($"/course/{courseId}/groups/{groupId}").Result;
            return JsonConvert.DeserializeObject<AssignmentGroup>(result);
        }

        public Course CopyCourse(int sourceCourseId, int sectionNumber, int year, SemesterType semester, int instructorId)
        {
            var request = new { SectionNumber = sectionNumber, Year = year, Semester = semester, InstructorId = instructorId };
            var result = _handler.Post($"/course/{sourceCourseId}/copy", request).Result;
            _courses = null;
            return JsonConvert.DeserializeObject<Course>(result);
        }

        public Quiz? GetQuiz(int courseId, int assignmentId)
        {
            var result = _handler.Get($"/course/{courseId}/assignments/{assignmentId}/quiz").Result;
            if (result == "ERROR" || string.IsNullOrWhiteSpace(result)) return null;
            return JsonConvert.DeserializeObject<Quiz>(result);
        }

        public Quiz? CreateQuiz(int courseId, int assignmentId, int? timeLimitMinutes, int allowedAttempts)
        {
            var body = new { TimeLimitMinutes = timeLimitMinutes, AllowedAttempts = allowedAttempts };
            var result = _handler.Post($"/course/{courseId}/assignments/{assignmentId}/quiz", body).Result;
            if (result == "ERROR") return null;
            _courses = null;
            return JsonConvert.DeserializeObject<Quiz>(result);
        }

        public Quiz? UpdateQuiz(int courseId, int assignmentId, int? timeLimitMinutes, int allowedAttempts)
        {
            var body = new { TimeLimitMinutes = timeLimitMinutes, AllowedAttempts = allowedAttempts };
            var result = _handler.Put($"/course/{courseId}/assignments/{assignmentId}/quiz", body).Result;
            if (result == "ERROR") return null;
            _courses = null;
            return JsonConvert.DeserializeObject<Quiz>(result);
        }

        public bool DeleteQuiz(int courseId, int assignmentId)
        {
            var result = _handler.Delete($"/course/{courseId}/assignments/{assignmentId}/quiz").Result;
            _courses = null;
            return result != "ERROR";
        }

        // ── QUIZ QUESTION METHODS ──

        public QuizQuestion? AddQuestion(int courseId, int assignmentId, QuizQuestion question)
        {
            var result = _handler.Post($"/course/{courseId}/assignments/{assignmentId}/quiz/questions", question).Result;
            if (result == "ERROR") return null;
            return JsonConvert.DeserializeObject<QuizQuestion>(result);
        }

        public QuizQuestion? UpdateQuestion(int courseId, int assignmentId, int questionId, string questionText, int points)
        {
            var body = new { QuestionText = questionText, Points = points };
            var result = _handler.Put($"/course/{courseId}/assignments/{assignmentId}/quiz/questions/{questionId}", body).Result;
            if (result == "ERROR") return null;
            return JsonConvert.DeserializeObject<QuizQuestion>(result);
        }

        public bool DeleteQuestion(int courseId, int assignmentId, int questionId)
        {
            var result = _handler.Delete($"/course/{courseId}/assignments/{assignmentId}/quiz/questions/{questionId}").Result;
            return result != "ERROR";
        }

        // ── QUIZ QUESTION OPTION METHODS ──

        public QuizQuestionOption? AddOption(int courseId, int assignmentId, int questionId, QuizQuestionOption option)
        {
            var result = _handler.Post($"/course/{courseId}/assignments/{assignmentId}/quiz/questions/{questionId}/options", option).Result;
            if (result == "ERROR") return null;
            return JsonConvert.DeserializeObject<QuizQuestionOption>(result);
        }

        public QuizQuestionOption? UpdateOption(int courseId, int assignmentId, int questionId, int optionId, string optionText, bool isCorrect)
        {
            var body = new { OptionText = optionText, IsCorrect = isCorrect };
            var result = _handler.Put($"/course/{courseId}/assignments/{assignmentId}/quiz/questions/{questionId}/options/{optionId}", body).Result;
            if (result == "ERROR") return null;
            return JsonConvert.DeserializeObject<QuizQuestionOption>(result);
        }

        public bool DeleteOption(int courseId, int assignmentId, int questionId, int optionId)
        {
            var result = _handler.Delete($"/course/{courseId}/assignments/{assignmentId}/quiz/questions/{questionId}/options/{optionId}").Result;
            return result != "ERROR";
        }

        public List<LetterGrade> GetGradeScale(int courseId)
        {
            var result = _handler.Get($"/course/{courseId}/gradescale").Result;
            if (result == "ERROR" || string.IsNullOrWhiteSpace(result)) return new List<LetterGrade>();
            return JsonConvert.DeserializeObject<List<LetterGrade>>(result) ?? new List<LetterGrade>();
        }

        public LetterGrade? AddOrUpdateLetterGrade(int courseId, LetterGrade grade)
        {
            var result = _handler.Post($"/course/{courseId}/gradescale", grade).Result;
            if (result == "ERROR") return null;
            _courses = null;
            return JsonConvert.DeserializeObject<LetterGrade>(result);
        }

        public bool DeleteLetterGrade(int courseId, int gradeId)
        {
            var result = _handler.Delete($"/course/{courseId}/gradescale/{gradeId}").Result;
            _courses = null;
            return result != "ERROR";
        }

        public string GetLetterGradeForScore(int courseId, double percentage)
        {
            var result = _handler.Get($"/course/{courseId}/gradescale/calculate?percentage={percentage}").Result;
            return result?.Trim('"') ?? "N/A";
        }

        public List<AssignmentComment> GetComments(int submissionId)
        {
            var result = _handler.Get($"/course/submissions/{submissionId}/comments").Result;
            if (result == null || result == "ERROR") return new List<AssignmentComment>();
            return JsonConvert.DeserializeObject<List<AssignmentComment>>(result) ?? new List<AssignmentComment>();
        }

        public AssignmentComment? AddComment(int submissionId, AssignmentComment comment)
        {
            comment.SubmissionId = submissionId;
            var result = _handler.Post($"/course/submissions/{submissionId}/comments", comment).Result;
            if (result == "ERROR" || string.IsNullOrWhiteSpace(result)) return null;
            return JsonConvert.DeserializeObject<AssignmentComment>(result);
        }

        public bool DeleteComment(int submissionId, int commentId)
        {
            var result = _handler.Delete($"/course/submissions/{submissionId}/comments/{commentId}").Result;
            return result != "ERROR";
        }

        public async Task<(string filePath, string mimeType)> UploadFileAsync(Stream stream, string fileName, string mimeType)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5258");

            var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mimeType);
            content.Add(streamContent, "file", fileName);

            var response = await client.PostAsync("/upload", content);
            if (!response.IsSuccessStatusCode) return (string.Empty, string.Empty);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UploadResult>(json);
            return (result?.FilePath ?? string.Empty, result?.MimeType ?? string.Empty);
        }

        private class UploadResult
        {
            public string FilePath { get; set; } = string.Empty;
            public string MimeType { get; set; } = string.Empty;
        }

    }
}
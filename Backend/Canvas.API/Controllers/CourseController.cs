using Microsoft.AspNetCore.Mvc;
using Canvas.Library.Model;
using Canvas.API.Enterprise;
using System.Security.Cryptography;

namespace Canvas.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CourseController : ControllerBase
    {
        private CourseEC _ec = new CourseEC();

        [HttpGet] public IEnumerable<Course> GetAll() => _ec.GetAll();
        [HttpGet("{id}")] public Course? GetById(int id) => _ec.GetById(id);
        [HttpPost] public Course? Create([FromBody] Course course) => _ec.Create(course);
        [HttpPut] public Course? Update([FromBody] Course course) => _ec.Update(course);
        [HttpDelete("{id}")] public Course? Delete(int id) => _ec.Delete(id);

        [HttpPost("{id}/copy")] public Course? CopyCourse(int id, [FromBody] CopyCourseRequest r) => _ec.CopyCourse(id, r.SectionNumber, r.Year, r.Semester);

        [HttpGet("{id}/assignments")] public IEnumerable<Assignment>? GetAssignments(int id) => _ec.GetById(id)?.Assignments;
        [HttpGet("{courseId}/assignments/{id}")] public Assignment? GetAssignmentById(int courseId, int id) => _ec.GetAssignmentById(courseId, id);
        [HttpPost("{courseId}/assignments")] public Assignment? AddOrUpdateAssignment(int courseId, [FromBody] Assignment assignment) => _ec.AddOrUpdateAssignment(courseId, assignment);
        [HttpDelete("{courseId}/assignments/{id}")] public bool DeleteAssignment(int courseId, int id) => _ec.DeleteAssignment(courseId, id);
        [HttpPost("{courseId}/assignments/copy")] public void CopyAssignment(int courseId, [FromBody] CopyAssignmentRequest r) => _ec.CopyAssignmentToCourse(r.AssignmentId, r.SourceCourseId, courseId);

        [HttpGet("{id}/modules")] public IEnumerable<Module>? GetModules(int id) => _ec.GetById(id)?.Modules;
        [HttpPost("{id}/modules")] public void AddModule(int id, [FromBody] string moduleName) => _ec.AddModule(id, moduleName);
        [HttpDelete("{courseId}/modules/{id}")] public void DeleteModule(int courseId, int id) => _ec.DeleteModule(courseId, id);
        [HttpPost("{courseId}/modules/{id}/content")] public bool AddModuleContent(int courseId, int id, [FromBody] string content) => _ec.AddModuleContent(courseId, id, content);

        [HttpPost("{courseId}/modules/{id}/content/update")]
        public void UpdateModuleContent(int courseId, int id, [FromBody] UpdateContentRequest r) 
            => _ec.UpdateModuleContent(courseId, id, r.Index, r.Content);
     
        [HttpDelete("{courseId}/modules/{moduleId}/content/{index}")] public void DeleteModuleContent(int courseId, int moduleId, int index) => _ec.DeleteModuleContent(courseId, moduleId, index);

        [HttpGet("{id}/roster")] public IEnumerable<Student>? GetRoster(int id) => _ec.GetById(id)?.Roster;
        [HttpPost("{courseId}/roster/{studentId}")] public void EnrollStudent(int courseId, int studentId) => _ec.EnrollStudent(studentId, courseId);
        [HttpDelete("{courseId}/roster/{studentId}")] public void UnenrollStudent(int courseId, int studentId) => _ec.UnenrollStudent(courseId, studentId);
        [HttpGet("{id}/roster/export")] public string ExportRoster(int id) => _ec.ExportRoster(id);

        [HttpPost("{id}/roster/import")]
        public ImportRosterResult ImportRoster(int id, [FromBody] string csvContent)
        {
            var (added, skipped, notFound) = _ec.ImportRoster(id, csvContent);
            return new ImportRosterResult { Added = added, Skipped = skipped, NotFound = notFound };
        }

        public class ImportRosterResult
        {
            public int Added { get; set; }
            public int Skipped { get; set; }
            public int NotFound { get; set; }
        }

        [HttpGet("{id}/announcements")] public IEnumerable<Announcement>? GetAnnouncements(int id) => _ec.GetById(id)?.Announcements;
        [HttpPost("{id}/announcements")] public void AddAnnouncement(int id, [FromBody] Announcement announcement) => _ec.AddAnnouncement(id, announcement);
        [HttpPut("{id}/announcements")] public void UpdateAnnouncement(int id, [FromBody] Announcement announcement) => _ec.UpdateAnnouncement(id, announcement);
        [HttpDelete("{courseId}/announcements/{id}")] public void DeleteAnnouncement(int courseId, int id) => _ec.DeleteAnnouncement(courseId, id);

        [HttpGet("student/{studentId}")] public IEnumerable<Course> GetCoursesForStudent(int studentId) => _ec.GetCoursesForStudent(studentId);
        [HttpGet("{courseId}/grades/{studentId}")] public double CalculateGrade(int courseId, int studentId) => _ec.CalculateGrade(courseId, studentId);
        // [HttpPost("{courseId}/submissions")] public void SubmitAssignment(int courseId, [FromBody] Submission submission) => _ec.SubmitAssignment(courseId, submission);
        [HttpPost("{courseId}/submissions")]
        public Submission SubmitAssignment(int courseId, [FromBody] Submission submission)
        {
            _ec.SubmitAssignment(courseId, submission);
            return submission;
        }
        [HttpGet("{courseId}/submission/{assignmentId}/student/{studentId}")] public Submission? GetStudentSubmission(int courseId, int assignmentId, int studentId) => _ec.GetStudentSubmission(courseId, assignmentId, studentId);
        [HttpPost("{courseId}/assignments/{assignmentId}/submissions/{submissionId}/grade")]
        public IActionResult GradeSubmission(int courseId, int assignmentId, int submissionId, [FromBody] int points)
        {
            _ec.GradeSubmission(courseId, assignmentId, submissionId, points);
            return Ok();
        }

        [HttpGet("{id}/groups")] public List<AssignmentGroup> GetAssignmentGroups(int id) => _ec.GetAssignmentGroups(id);
        [HttpGet("{courseId}/groups/{id}")] public AssignmentGroup? GetAssignmentGroupById(int courseId, int id) => _ec.GetAssignmentGroupById(courseId, id);
        [HttpPost("{id}/groups")] public AssignmentGroup? AddOrUpdateAssignmentGroup(int id, [FromBody] AssignmentGroup group) => _ec.AddOrUpdateAssignmentGroup(id, group);
        [HttpDelete("{courseId}/groups/{id}")] public bool DeleteAssignmentGroup(int courseId, int id) => _ec.DeleteAssignmentGroup(courseId, id);
        [HttpPost("{courseId}/groups/{groupId}/assignments/{assignmentId}")] public bool AddAssignmentToGroup(int courseId, int groupId, int assignmentId) => _ec.AddAssignmentToGroup(courseId, groupId, assignmentId);
        [HttpDelete("{courseId}/groups/{groupId}/assignments/{assignmentId}")] public bool RemoveAssignmentFromGroup(int courseId, int groupId, int assignmentId) => _ec.RemoveAssignmentFromGroup(courseId, groupId, assignmentId);

        [HttpGet("{courseId}/submissions/{studentId}")] public List<Submission> GetStudentSubmissions(int courseId, int studentId) => _ec.GetStudentSubmissions(courseId, studentId);
        [HttpPost("{courseId}/modules/{moduleId}/contents")] public bool AddModuleContents(int courseId, int moduleId, [FromBody] ModuleContent content) => _ec.AddModuleContents(courseId, moduleId, content);
        [HttpDelete("{courseId}/modules/{moduleId}/contents/{contentId}")] public void DeleteModuleContents(int courseId, int moduleId, int contentId) => _ec.DeleteModuleContents(courseId, moduleId, contentId);
    }

    public record CopyCourseRequest(int SectionNumber, int Year, SemesterType Semester);
    public record CopyAssignmentRequest(int AssignmentId, int SourceCourseId);
    public record UpdateContentRequest(int Index, string Content);
}
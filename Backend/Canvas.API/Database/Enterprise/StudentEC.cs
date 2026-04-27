using Canvas.Library.Model;
using Canvas.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Canvas.API.Enterprise
{
    public class StudentEC
    {
        private readonly CanvasDbContext _context;

        public StudentEC(CanvasDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Student> GetAll()
        {
            return _context.Students.ToList();
        }

        public Student? GetById(int id)
        {
            if (id == 0) return null;
            return _context.Students.FirstOrDefault(s => s.Id == id);
        }

        public Student? Create(Student student)
        {
            if (student == null) return null;
            if (student.Id == 0)
            {
                student.Id = 0;
                _context.Students.Add(student);
                _context.SaveChanges();
            }
            return student;
        }

        public Student? Update(Student student)
        {
            var existing = _context.Students.FirstOrDefault(s => s.Id == student.Id);
            if (existing == null) return null;
            existing.Name = student.Name;
            existing.Code = student.Code;
            existing.Classification = student.Classification;
            existing.Email = student.Email; // add this
            _context.SaveChanges();
            return existing;
        }

        public Student? Delete(int id)
        {
            var student = _context.Students.FirstOrDefault(s => s.Id == id);
            if (student == null) return null;

            // remove from all course rosters
            var courses = _context.Courses
                .Include(c => c.Roster)
                .Include(c => c.Assignments).ThenInclude(a => a.Submissions)
                .ToList() // materialize first
                .Where(c => c.Roster != null && c.Roster.Any(s => s.Id == id))
                .ToList();

            foreach (var course in courses)
            {
                var studentInRoster = course.Roster?.FirstOrDefault(s => s.Id == id);
                if (studentInRoster != null)
                    course.Roster.Remove(studentInRoster);

                course.Assignments?.ForEach(a =>
                {
                    var subs = a.Submissions?.Where(s => s.StudentId == id).ToList();
                    subs?.ForEach(s => a.Submissions.Remove(s));
                });
            }

            _context.Students.Remove(student);
            _context.SaveChanges();
            return student;
        }
    }
}
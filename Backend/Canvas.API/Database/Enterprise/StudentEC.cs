

using Canvas.Library.Model;

namespace Canvas.API.Enterprise
{
    public class StudentEC
    {
        public StudentEC() {}

        public IEnumerable<Student> GetAll()
        {
            return FakeDatabase.Students;
        }

        public Student? GetById(int ID)
        {
            if(ID == 0)
            {
                return null;
            }
            return FakeDatabase.Students.FirstOrDefault(i => i.Id == ID);
        }

        public Student? Create(Student student)
        {
            if (student == null) return null;

            if (student.Id == 0)
            {
                student.Id = NextKey; 
                FakeDatabase.Students.Add(student);
            }
            return student;
        }

        public int NextKey
        {
            get
            {
                if(FakeDatabase.Students.Any()){
                    return FakeDatabase.Students.Select(i => i.Id).Max() + 1;
                }
                return 1;
            }
        }

        public Student? Update(Student student)
        {
            var existingStudent = FakeDatabase.Students.FirstOrDefault(s => s.Id == student.Id);

            if (existingStudent != null)
            {
                existingStudent.Name = student.Name;
                existingStudent.Code = student.Code;
                existingStudent.Classification = student.Classification;
            }
            return student;
        }

        public Student? Delete(int id)
        {
            Student? studentToRemove = FakeDatabase.Students.FirstOrDefault(s => s.Id == id);
            if (studentToRemove == null) return null;

            // remove from all course rosters
            foreach (var course in FakeDatabase.Courses)
            {
                var studentInRoster = course.Roster?.FirstOrDefault(s => s.Id == id);
                if (studentInRoster != null)
                    course.Roster.Remove(studentInRoster);

                // delete all their submissions
                course.Assignments?.ForEach(a =>
                {
                    var index = a.Submissions?.FindIndex(s => s.StudentId == id) ?? -1;
                    if (index >= 0)
                        a.Submissions.RemoveAt(index);
                });
            }

            FakeDatabase.Students.Remove(studentToRemove);
            return studentToRemove;
        }
    }
}
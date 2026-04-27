using Canvas.Library.Model;
using Canvas.Library.Utilities;
using Newtonsoft.Json;

namespace Canvas.Library.Services
{
    public class StudentServiceProxy
    {
        public Student CurrentStudent { get; set; }
        private List<Student>? _students;
        private WebRequestHandler _handler = new WebRequestHandler();

        public List<Student> Students
        {
            get
            {
                if (_students == null)
                {
                    var result = _handler.Get("/student").Result;
                    _students = JsonConvert.DeserializeObject<List<Student>>(result) ?? new List<Student>();
                }
                return _students;
            }
        }

        private static StudentServiceProxy? instance;
        private static object instanceLock = new object();

        public static StudentServiceProxy Current
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                        instance = new StudentServiceProxy();
                }
                return instance;
            }
        }

        private StudentServiceProxy() { }

        public void ProxyAs(int id)
        {
            CurrentStudent = Students.FirstOrDefault(s => s.Id == id);
        }

        public Student? GetById(int id)
        {
            if (id == 0) return null;
            var result = _handler.Get($"/student/{id}").Result;
            if (result == null) return null;
            return JsonConvert.DeserializeObject<Student>(result);
        }

        public void AddOrUpdate(Student? student)
        {
            if (student == null) return;
            if (student.Id == 0)
                _handler.Post("/student", student).Wait();
            else
                _handler.Put("/student", student).Wait();
            _students = null;
        }

        public Task<Student> DeleteStudent(int studentId)
        {
            var student = Students.FirstOrDefault(s => s.Id == studentId);
            if (student == null) return Task.FromResult<Student>(null);
            _handler.Delete($"/student/{studentId}").Wait();
            _students = null;
            return Task.FromResult(student);
        }
    }
}

// using Canvas.Library.Model;
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Data.Common;
// using System.Net;
// using System.Net.ServerSentEvents;
// using System.Text;

// namespace Canvas.Library.Services
// {
//     public class StudentServiceProxy
//     {
//         public Student CurrentStudent{get; set;}
//         private List<Student> students;

//         public List<Student> Students
//         {
//             get
//             {
//                 return students;
//             }

//             set
//             {
//                 if(students != value)
//                 {
//                     students = value;
//                 }
//             }
//         }
//         private static StudentServiceProxy? instance;
//         private static object instanceLock = new object();

//         public static StudentServiceProxy Current
//         {
//             get
//             {
//                 lock (instanceLock)
//                 {
//                     if(instance == null)
//                     {
//                         instance = new StudentServiceProxy();
//                     }
//                 }
//                 return instance;
//             }
//         }
//         private StudentServiceProxy()
//         {
//             students = new List<Student>
//             {
//                 new Student { Id = 1,  Code = "cr24", Name = "Carter Rudolph", Classification = Classification.Sophomore },
//                 new Student { Id = 2,  Code = "jd24", Name = "John Doe", Classification = Classification.Junior },
//                 new Student { Id = 3,  Code = "js25", Name = "Jane Smith", Classification = Classification.Sophomore },
//                 new Student { Id = 4,  Code = "ab26", Name = "Alice Brown", Classification = Classification.Freshman },
//                 new Student { Id = 5,  Code = "bc24", Name = "Bob Clark", Classification = Classification.Senior },
//                 new Student { Id = 6,  Code = "dm25", Name = "Diana Miller", Classification = Classification.Sophomore },
//                 new Student { Id = 7,  Code = "ew26", Name = "Ethan Wright", Classification = Classification.Freshman },
//                 new Student { Id = 8,  Code = "fl24", Name = "Fiona Lewis", Classification = Classification.Junior },
//                 new Student { Id = 9,  Code = "gh25", Name = "George Hall", Classification = Classification.Sophomore },
//                 new Student { Id = 10, Code = "iy26", Name = "Isabel Young", Classification = Classification.Freshman }
//             };
//         }

//         public void ProxyAs(int id) {
//             CurrentStudent = Students.FirstOrDefault(s => s.Id == id);
//         }

//         public int NextKey
//         {
//             get
//             {
//                 if(Students.Any()){
//                     return Students.Select(i => i.Id).Max() + 1;
//                 }
//                 return 1;
//             }
//         }

//         public Student? GetById(int ID)
//         {
//             if(ID == 0)
//             {
//                 return null;
//             }
//             return Students.FirstOrDefault(i => i.Id == ID);
//         }

//         public void AddOrUpdate(Student? student)
//         {
//             if (student == null) return;

//             if (student.Id == 0)
//             {
//                 student.Id = NextKey; 
//                 Students.Add(student);
//             }
//             else
//             {
//                 var existingStudent = Students.FirstOrDefault(s => s.Id == student.Id);

//                 if (existingStudent != null)
//                 {
//                     existingStudent.Name = student.Name;
//                     existingStudent.Code = student.Code;
//                     existingStudent.Classification = student.Classification;
//                 }
//             }
//         }

//         public Task<Student> DeleteStudent(int studentID)
//         {
//             CourseServiceProxy.Current.UnenrollStudentFromAllCourses(studentID);
//             int index = Students.FindIndex(s => s.Id == studentID);
//             if (index >= 0){
//                 var student = new Student(Students[index]);
//                 Students.RemoveAt(index);
//                 return Task.FromResult(student);
//             }
//             return Task.FromResult<Student>(null);
//         }
//     }
// }
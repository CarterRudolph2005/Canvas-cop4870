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
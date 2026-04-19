using Microsoft.AspNetCore.Mvc;
using Canvas.Library.Model;
using Canvas.API.Enterprise;
using Canvas.API.Data;

namespace Canvas.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {
        private StudentEC _ec;

        public StudentController(CanvasDbContext context)
        {
            _ec = new StudentEC(context);
        }

        [HttpGet]
        public IEnumerable<Student> GetAll()
        {
            return _ec.GetAll();
        }

        [HttpGet("{id}")]
        public Student? GetById(int id)
        {
            return _ec.GetById(id);
        }

        [HttpPost]
        public Student? Create([FromBody] Student student)
        {
            return _ec.Create(student);
        }

        [HttpPut]
        public Student? Update([FromBody] Student student)
        {
            return _ec.Update(student);
        }

        [HttpDelete("{id}")]
        public Student? Delete(int id)
        {
            return _ec.Delete(id);
        }
        
    }
}
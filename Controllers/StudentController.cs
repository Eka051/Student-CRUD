using Microsoft.AspNetCore.Mvc;
using Student_CRUD.Helpers;
using Student_CRUD.Models;

namespace Student_CRUD.Controllers
{
    public class StudentController : Controller
    {
        private readonly string _constr;

        public StudentController(IConfiguration configuration)
        {
            _constr = configuration.GetConnectionString("WebApiDatabase");
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("api/student/all")]
        public ActionResult<IEnumerable<Student>> FindAll()
        {
            var context = new StudentContext(_constr);
            var students = context.FindAll();
            return Ok(students);
        }

        [HttpGet("api/student/{student_id}")]
        public ActionResult<Student> FindById(int student_id)
        {
            var context = new StudentContext(_constr);
            var student = context.FindById(student_id);
            if (student == null)
            {
                return NotFound();
            }
            return Ok(student);
        }

        [HttpPost("api/student/create")]
        public ActionResult<Student> Create(Student student)
        {
            if (student == null)
            {
                return BadRequest();
            }

            var context = new StudentContext(_constr);
            var createdStudent = context.Create(student);
            return Ok(createdStudent);
        }

        [HttpPut("api/student/update/{student_id}")]
        public ActionResult<Student> Update(int student_id, Student student)
        {
            if (student_id <= 0 || student == null)
            {
                return BadRequest("ID invalid");
            }

            var context = new StudentContext(_constr);
            var existingStudent = context.FindById(student_id);
            if (existingStudent == null)
            {
                return NotFound($"Student with ID {student_id} not found");
            }
            if (student.student_id != 0 && student.student_id != student_id)
            {
                return BadRequest("Mismatch between student ID in URL and request body.");
            }
            var updatedStudent = context.Update(student_id, student);
            return Ok(updatedStudent);
        }

        [HttpDelete("api/delete/{student_id}")]
        public ActionResult Delete(int student_id)
        {
            if (student_id <= 0)
            {
                return BadRequest();
            }

            var context = new StudentContext(_constr);
            var student = context.FindById(student_id);
            if (student == null)
            {
                return NotFound();
            }

            var delete = context.Delete(student);
            return Ok(delete);
        }
    }
}

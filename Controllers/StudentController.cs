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

        [HttpPost("api/student")]
        public ActionResult<Student> Create(Student student)
        {
            if (student == null)
            {
                return BadRequest();
            }

            var context = new StudentContext(_constr);
            var createdStudent = context.Create(student);
            return CreatedAtAction(nameof(FindById), new { student_id = createdStudent.student_id }, createdStudent);
        }

        [HttpPut("api/student/{student_id}")]
        public ActionResult<Student> Update(int student_id, Student student)
        {
            if (student_id <= 0 || student == null)
            {
                return BadRequest();
            }

            var context = new StudentContext(_constr);
            if (context.FindById(student_id) == null)
            {
                return NotFound();
            }

            var updatedStudent = context.Update(student);
            return Ok(updatedStudent);
        }

        [HttpDelete("{student_id}")]
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

            context.Delete(student);
            return NoContent();
        }
    }
}

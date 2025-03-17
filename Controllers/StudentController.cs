using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Npgsql;
using Student_CRUD.Helpers;
using Student_CRUD.Models;

namespace Student_CRUD.Controllers
{
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly string _constr;

        public StudentController(IConfiguration configuration)
        {
            _constr = configuration.GetConnectionString("WebApiDatabase");
        }

        [HttpGet("all")]
        public ActionResult<IEnumerable<Student>> FindAll()
        {
            var context = new StudentContext(_constr);
            var students = context.FindAll();
            return Ok(students);
        }

        [HttpGet("{student_id}")]
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

        [HttpPost("create")]
        public ActionResult<Student> Create([FromBody] Student student)
        {
            if (student == null)
            {
                return BadRequest();
            }

            var context = new StudentContext(_constr);
            var createdStudent = context.Create(student);
            return Ok(createdStudent);
        }

        [HttpPut("update/{student_id}")]
        public ActionResult<Student> Update(int student_id, [FromBody] Student student)
        {
            if (student_id <= 0 || student == null)
            {
                return BadRequest("Invalid student data or ID");
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

        [HttpDelete("delete/{student_id}")]
        public ActionResult Delete(int student_id)
        {
            if (student_id <= 0)
            {
                return BadRequest("Invalid Student ID");
            }

            var context = new StudentContext(_constr);
            var student = context.FindById(student_id);
            if (student == null)
            {
                return NotFound($"Student with ID {student_id} Not Found");
            }

            context.Delete(student);
            return NoContent();
        }
    }
    public class StudentContext
    {
        private readonly string constr;

        public StudentContext(string constr)
        {
            this.constr = constr;
        }

        public List<Student> FindAll()
        {
            List<Student> students = new List<Student>();
            SqlDBHelper dbHelper = new SqlDBHelper(constr);
            string query = "SELECT * FROM students";

            try
            {
                NpgsqlCommand cmd = dbHelper.GetCommand(query);
                NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DateOnly dateOfBirth = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("date_of_birth")));
                    students.Add(new Student
                    {
                        student_id = int.Parse(reader["student_id"].ToString()),
                        name = (string)reader["name"],
                        email = (string)reader["email"],
                        dateOfBirth = dateOfBirth
                    });

                }
                cmd.Dispose();
                dbHelper.CloseConnection();
            }
            catch (Exception e)
            {
                dbHelper.CloseConnection();
                throw new Exception(e.Message);
            }
            return students;
        }

        public Student FindById(int student_id)
        {
            SqlDBHelper dbHelper = new SqlDBHelper(constr);
            string query = "SELECT * FROM students WHERE student_id = @id";
            try
            {
                NpgsqlCommand cmd = dbHelper.GetCommand(query);
                cmd.Parameters.AddWithValue("@id", student_id);
                NpgsqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    Student student = new Student
                    {
                        student_id = int.Parse(reader["student_id"].ToString()),
                        name = (string)reader["name"],
                        email = (string)reader["email"],
                        dateOfBirth = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("date_of_birth")))
                    };
                    return student;

                }
                cmd.Dispose();
                dbHelper.CloseConnection();
            }
            catch (Exception e)
            {
                dbHelper.CloseConnection();
                throw new Exception(e.Message);
            }
            return null;
        }

        public Student Create([FromBody] Student student)
        {
            SqlDBHelper dbHelper = new SqlDBHelper(constr);
            string query = "INSERT INTO students (name, email, date_of_birth) VALUES (@name, @email, @dateOfBirth) RETURNING student_id";
            try
            {
                NpgsqlCommand cmd = dbHelper.GetCommand(query);
                cmd.Parameters.AddWithValue("@name", student.name);
                cmd.Parameters.AddWithValue("@email", student.email);
                cmd.Parameters.AddWithValue("@dateOfBirth", student.dateOfBirth);
                student.student_id = (int)cmd.ExecuteScalar();
                cmd.Dispose();
                dbHelper.CloseConnection();
                return student;
            }
            catch (Exception e)
            {
                dbHelper.CloseConnection();
                throw new Exception(e.Message);
            }
        }

        public Student Update(int student_id, [FromBody] Student student)
        {
            SqlDBHelper dbHelper = new SqlDBHelper(constr);
            string query = "UPDATE students SET name = @name, email = @email, date_of_birth = @dateOfBirth WHERE student_id = @student_id";
            try
            {
                NpgsqlCommand cmd = dbHelper.GetCommand(query);
                cmd.Parameters.AddWithValue("@name", student.name);
                cmd.Parameters.AddWithValue("@email", student.email);
                cmd.Parameters.AddWithValue("@dateOfBirth", student.dateOfBirth);
                cmd.Parameters.AddWithValue("@student_id", student_id);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                dbHelper.CloseConnection();
                return student;
            }
            catch (Exception e)
            {
                dbHelper.CloseConnection();
                throw new Exception(e.Message);
            }
        }

        public Student Delete(Student student)
        {
            SqlDBHelper dbHelper = new SqlDBHelper(constr);
            string query = "DELETE FROM students WHERE student_id = @id";
            try
            {
                NpgsqlCommand cmd = dbHelper.GetCommand(query);
                cmd.Parameters.AddWithValue("@id", student.student_id);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                dbHelper.CloseConnection();
                return student;
            }
            catch (Exception e)
            {
                dbHelper.CloseConnection();
                throw new Exception(e.Message);
            }
        }
    }
}

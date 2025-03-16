using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Student_CRUD.Helpers;

namespace Student_CRUD.Models
{
    public class StudentContext
    {
        private string constr;

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

        public Student Create(Student student)
        {
            SqlDBHelper dbHelper = new SqlDBHelper(constr);
            string query = "INSERT INTO students (name, email, date_of_birth) VALUES (@name, @email, @dateOfBirth)";
            try
            {
                NpgsqlCommand cmd = dbHelper.GetCommand(query);
                cmd.Parameters.AddWithValue("@name", student.name);
                cmd.Parameters.AddWithValue("@email", student.email);
                cmd.Parameters.AddWithValue("@dateOfBirth", student.dateOfBirth);
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

        public Student Update(int student_id, Student student)
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

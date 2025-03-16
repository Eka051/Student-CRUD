namespace Student_CRUD.Models
{
    public class Student
    {
        public int student_id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public DateOnly dateOfBirth { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.AspNetCore.Http;

namespace StudentExerciseMVC.Models.ViewModels
{
    public class StudentInstructorViewModel
    {
        public IEnumerable<Student> Students { get; set; }
        public IEnumerable<Instructor> Instructors { get; set; }

        private string _connectionString;

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }

        public StudentInstructorViewModel(string connectionString)
        {
            _connectionString = connectionString;
            GetAllStudents();
            GetAllInstructors();
        }

        private void GetAllStudents()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT
                            Id,
                            StudentFirstName,
                            StudentLastName,
                            StudentSlackHandle
                        FROM Student";
                    SqlDataReader reader = cmd.ExecuteReader();

                   List<Student> students = new List<Student>();
                    while (reader.Read())
                    {
                        students.Add(new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            StudentFirstName = reader.GetString(reader.GetOrdinal("StudentFirstName")),
                            StudentLastName = reader.GetString(reader.GetOrdinal("StudentLastName")),
                            StudentSlackHandle = reader.GetString(reader.GetOrdinal("StudentSlackHandle")),
                        }
                        );
                    }

                    reader.Close();
                }
            }
        }

        private void GetAllInstructors()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            SELECT
                            Id,
                            InstructorFirstName,
                            InstructorLastName,
                            InstructorSlackHandle
                        FROM Instructor";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Instructor> instructors = new List<Instructor>();

                    while (reader.Read())
                    {
                        instructors.Add(new Instructor
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            InstructorFirstName = reader.GetString(reader.GetOrdinal("InstructorFirstName")),
                            InstructorLastName = reader.GetString(reader.GetOrdinal("InstructorLastName")),
                            InstructorSlackHandle = reader.GetString(reader.GetOrdinal("InstructorSlackHandle")),
                        }
                        );
                    }

                    reader.Close();
                }
            }
        }
    }
}

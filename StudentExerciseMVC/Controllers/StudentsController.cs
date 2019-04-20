using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StudentExerciseMVC.Models;
using StudentExerciseMVC.Models.ViewModels;


namespace StudentExerciseMVC.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IConfiguration _configuration;

        public StudentsController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            }
        }
        //    **********************************
        //              GET: Students
        //    **********************************
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT s.Id,
                                            s.StudentFirstName,
                                            s.StudentLastName,
                                            s.StudentSlackHandle,
                                            s.CohortId,
                                            c.CohortName
                                            FROM Student s
                                        LEFT JOIN Cohort c on s.Cohortid = c.id";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Student> students = new List<Student>();

                    while (reader.Read())
                    {
                        Student student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            StudentFirstName = reader.GetString(reader.GetOrdinal("StudentFirstName")),
                            StudentLastName = reader.GetString(reader.GetOrdinal("StudentLastName")),
                            StudentSlackHandle = reader.GetString(reader.GetOrdinal("StudentSlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Cohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                            }
                        };

                        students.Add(student);
                    }

                    reader.Close();
                    return View(students);
                }
            }
        }

        //    **********************************
        //         GET: Students/Details/5
        //    **********************************
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT s.Id,
                                            s.StudentFirstName,
                                            s.StudentLastName,
                                            s.StudentSlackHandle,
                                            s.CohortId,
                                            c.CohortName,
                                            ei.ExerciseId,
                                            ei.StudentId,
                                            ei.Id AS IntersectionId,
                                            e.ExerciseName,
                                            e.Language
                                        FROM Student s                                        
                                        LEFT JOIN Cohort c ON s.Cohortid = c.Id
                                        LEFT JOIN ExerciseIntersection ei ON s.Id = ei.StudentId
                                        LEFT JOIN Exercise e ON ei.ExerciseId = e.Id
                                        WHERE s.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    Student student = null;

                    while (reader.Read())
                    {
                        if (student == null)
                        {
                            student = new Student
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                StudentFirstName = reader.GetString(reader.GetOrdinal("StudentFirstName")),
                                StudentLastName = reader.GetString(reader.GetOrdinal("StudentLastName")),
                                StudentSlackHandle = reader.GetString(reader.GetOrdinal("StudentSlackHandle")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                Cohort = new Cohort
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                    CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                                }
                            };
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("ExerciseId")))
                        {
                            student.ListofExercises.Add(
                                new Exercise
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    ExerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
                                    Language = reader.GetString(reader.GetOrdinal("Language"))
                                }
                            );
                        }
                    }
                    reader.Close();

                    return View(student);
                }
            }
        }

        //    **********************************
        //          GET: Students/Create
        //    **********************************

        public ActionResult Create()
        {
            {
                StudentCreateViewModel viewModel =
                    new StudentCreateViewModel(_configuration.GetConnectionString("DefaultConnection"));
                return View(viewModel);
            }
        }

        //    **********************************
        //          POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StudentCreateViewModel viewModel)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Student 
                                        (StudentFirstName, StudentLastName, StudentSlackHandle, CohortId)
                                             VALUES (@firstname, @lastname, @slackhandle, @cohortid)";
                        cmd.Parameters.Add(new SqlParameter("@firstname", viewModel.Student.StudentFirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastname", viewModel.Student.StudentLastName));
                        cmd.Parameters.Add(new SqlParameter("@slackhandle", viewModel.Student.StudentSlackHandle));
                        cmd.Parameters.Add(new SqlParameter("@cohortid", viewModel.Student.CohortId));

                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                viewModel.Cohorts = GetAllCohorts();
                return View(viewModel);
            }
        }


        //    **********************************
        //          GET: Students/Edit/5
        //    **********************************
        public ActionResult Edit(int id)
        {

            StudentEditViewModel viewModel = new StudentEditViewModel
            {

                ExerciseIntersectionEditViewModel = new ExerciseIntersectionEditViewModel
                {
                    AssignedExercises = GetAssignedExercises(id),
                    NotAssignedExercises = GetNotAssignedExercises(id),
                    EditedAssignedExercises = null,
                    EditedNotAssignedExercises = null
                },
                StudentDataAndCohortEditViewModel = new StudentDataAndCohortEditViewModel
                {
                    Student = GetStudentById(id),
                    Cohorts = GetAllCohorts()
                },               
            };

            return View(viewModel);
        }


        //    **********************************
        // POST: Student/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, StudentEditViewModel viewModel)
        {
            Student Student = GetStudentById(id);

            using (SqlConnection conn = Connection)
            {

                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Student 
                                            SET StudentFirstname = @firstname, 
                                                StudentLastname = @lastname,
                                                StudentSlackHandle = @slackhandle, 
                                                CohortId = @cohortId
                                            WHERE Id = @id;";

                    cmd.Parameters.Add(new SqlParameter("@firstname", viewModel.StudentDataAndCohortEditViewModel.Student.StudentFirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastname", viewModel.StudentDataAndCohortEditViewModel.Student.StudentLastName));
                    cmd.Parameters.Add(new SqlParameter("@slackhandle", viewModel.StudentDataAndCohortEditViewModel.Student.StudentSlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@cohortId", viewModel.StudentDataAndCohortEditViewModel.Student.CohortId));
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();                   

                    UpdateExerciseIntersections(id, viewModel);

                    return RedirectToAction(nameof(Edit));
                }
            }
        }
               
        private void UpdateExerciseIntersections (int StudentId, StudentEditViewModel viewModel)
        {

            using (SqlConnection conn = Connection)
            {

                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {

                    if (viewModel.ExerciseIntersectionEditViewModel.EditedAssignedExercises != null)
                    {

                        foreach (var item in viewModel.ExerciseIntersectionEditViewModel.EditedAssignedExercises)
                        {

                            int StudentIdToRemove = StudentId;
                            int ExerciseIdToRemove = int.Parse(item);

                            cmd.CommandText = $@"DELETE FROM ExerciseIntersection 
                                                WHERE ExerciseId = {ExerciseIdToRemove}
                                                AND StudentId = {StudentIdToRemove};";

                            cmd.ExecuteNonQuery();
                        }
                    }

                    if (viewModel.ExerciseIntersectionEditViewModel.EditedNotAssignedExercises != null)
                    {

                        foreach (var item in viewModel.ExerciseIntersectionEditViewModel.EditedNotAssignedExercises)
                        {

                            int StudentIdToAdd = StudentId;
                            int ExerciseIdToAdd = int.Parse(item);
                            
                            cmd.CommandText = $@"INSERT INTO ExerciseIntersection (StudentId, ExerciseId)
                                                 OUTPUT INSERTED.Id
                                                 VALUES ({StudentIdToAdd}, {ExerciseIdToAdd});
                                                 SELECT MAX(Id) 
                                                   FROM ExerciseIntersection;";
                            cmd.ExecuteNonQuery();

                        }
                    }
                }
            }
        }

        
        private List<Exercise> GetAssignedExercises(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = $@"SELECT ei.StudentId, ei.ExerciseId, e.Id AS EId, e.ExerciseName, e.Language
                                         FROM ExerciseIntersection ei
                                         LEFT JOIN Exercise e ON e.Id = ei.ExerciseId
                                         WHERE ei.StudentId = {id}";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Exercise> assignedExercises = new List<Exercise>();

                    while (reader.Read())
                    {
                        assignedExercises.Add(new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("EId")),
                            ExerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
                            Language = reader.GetString(reader.GetOrdinal("Language"))
                        });
                    }
                    reader.Close();
                    return assignedExercises;
                }
            }
        }

        private List<Exercise> GetNotAssignedExercises(int id)
        {
            using (SqlConnection conn = Connection)
            {

                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText =$@"SELECT DISTINCT e.Id AS EId, 
                                            e.ExerciseName, 
                                            e.Language
                                        FROM Exercise e
                                        LEFT JOIN ExerciseIntersection ei ON e.Id = ei.ExerciseId
                                            WHERE e.ExerciseName NOT IN (SELECT DISTINCT e.ExerciseName 
							                                            FROM ExerciseIntersection ei
							                                            LEFT JOIN Exercise e ON e.Id = ei.ExerciseId
							                                            WHERE ei.StudentId = {id})";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Exercise> unassignedExercises = new List<Exercise>();

                    while (reader.Read())

                    {
                        unassignedExercises.Add(new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("EId")),
                            ExerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
                            Language = reader.GetString(reader.GetOrdinal("Language"))
                        });
                    }
                    reader.Close();
                    return unassignedExercises;
                }
            }
        }
        
        private List<Cohort> GetAllCohorts()
        {

            using (SqlConnection conn = Connection)
            {

                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = @"SELECT Id, CohortName FROM Cohort;";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Cohort> Cohorts = new List<Cohort>();

                    while (reader.Read())
                    {
                        Cohorts.Add(new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CohortName = reader.GetString(reader.GetOrdinal("CohortName")),
                            ListofStudents = new List<Student>()
                        });
                    }
                    reader.Close();
                    return Cohorts;
                }
            }
        }

        private Student GetStudentById(int id)
        {

            using (SqlConnection conn = Connection)
            {

                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = @"SELECT s.Id AS StudentId, 
                                        s.StudentFirstName, 
                                        s.StudentLastName, 
                                        s.StudentSlackHandle, 
                                        s.CohortId, 
                                        c.CohortName,                                        
                                        e.Id AS ExerciseId, 
                                        e.ExerciseName, 
                                        e.Language
                                    FROM Student s
                                      LEFT JOIN ExerciseIntersection ei ON s.Id = ei.StudentId
                                      LEFT JOIN Exercise e ON e.Id = ei.ExerciseId
                                      LEFT JOIN Cohort c ON c.id =s.CohortId                                      
                                    WHERE s.Id = @id;";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Student student = null;
                    if (reader.Read())
                    {
                        if (student == null)
                        {

                            student = new Student
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("StudentId")),
                                StudentFirstName = reader.GetString(reader.GetOrdinal("StudentFirstName")),
                                StudentLastName = reader.GetString(reader.GetOrdinal("StudentLastName")),
                                StudentSlackHandle = reader.GetString(reader.GetOrdinal("StudentSlackHandle")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                Cohort = new Cohort
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                    CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                                },

                                ListofExercises = new List<Exercise>()
                            };
                        }                        

                        if (!reader.IsDBNull(reader.GetOrdinal("ExerciseId")))
                        {
                            int exerciseId = reader.GetInt32(reader.GetOrdinal("ExerciseId"));

                            if (!student.ListofExercises.Any(e => e.Id == exerciseId))
                            {
                                Exercise newExercise = new Exercise
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ExerciseId")),
                                    ExerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
                                    Language = reader.GetString(reader.GetOrdinal("Language"))
                                };
                                student.ListofExercises.Add(newExercise);
                            }
                        }
                    }
                    reader.Close();
                    return student;
                }
            }
        }


        //    ***********************************
        //      DELETE - GET: Students/Delete/5
        //    ***********************************

        public ActionResult Delete(int id)
        {
            Student student = GetStudentById(id);
            if (student == null)
            {
                return NotFound();
            }
            else
            {
                return View(student);
            }
        }
        //    ***********************************
        //          POST: Student/Delete/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Student student)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM Student WHERE id = @id";

                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View(student);
            }
        }
    }
}
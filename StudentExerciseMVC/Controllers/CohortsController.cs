using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudentExerciseMVC.Models;
using StudentExerciseMVC.Models.ViewModels;

namespace StudentExerciseMVC.Controllers
{
    public class CohortsController : Controller
    {
        private readonly IConfiguration _configuration;

        public CohortsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            }
        }

        // *******************************
                // GET: Cohorts
        // *******************************
        public ActionResult Index()
        {

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id,
                                            c.CohortName
                                            FROM Cohort c";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<cohort> cohorts = new List<cohort>();

                    while (reader.Read())
                    {
                        cohort cohort = new cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                        };

                        cohorts.Add(cohort);
                    }
                    reader.Close();
                    return View(cohorts);
                }
            }
        }

        // *********************************
        //      GET: Cohorts/Details/5
        // *********************************
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id, 
                                               c.CohortName, 
                                               s.id AS StudentId, 
                                               s.StudentFirstname, 
                                               s.StudentLastName, 
                                               s.StudentSlackHandle,
                                               i.id AS InstructorId, 
                                               i.InstructorFirstName,
                                               i.InstructorLastName, 
                                               i.InstructorSlackHandle
                                        FROM Cohort c 
                                        LEFT JOIN Student s on c.Id = s.Cohortid
                                        LEFT JOIN Instructor i on c.Id = i.CohortId
                                        WHERE c.id = @id;";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    cohort cohort = null;

                    while (reader.Read())
                    {
                        if (cohort == null)
                        {
                            cohort = new cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                            };
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("StudentId")))
                        {
                            int studentId = reader.GetInt32(reader.GetOrdinal("StudentId"));
                            if (!cohort.ListofStudents.Any(s => s.Id == studentId))
                            {
                                Student student = new Student
                                {
                                    Id = studentId,
                                    StudentFirstName = reader.GetString(reader.GetOrdinal("StudentFirstName")),
                                    StudentLastName = reader.GetString(reader.GetOrdinal("StudentLastName")),
                                    StudentSlackHandle = reader.GetString(reader.GetOrdinal("StudentSlackHandle")),
                                    CohortId = cohort.Id
                                };
                                cohort.ListofStudents.Add(student);
                            }
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("InstructorId")))
                        {
                            int instructorId = reader.GetInt32(reader.GetOrdinal("InstructorId"));
                            if (!cohort.ListofInstructors.Any(i => i.Id == instructorId))
                            {
                                Instructor instructor = new Instructor
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("InstructorId")),
                                    InstructorFirstName = reader.GetString(reader.GetOrdinal("InstructorFirstName")),
                                    InstructorLastName = reader.GetString(reader.GetOrdinal("InstructorLastName")),
                                    InstructorSlackHandle = reader.GetString(reader.GetOrdinal("InstructorSlackHandle")),
                                    CohortId = cohort.Id
                                };

                                cohort.ListofInstructors.Add(instructor);
                            }
                        }
                    }
                    reader.Close();
                    return View(cohort);
                }
            }
        }

        // *********************************
        //      GET: Cohorts/Create
        // *********************************
        public ActionResult Create()
        {
            CohortCreateViewModel viewModel = new CohortCreateViewModel();

            return View(viewModel);
        }

        // *********************************
        //      POST: Cohorts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CohortCreateViewModel viewModel)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Cohort (CohortName)
                                             VALUES (@cohortName)";
                        cmd.Parameters.Add(new SqlParameter("@cohortName", viewModel.CohortName));

                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {                
                return View(viewModel);
            }
        }


        // *********************************
        //      GET: Cohorts/Edit/5
        // *********************************
        public ActionResult Edit(int id)
        {
            cohort cohort = GetCohortById(id);
            if (cohort == null)
            {
                return NotFound();
            }
            CohortEditViewModel viewModel = new CohortEditViewModel
            {
                 CohortName = cohort.CohortName
                // Cohort = cohort
            };

            return View(viewModel);
            
        }

        // *********************************
        //      POST: Cohorts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, CohortEditViewModel viewModel)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Cohort 
                                           SET Name = @cohortName 
                                           WHERE id = @id;";
                        cmd.Parameters.Add(new SqlParameter("@cohortName", viewModel.CohortName));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();
                        return RedirectToAction(nameof(Index));
                    }
                }          
            
            }
            catch
            {

                return View(viewModel);
            }
        }

        // *********************************
        //      GET: Cohorts/Delete/5
        // *********************************
        public ActionResult Delete(int id)
        {
            cohort cohort = GetCohortById(id);

            CohortDeleteViewModel viewModel = new CohortDeleteViewModel
            {
                CohortName = cohort.CohortName
            };

            return View(viewModel);
        }

        // *********************************
        //      POST: Cohorts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, CohortDeleteViewModel viewModel)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Cohort WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View(viewModel);
            }
        }

        private cohort GetCohortById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, CohortName FROM Cohort
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    cohort cohort = null;

                    while (reader.Read())
                    {
                        cohort = new cohort()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                        };
                    };
                    reader.Close();
                    return cohort;
                }
            }
        }
    }
}
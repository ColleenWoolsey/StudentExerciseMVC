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

        // *********************************************
        // GET: Cohorts WITH Students - This is somewhat
        //  irrelevant as the list needs to go in detail
        //  But practice for dictionary
        // *********************************************

        //[HttpGet]
        //public ActionResult Index()
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"SELECT c.Id AS CohortId, 
        //                                       c.CohortName, 
        //                                       s.Id AS StudentId, 
        //                                       s.StudentFirstname, 
        //                                       s.StudentLastName,
        //                                       s.StudentSlackHandle
        //                                FROM Cohort c 
        //                                LEFT JOIN Student s ON c.Id = s.Cohortid";                

        //            SqlDataReader reader = cmd.ExecuteReader();

        //            Dictionary<int, Cohort> DictWithStudents = new Dictionary<int, Cohort>();

        //            while (reader.Read())
        //            {
        //                int cohortId = reader.GetInt32(reader.GetOrdinal("cohortId"));

        //                if (!DictWithStudents.ContainsKey(cohortId))
        //                {
        //                    Cohort newCohort = new Cohort
        //                    {
        //                        Id = reader.GetInt32(reader.GetOrdinal("cohortId")),
        //                        CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
        //                    };

        //                    DictWithStudents.Add(cohortId, newCohort);
        //                }

        //                if (!reader.IsDBNull(reader.GetOrdinal("StudentId")))
        //                {
        //                    Cohort currentDepartment = DictWithStudents[cohortId];
        //                    currentDepartment.ListofStudents.Add(
        //                        new Student
        //                        {
        //                            Id = reader.GetInt32(reader.GetOrdinal("StudentId")),
        //                            StudentFirstName = reader.GetString(reader.GetOrdinal("StudentFirstName")),
        //                            StudentLastName = reader.GetString(reader.GetOrdinal("StudentLastName")),
        //                            StudentSlackHandle = reader.GetString(reader.GetOrdinal("StudentSlackHandle"))
        //                        }
        //                    );
        //                }                       
        //            }
        //            reader.Close();
        //            return View(DictWithStudents.Values.ToList());
        //        }
        //    }
        //}
        // *****************************************
        //      COHORT LIST WITHOUT STUDENTS    
        // *****************************************

        [HttpGet]
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

                    List<Cohort> cohorts = new List<Cohort>();

                    while (reader.Read())
                    {
                        Cohort cohort = new Cohort
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
                                               s.StudentSlackHandle
                                        FROM Cohort c 
                                        LEFT JOIN Student s ON s.Cohortid = c.Id                                        
                                        WHERE c.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    Cohort cohort = null;

                    while (reader.Read())
                    {
                        if (cohort == null)
                        {
                            cohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                            };
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("StudentId")))
                        {                           
                            cohort.ListofStudents.Add(
                                new Student
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("StudentId")),
                                    StudentFirstName = reader.GetString(reader.GetOrdinal("StudentFirstName")),
                                    StudentLastName = reader.GetString(reader.GetOrdinal("StudentLastName")),
                                    StudentSlackHandle = reader.GetString(reader.GetOrdinal("StudentSlackHandle"))
                                }
                            );                            
                        }
                        // ADDING BOTH LISTS CAUSES DUPLICATE RECORDS
                        //i.id AS InstructorId, 
                        //i.InstructorFirstName,
                        //i.InstructorLastName,
                        //i.InstructorSlackHandle
                        //LEFT JOIN Instructor i ON i.CohortId = c.Id
                        //if (!reader.IsDBNull(reader.GetOrdinal("InstructorId")))
                        //{
                        //    cohort.ListofInstructors.Add(
                        //        new Instructor
                        //        {
                        //            Id = reader.GetInt32(reader.GetOrdinal("InstructorId")),
                        //            InstructorFirstName = reader.GetString(reader.GetOrdinal("InstructorFirstName")),
                        //            InstructorLastName = reader.GetString(reader.GetOrdinal("InstructorLastName")),
                        //            InstructorSlackHandle = reader.GetString(reader.GetOrdinal("InstructorSlackHandle"))
                        //        }
                        //    );
                        //}                        
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
            Cohort cohort = GetCohortById(id);
            if (cohort == null)
            {
                return NotFound();
            }
            CohortEditViewModel viewModel = new CohortEditViewModel
            {
                CohortName = cohort.CohortName
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
                                           SET CohortName = @cohortName 
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
            Cohort cohort = GetCohortById(id);

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

        private Cohort GetCohortById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, CohortName FROM Cohort WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    Cohort cohort = null;

                    while (reader.Read())
                    {
                        cohort = new Cohort()
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
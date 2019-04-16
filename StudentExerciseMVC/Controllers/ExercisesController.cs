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
    public class ExercisesController : Controller
    {
        private readonly IConfiguration _configuration;

        public ExercisesController(IConfiguration configuration)
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


        //    ****************************************************************
        //                   GET: LIST of ALL Exercises
        //    ****************************************************************

        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id,
                                    e.ExerciseName,                                    
                                    e.Language
                                FROM Exercise e";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Exercise> listExercises = new List<Exercise>();

                    while (reader.Read())
                    {
                        Exercise exercise = new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ExerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
                            Language = reader.GetString(reader.GetOrdinal("Language"))
                        };

                        listExercises.Add(exercise);
                    }

                    reader.Close();
                    return View(listExercises);
                }
            }
        }

        //    **********************************
        //         GET: Exercise/Details/5
        //    **********************************
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id,
                                    e.ExerciseName,                                    
                                    e.Language
                                FROM Exercise e                    
                                WHERE e.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    Exercise exercise = null;

                    while (reader.Read())
                    {
                        exercise = new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ExerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
                            Language = reader.GetString(reader.GetOrdinal("Language"))
                        };
                    }
                    reader.Close();

                    return View(exercise);
                }
            }
        }

        //    **********************************
        //       GET: Exercises/Create
        //    **********************************
        public ActionResult Create()
        {
            return View();
        }

        // POST: Exercises/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Exercise exercise)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Exercise(ExerciseName, Language) 
                                            VALUES (@name, @language)";
                        cmd.Parameters.Add(new SqlParameter("@name", exercise.ExerciseName));
                        cmd.Parameters.Add(new SqlParameter("@language", exercise.Language));

                        cmd.ExecuteNonQuery();
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View(exercise);
            }
        }


        //    **********************************
        //      GET: Exercises/Edit/5
        //    **********************************
        public ActionResult Edit(int id)
        {
            Exercise exercise = GetExerciseById(id);
            if (exercise == null)
            {
                return NotFound();
            }

            ExerciseEditViewModel viewModel = new ExerciseEditViewModel
            {
                // Exercise = exercise
                ExerciseName = exercise.ExerciseName,
                Language = exercise.Language
            };

            return View(viewModel);
        }

        //    **********************************
        //      POST: Exercises/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Edit(int id, ExerciseEditViewModel viewModel)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Exercise 
                                           SET ExerciseName = @exerciseName,                                                
                                                   Language = @language
                                           WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@exerciseName", viewModel.ExerciseName));
                        cmd.Parameters.Add(new SqlParameter("@language", viewModel.Language));
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
        //      GET: Exercises/Delete/5
        // *********************************
        public ActionResult Delete(int id)
        {
            Exercise exercise = GetExerciseById(id);
            if (exercise == null)
            {
                return NotFound();
            }
            else
            {
                return View(exercise);
            }
        }

        // *********************************
        //      POST: Exercises/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Exercise exercise)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Exercise WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View(exercise);
            }
        }


        private Exercise GetExerciseById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id,
                                    e.ExerciseName,                                    
                                    e.Language
                                FROM Exercise e                    
                                WHERE e.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Exercise exercise = null;

                    if (reader.Read())
                    {
                        exercise = new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ExerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
                            Language = reader.GetString(reader.GetOrdinal("Language"))
                        };
                    }

                    reader.Close();
                    return exercise;
                }
            }
        }
    }
}
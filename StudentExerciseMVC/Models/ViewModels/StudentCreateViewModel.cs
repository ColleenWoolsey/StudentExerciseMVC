﻿using Microsoft.AspNetCore.Mvc.Rendering;
using StudentExerciseMVC.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExerciseMVC.Models.ViewModels
{
    public class StudentCreateViewModel
    {
        public StudentCreateViewModel()
        {
            Cohorts = new List<cohort>();
        }

        public StudentCreateViewModel(string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT id, CohortName from Cohort;";
                    SqlDataReader reader = cmd.ExecuteReader();

                    Cohorts = new List<cohort>();

                    while (reader.Read())
                    {
                        Cohorts.Add(new cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                        });
                    }
                    reader.Close();
                }
            }
        }


        public Student Student { get; set; }
        public List<cohort> Cohorts { get; set; }

        public List<SelectListItem> CohortOptions
        {
            get
            {
                return Cohorts.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CohortName
                }).ToList();
            }
        }
    }
}
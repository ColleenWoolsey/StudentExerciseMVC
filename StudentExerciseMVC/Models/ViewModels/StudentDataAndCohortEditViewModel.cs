using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudentExerciseMVC.Models.ViewModels
{
    public class StudentDataAndCohortEditViewModel
    {
        public StudentDataAndCohortEditViewModel()
        {

            Cohorts = new List<Cohort>();
            Student = new Student();
        }

        public Student Student { get; set; }
        public List<Cohort> Cohorts { get; set; }

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

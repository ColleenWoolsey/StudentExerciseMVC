using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentExerciseMVC.Models;

namespace StudentExerciseMVC.Models.ViewModels
{
    public class InstructorEditViewModel
    {
        public Instructor Instructor { get; set; }
        public List<cohort> Cohorts { get; set; }
        public List<SelectListItem> CohortOptions
        {
            get
            {
                if (Cohorts == null)
                {
                    return null;
                }

                return Cohorts.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CohortName
                }).ToList();
            }
        }
    }
}

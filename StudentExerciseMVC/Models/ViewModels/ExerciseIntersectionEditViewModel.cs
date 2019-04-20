using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace StudentExerciseMVC.Models.ViewModels
{
    public class ExerciseIntersectionEditViewModel
    {
        public ExerciseIntersectionEditViewModel()
        {
            AssignedExercises = new List<Exercise>();
            NotAssignedExercises = new List<Exercise>();
        }

        public Student Student { get; set; }


        public List<Exercise> AssignedExercises { get; set; }
        public List<Exercise> NotAssignedExercises { get; set; }

        [Display(Name = "")]
        public List<string> EditedAssignedExercises { get; set; }

        [Display(Name = "")]
        public List<string> EditedNotAssignedExercises { get; set; }

        public List<SelectListItem> AssignedExercisesOptions
        {
            get
            {
                return AssignedExercises.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.ExerciseName
                }).ToList();
            }
        }

        public List<SelectListItem> NotAssignedExercisesOptions
        {
            get
            {
                return NotAssignedExercises.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.ExerciseName
                }).ToList();
            }
        }
    }
}
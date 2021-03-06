﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExerciseMVC.Models
{
    public class Exercise
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter the Exercise Name.")]
        [Display(Name = "Exericise Name")]
        public string ExerciseName { get; set; }

        [Required(ErrorMessage = "Enter the Exercise Language.")]
        [Display(Name = "Language")]
        public string Language { get; set; }
    }
}

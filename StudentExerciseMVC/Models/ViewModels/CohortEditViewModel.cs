using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExerciseMVC.Models.ViewModels
{
    public class CohortEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter the Cohort Name in the form of Day or Evening with a number.")]
        [Display(Name = "Cohort")]
        [StringLength(11, MinimumLength = 5)]
        // [RegularExpression(@"(\bday\b|\bDay\b|\bevening\b|\bEvening\b)\s(\b\d{1,2})")]
        public string CohortName { get; set; }

        public Cohort Cohort { get; set; }
    }
}

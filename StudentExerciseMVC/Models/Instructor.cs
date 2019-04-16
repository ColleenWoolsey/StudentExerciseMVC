using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExerciseMVC.Models
{
    public class Instructor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter the Instructor's First Name.")]
        [Display(Name = "First Name")]
        public string InstructorFirstName { get; set; }

        [Required(ErrorMessage = "Enter the Instructor's Last Name.")]
        [Display(Name = "Last Name")]
        public string InstructorLastName { get; set; }

        [Display(Name = "Instructor")]
        public string FullName
        {
            get
            {
                return $"{InstructorFirstName} {InstructorLastName}";
            }
        }

        [Required(ErrorMessage = "Enter the Instructor's Slack Handle.")]
        [Display(Name = "Slack Handle")]
        [StringLength(20, MinimumLength = 3)]
        public string InstructorSlackHandle { get; set; }

        [Required]
        [Display(Name = "Cohort")]
        public int CohortId { get; set; }

        public Cohort Cohort { get; set; }
    }
}

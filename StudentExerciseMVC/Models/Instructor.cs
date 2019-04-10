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

        [Required]
        [Display(Name = "First Name")]
        public string InstructorFirstName { get; set; }

        [Required]
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

        [Required]
        [Display(Name = "SlackHandle")]
        [StringLength(20, MinimumLength = 3)]
        public string InstructorSlackHandle { get; set; }

        [Required]
        [Display(Name = "Cohort")]
        public int CohortId { get; set; }

        public cohort Cohort { get; set; }
    }
}

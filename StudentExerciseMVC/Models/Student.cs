using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExerciseMVC.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string StudentFirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string StudentLastName { get; set; }

        [Display(Name = "Student Name")]
        public string FullName
        {
            get
            {
                return $"{StudentFirstName} {StudentLastName}";
            }
        }

        [Required]
        [Display(Name = "SlackHandle")]
        [StringLength(20, MinimumLength = 3)]
        public string StudentSlackHandle { get; set; }

        [Required]
        [Display(Name = "Cohort")]
        public int CohortId { get; set; }

        public Cohort Cohort { get; set; }

        public List<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}

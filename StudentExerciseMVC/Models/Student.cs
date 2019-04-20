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

        [Required(ErrorMessage = "Enter the Student's First Name.")]
        [Display(Name = "First Name")]
        public string StudentFirstName { get; set; }

        [Required(ErrorMessage = "Enter the Student's Last Name.")]
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

        [Required(ErrorMessage = "Enter the Student's Slack Handle.")]
        [Display(Name = "Slack Handle")]
        [StringLength(20, MinimumLength = 3)]
        public string StudentSlackHandle { get; set; }

        [Required]
        [Display(Name = "Cohort")]
        public int CohortId { get; set; }

        public Cohort Cohort { get; set; } = new Cohort();

        [Display(Name = "Exercise")]
        public List<Exercise> ListofExercises { get; set; } = new List<Exercise>();
    }
}

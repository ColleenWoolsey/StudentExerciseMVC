using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExerciseMVC.Models
{
    public class Cohort
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter the Cohort Name in the form of Day or Evening with a number.")]
        [Display(Name = "Cohort Name")]
        [StringLength(11, MinimumLength = 5)]
        
        // [RegularExpression(@"(\bday\b|\bDay\b|\bevening\b|\bEvening\b)\s(\b\d{1,2})")]
        public string CohortName { get; set; }

        [Display(Name = "Students in Cohort")]
        public List<Student> ListofStudents { get; set; } = new List<Student>();

        [Display(Name = "Instructors in Cohort")]
        public List<Instructor> ListofInstructors { get; set; } = new List<Instructor>();       
    }
}

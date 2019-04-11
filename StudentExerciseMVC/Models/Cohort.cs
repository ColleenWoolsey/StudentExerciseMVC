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
       
        [Required]
        [Display(Name = "Cohort Name")]
        [StringLength(11, MinimumLength = 5)]
        // Message "Cohort name should be in the format of [Day|Evening] [number]"
        // [RegularExpression(@"(\bday\b|\bDay\b|\bevening\b|\bEvening\b)\s(\b\d{1,2})")]
        public string CohortName { get; set; }

        public List<Student> ListofStudents { get; set; } = new List<Student>();

        public List<Instructor> ListofInstructors { get; set; } = new List<Instructor>();       
    }
}

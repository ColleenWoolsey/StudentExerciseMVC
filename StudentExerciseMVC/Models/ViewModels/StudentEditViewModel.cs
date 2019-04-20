using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentExerciseMVC.Models;
using System.Data.SqlClient;

namespace StudentExerciseMVC.Models.ViewModels
{
    public class StudentEditViewModel
    {
        public StudentEditViewModel()
        {

            ExerciseIntersectionEditViewModel = new ExerciseIntersectionEditViewModel();
                
            StudentDataAndCohortEditViewModel = new StudentDataAndCohortEditViewModel();
        }

            public ExerciseIntersectionEditViewModel ExerciseIntersectionEditViewModel { get; set; }
           
            public StudentDataAndCohortEditViewModel StudentDataAndCohortEditViewModel { get; set; }
    }
}
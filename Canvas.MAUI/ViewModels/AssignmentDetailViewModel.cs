using Canvas.Library.Model;
using Canvas.Library.Services;
using Canvas.MAUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Canvas.MAUI.ViewModels
{
    internal class AssignmentDetailViewModel: INotifyPropertyChanged, IQueryAttributable
    {
        //************** Set up ******************************
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public int CourseId => courseId;
        private int courseId;
        public int AssignmentId => assignmentId;
        private int assignmentId;
        
        public AssignmentDetailViewModel()
        {
            AssignmentName = string.Empty;
            AssignmentDescription = string.Empty;
            NewAvailablePoints = 0;
            NewDueDate = DateTime.Today;
            // TodayDate = DateTime.Today;
            ValidationError = string.Empty;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("courseId", out var cId) && int.TryParse(cId?.ToString(), out int _courseId))
                courseId = _courseId;

            if (query.TryGetValue("assignmentId", out var aId) && int.TryParse(aId?.ToString(), out int _assignmentId) && _assignmentId != 0)
            {
                assignmentId = _assignmentId;
                LoadAssignment();
            }
        }

        //************** Assignment Properties ******************************
        public string CourseName {get; set;}
        public string PageTitle => assignmentId == 0 
            ? "New Assignment" : "Edit Assignment";

        private string assignmentName;
        public string AssignmentName
        {
            get => assignmentName;
            set
            { assignmentName = value; OnPropertyChanged(); }
        }

        private string assignmentDescription;
        public string AssignmentDescription
        {
            get => assignmentDescription;
            set{ assignmentDescription = value; OnPropertyChanged(); }
        }
        
        private int newAvailablePoints;
        public int NewAvailablePoints
        {
            get => newAvailablePoints;
            set{ newAvailablePoints = value; OnPropertyChanged(); }
        }

        private DateTime newDueDate;
        public DateTime NewDueDate
        {
            get => newDueDate;
            set{ newDueDate = value; OnPropertyChanged(); }
        }

        private string validationError;
        public string ValidationError
        {
            get => validationError;
            set{ validationError = value; OnPropertyChanged(); }
        }

        private bool hasValidationError;
        public bool HasValidationError
        {
            get => hasValidationError;
            set{ hasValidationError = value; OnPropertyChanged(); }
        }

        //************** Load Page ******************************
        public void LoadAssignment()
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(i => i.Id == CourseId);
            var assignment = course?.Assignments?.FirstOrDefault(i => i.Id == AssignmentId);
            if(assignment != null)
            {
                AssignmentName = assignment.Name;
                AssignmentDescription = assignment.Description;
                NewAvailablePoints = assignment.AvailablePoints;
                NewDueDate = assignment.DueDate;
                CourseName = course.Name;
            }
        }

        //************** Helper Functions ******************************
        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(AssignmentName))
            {
                ValidationError = "Assignment name is required.";
                HasValidationError = true;
                return false;
            }
            if (NewAvailablePoints <= 0)
            {
                ValidationError = "Points must be greater than zero.";
                HasValidationError = true;
                return false;
            }
            ValidationError = string.Empty;
            return true;
        }

        public void Save()
        {
            var assignmentToSave = new Assignment
            {
                Name = AssignmentName,
                Description = AssignmentDescription,
                AvailablePoints = NewAvailablePoints,
                DueDate = newDueDate
            };
            CourseServiceProxy.Current.AddOrUpdateAssignment(CourseId, assignmentToSave);
        }
    }
}
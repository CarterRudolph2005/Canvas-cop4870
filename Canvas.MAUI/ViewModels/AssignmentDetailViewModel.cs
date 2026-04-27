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
    internal class AssignmentDetailViewModel : INotifyPropertyChanged, IQueryAttributable
    {
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
            ValidationError = string.Empty;
            AvailableGroups = new ObservableCollection<AssignmentGroup>();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("courseId", out var cId) && int.TryParse(cId?.ToString(), out int _courseId))
                courseId = _courseId;

            // load groups first so LoadAssignment can pre-select correctly
            LoadGroups();

            if (query.TryGetValue("assignmentId", out var aId) && int.TryParse(aId?.ToString(), out int _assignmentId) && _assignmentId != 0)
            {
                assignmentId = _assignmentId;
                LoadAssignment();
            }
        } 

        public string CourseName { get; set; }
        public string PageTitle => assignmentId == 0
            ? "New Assignment" : "Edit Assignment";

        private string assignmentName;
        public string AssignmentName
        {
            get => assignmentName;
            set { assignmentName = value; OnPropertyChanged(); }
        }

        private string assignmentDescription;
        public string AssignmentDescription
        {
            get => assignmentDescription;
            set { assignmentDescription = value; OnPropertyChanged(); }
        }

        private int newAvailablePoints;
        public int NewAvailablePoints
        {
            get => newAvailablePoints;
            set { newAvailablePoints = value; OnPropertyChanged(); }
        }

        private DateTime newDueDate;
        public DateTime NewDueDate
        {
            get => newDueDate;
            set { newDueDate = value; OnPropertyChanged(); }
        }

        private string validationError;
        public string ValidationError
        {
            get => validationError;
            set { validationError = value; OnPropertyChanged(); }
        }

        private bool hasValidationError;
        public bool HasValidationError
        {
            get => hasValidationError;
            set { hasValidationError = value; OnPropertyChanged(); }
        }

        public ObservableCollection<AssignmentGroup> AvailableGroups { get; set; }

        private AssignmentGroup selectedGroup;
        public AssignmentGroup SelectedGroup
        {
            get => selectedGroup;
            set { selectedGroup = value; OnPropertyChanged(); }
        }

        private void LoadGroups()
        {
            AvailableGroups.Clear();
            var groups = CourseServiceProxy.Current.GetAssignmentGroups(courseId);
            foreach (var g in groups)
                AvailableGroups.Add(g);
        }

        public void LoadAssignment()
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(i => i.Id == CourseId);
            var assignment = course?.Assignments?.FirstOrDefault(i => i.Id == AssignmentId);
            if (assignment != null)
            {
                AssignmentName = assignment.Name;
                AssignmentDescription = assignment.Description;
                NewAvailablePoints = assignment.AvailablePoints;
                NewDueDate = assignment.DueDate;
                CourseName = course.Name;

                // groups already loaded so pre-selection will find the match
                if (assignment.GroupId != 0)
                    SelectedGroup = AvailableGroups.FirstOrDefault(g => g.Id == assignment.GroupId);
            }
        }

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
            HasValidationError = false;
            return true;
        }

        public void Save()
        {
            // capture original groupId before overwriting
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == courseId);
            var original = course?.Assignments?.FirstOrDefault(a => a.Id == assignmentId);
            int previousGroupId = original?.GroupId ?? 0;

            var assignmentToSave = new Assignment
            {
                Id = assignmentId,
                Name = AssignmentName,
                Description = AssignmentDescription,
                AvailablePoints = NewAvailablePoints,
                DueDate = newDueDate,
                GroupId = SelectedGroup?.Id ?? 0
            };

            // capture returned assignment to get generated Id for new assignments
            var saved = CourseServiceProxy.Current.AddOrUpdateAssignment(courseId, assignmentToSave);
            if (saved == null) return;

            if (saved.GroupId != 0)
                CourseServiceProxy.Current.AddAssignmentToGroup(courseId, saved.GroupId, saved.Id);
            else if (previousGroupId != 0)
                CourseServiceProxy.Current.RemoveAssignmentFromGroup(courseId, previousGroupId, saved.Id);
        }
    }
}
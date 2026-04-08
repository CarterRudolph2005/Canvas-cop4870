using Canvas.Library.Model;
using Canvas.Library.Services;
using Canvas.MAUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Canvas.MAUI.ViewModels
{
    internal class StudentCourseViewModel : INotifyPropertyChanged, IQueryAttributable
    {
        // ── Wrapper for assignment + resolved group name ────────────────────
        public class StudentAssignmentDisplay
        {
            public Assignment Assignment { get; set; }
            public string Name => Assignment.Name;
            public string Description => Assignment.Description;
            public int AvailablePoints => Assignment.AvailablePoints;
            public DateTime DueDate => Assignment.DueDate;
            public string GroupName { get; set; }
            public bool HasGroup => !string.IsNullOrEmpty(GroupName);
        }

        public StudentCourseViewModel()
        {
            Modules = new ObservableCollection<ModuleViewModel>();
            Assignments = new ObservableCollection<StudentAssignmentDisplay>();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("studentId", out var sId) && int.TryParse(sId?.ToString(), out int _studentId))
                studentId = _studentId;

            if (query.TryGetValue("courseId", out var value) && int.TryParse(value?.ToString(), out int id))
            {
                courseId = id;
                LoadMenu();
            }
        }

        private string name;
        public string Name
        {
            get => name;
            set { name = value; OnPropertyChanged(); }
        }

        private int courseId;
        public int CourseId
        {
            get => courseId;
            set { courseId = value; OnPropertyChanged(); }
        }

        private int studentId;
        public int StudentId
        {
            get => studentId;
            set { studentId = value; OnPropertyChanged(); }
        }

        private string code;
        public string Code
        {
            get => code;
            set { code = value; OnPropertyChanged(); }
        }

        private double gradePercentage;
        public double GradePercentage
        {
            get => gradePercentage;
            set { gradePercentage = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ModuleViewModel> modules;
        public ObservableCollection<ModuleViewModel> Modules
        {
            get => modules;
            set { modules = value; OnPropertyChanged(); }
        }

        private ObservableCollection<StudentAssignmentDisplay> assignments;
        public ObservableCollection<StudentAssignmentDisplay> Assignments
        {
            get => assignments;
            set { assignments = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Announcement> announcements;
        public ObservableCollection<Announcement> Announcements
        {
            get => announcements;
            set { announcements = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void LoadMenu()
        {
            var _course = CourseServiceProxy.Current.Courses.FirstOrDefault(i => i.Id == courseId);
            if (_course == null) return;

            Name = _course.Name ?? string.Empty;
            Code = _course.Code ?? string.Empty;
            Announcements = new ObservableCollection<Announcement>(_course.Announcements ?? new List<Announcement>());
            GradePercentage = CourseServiceProxy.Current.CalculateGrade(CourseId, StudentId);

            // build display wrappers with resolved group names
            var groups = CourseServiceProxy.Current.GetAssignmentGroups(courseId);
            var displayList = new ObservableCollection<StudentAssignmentDisplay>();
            foreach (var a in _course.Assignments ?? new List<Assignment>())
            {
                var group = groups.FirstOrDefault(g => g.Id == a.GroupId);
                displayList.Add(new StudentAssignmentDisplay
                {
                    Assignment = a,
                    GroupName = group?.Name ?? string.Empty
                });
            }
            Assignments = displayList;

            // modules unchanged
            Modules = new ObservableCollection<ModuleViewModel>();
            if (_course.Modules != null)
            {
                foreach (var m in _course.Modules)
                {
                    var moduleVM = new ModuleViewModel
                    {
                        Id = m.Id,
                        ModuleName = m.ModuleName,
                        ModuleContents = m.ModuleContents?.ToList() ?? new List<ModuleContent>()
                    };

                    foreach (var content in moduleVM.ModuleContents)
                    {
                        if (content is AssignmentContent ac)
                        {
                            var assignment = _course.Assignments?.FirstOrDefault(a => a.Id == ac.AssignmentId);
                            if (assignment != null)
                                ac.Name = assignment.Name;
                        }
                    }

                    moduleVM.RefreshContents();
                    Modules.Add(moduleVM);
                }
            }
        }
    }
}
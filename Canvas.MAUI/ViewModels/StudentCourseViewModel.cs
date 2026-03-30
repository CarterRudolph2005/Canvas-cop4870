using Canvas.Library.Model;
using Canvas.Library.Services;

//from class github
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Canvas.MAUI.ViewModels
{

    internal class StudentCourseViewModel : INotifyPropertyChanged, IQueryAttributable
    {
        public StudentCourseViewModel()
        {
            Modules = new ObservableCollection<Module>();
            Assignments = new ObservableCollection<Assignment>();
        }
        private int courseId;
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
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

        private string code;
        public string Code
        {
            get => code;
            set { code = value; OnPropertyChanged(); }
        }
        private ObservableCollection<Module> modules;
        public ObservableCollection<Module> Modules
        {
            get => modules;
            set
            {
                modules = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Assignment> assignments;
        public ObservableCollection<Assignment> Assignments
        {
            get => assignments;
            set
            {
                assignments = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Announcement> announcements;
        public ObservableCollection<Announcement> Announcements
        {
            get => announcements;
            set{ announcements = value; OnPropertyChanged(); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void LoadMenu()
        {
            var _course = CourseServiceProxy.Current.Courses.FirstOrDefault(i => i.Id == courseId);
            if (_course == null) return;

            Modules = new ObservableCollection<Module>(_course.Modules ?? new List<Module>());
            Assignments = new ObservableCollection<Assignment>(_course.Assignments ?? new List<Assignment>());
            Announcements = new ObservableCollection<Announcement>(_course.Announcements ?? new List<Announcement>());
            Name = _course.Name ?? String.Empty;
            Code = _course.Code ?? String.Empty;
        }

    }
}
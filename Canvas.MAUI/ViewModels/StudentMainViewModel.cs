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

    internal class StudentMainViewModel : INotifyPropertyChanged, IQueryAttributable
    {
        public StudentMainViewModel()
        {
            Courses = new ObservableCollection<Course>();
        }
        private int studentId;
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("studentId", out var value) && int.TryParse(value?.ToString(), out int id))
            {
                studentId = id;
                LoadCourses();
            }
        }
        private ObservableCollection<Course> courses;
        public ObservableCollection<Course> Courses
        {
            get => courses;
            set
            {
                courses = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void LoadCourses()
        {
            var courses = CourseServiceProxy.Current.GetCoursesForStudent(studentId);
            Courses = new ObservableCollection<Course>(courses);
        }

    }
}
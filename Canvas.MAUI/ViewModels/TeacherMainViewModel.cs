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
    internal class TeacherMainViewModel : INotifyPropertyChanged, IQueryAttributable
    {
        public TeacherMainViewModel()
        {
            Courses = new ObservableCollection<Course>();
        }

        private int teacherId;
        public int TeacherId
        {
            get => teacherId;
            set{ teacherId = value; OnPropertyChanged(); }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("teacherId", out var value) && int.TryParse(value?.ToString(), out int id))
            {
                teacherId = id;
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
            var courses = InstructorServiceProxy.Current.GetCoursesForInstructor(teacherId);
            Courses = new ObservableCollection<Course>(courses);
        }
    }
}
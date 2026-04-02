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
            InitializeFilterOptions();
        }

        private List<int> availableYears;
        public List<int> AvailableYears
        {
            get => availableYears;
            set { availableYears = value; OnPropertyChanged(); }
        }

        private List<string> availableSemesters;
        public List<string> AvailableSemesters
        {
            get => availableSemesters;
            set { availableSemesters = value; OnPropertyChanged(); }
        }

        public List<string> SortOptions { get; } = new List<string>
        {
            "Newest First",
            "Oldest First",
            "Name A-Z",
            "Name Z-A"
        };

        private int? selectedYear;
        public int? SelectedYear
        {
            get => selectedYear;
            set { selectedYear = value; OnPropertyChanged(); ApplyFilters(); }
        }

        private string selectedSemester;
        public string SelectedSemester
        {
            get => selectedSemester;
            set { selectedSemester = value; OnPropertyChanged(); ApplyFilters(); }
        }

        private string selectedSortOption;
        public string SelectedSortOption
        {
            get => selectedSortOption;
            set { selectedSortOption = value; OnPropertyChanged(); ApplyFilters(); }
        }

        private bool showFilterCoursesForm;
        public bool ShowFilterCoursesForm
        {
            get => showFilterCoursesForm;
            set { showFilterCoursesForm = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Course> filteredCourses;
        public ObservableCollection<Course> FilteredCourses
        {
            get => filteredCourses;
            set { filteredCourses = value; OnPropertyChanged(); }
        }

        public void ToggleFilter() => ShowFilterCoursesForm = !ShowFilterCoursesForm;

        public void ClearFilters()
        {
            SelectedYear = null;
            SelectedSemester = null;
            SelectedSortOption = SortOptions[0];
        }

        private void ApplyFilters()
        {
            var filtered = Courses.AsEnumerable();

            if (SelectedYear.HasValue)
                filtered = filtered.Where(c => c.SemesterTaught?.Year == SelectedYear.Value);

            if (!string.IsNullOrEmpty(SelectedSemester))
                filtered = filtered.Where(c => c.SemesterTaught?.Session.ToString() == SelectedSemester);

            filtered = SelectedSortOption switch
            {
                "Oldest First" => filtered.OrderBy(c => c.SemesterTaught?.Year)
                                        .ThenBy(c => c.SemesterTaught?.Session),
                "Name A-Z"     => filtered.OrderBy(c => c.Name),
                "Name Z-A"     => filtered.OrderByDescending(c => c.Name),
                _              => filtered.OrderByDescending(c => c.SemesterTaught?.Year)
                                        .ThenBy(c => c.SemesterTaught?.Session)
            };

            FilteredCourses = new ObservableCollection<Course>(filtered);
        }

        private void InitializeFilterOptions()
        {
            AvailableYears = Courses
                .Where(c => c.SemesterTaught != null)
                .Select(c => c.SemesterTaught.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            AvailableSemesters = Enum.GetNames(typeof(SemesterType)).ToList();

            SelectedSortOption = SortOptions[0];
            ApplyFilters();
        }
    }
}
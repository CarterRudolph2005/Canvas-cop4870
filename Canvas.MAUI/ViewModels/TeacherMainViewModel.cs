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

        public void Refresh()
        {
            var courses = InstructorServiceProxy.Current.GetCoursesForInstructor(teacherId);
            Courses = new ObservableCollection<Course>(courses);
            InitializeFilterOptions();
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

        private bool showAddCourseForm;
        public bool ShowAddCourseForm
        {
            get => showAddCourseForm;
            set { showAddCourseForm = value; OnPropertyChanged(); }
        }

        private string newCourseName;
        public string NewCourseName
        {
            get => newCourseName;
            set { newCourseName = value; OnPropertyChanged(); }
        }

        private string newCourseCode;
        public string NewCourseCode
        {
            get => newCourseCode;
            set { newCourseCode = value; OnPropertyChanged(); }
        }

        private int newCourseSelectedYear;
        public int NewCourseSelectedYear
        {
            get => newCourseSelectedYear;
            set { newCourseSelectedYear = value; OnPropertyChanged(); }
        }

        private SemesterType newCourseSelectedSemester;
        public SemesterType NewCourseSelectedSemester
        {
            get => newCourseSelectedSemester;
            set { newCourseSelectedSemester = value; OnPropertyChanged(); }
        }

        public List<int> NewCourseYears { get; } = Enumerable
            .Range(2026, 10)
            .ToList();

        public List<SemesterType> NewCourseSemesters { get; } = Enum
            .GetValues(typeof(SemesterType))
            .Cast<SemesterType>()
            .ToList();

        public void ToggleAddCourseForm()
        {
            ShowAddCourseForm = !ShowAddCourseForm;
            if (!ShowAddCourseForm)
            {
                NewCourseName = string.Empty;
                NewCourseCode = string.Empty;
                NewCourseSelectedYear = NewCourseYears[0];
                NewCourseSelectedSemester = NewCourseSemesters[0];
            }
        }

        public async Task AddCourse()
        {
            if (string.IsNullOrWhiteSpace(NewCourseName) || 
                string.IsNullOrWhiteSpace(NewCourseCode)) return;
            try
            {
                var course = new Course
                {
                    Name = NewCourseName,
                    Code = NewCourseCode,
                    SemesterTaught = new Semester(NewCourseSelectedYear, NewCourseSelectedSemester),
                    Instructors = new List<Instructor> { new Instructor { Id = TeacherId } },
                    Assignments = new List<Assignment>(),
                    Modules = new List<Module>(),
                    Announcements = new List<Announcement>(),
                    AssignmentGroups = new List<AssignmentGroup>(),
                    Roster = new List<Student>(),
                    GradeScale = new List<LetterGrade>()
                };

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(course);
                var response = CourseServiceProxy.Current.AddOrUpdate(course);
                
                LoadCourses();
                ToggleAddCourseForm();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message + "\n\n" + ex.InnerException?.Message, "OK");
            }
        }


        public List<Course> GetAllCourses()
        {
            return InstructorServiceProxy.Current.GetCoursesForInstructor(teacherId);
        }

        public Course CopyCourseWithDetails(int sourceCourseId, int sectionNumber, int year, SemesterType semester)
        {
            var copy = CourseServiceProxy.Current.CopyCourse(sourceCourseId, sectionNumber, year, semester, teacherId);
            LoadCourses();
            return copy;
        }
    }
}
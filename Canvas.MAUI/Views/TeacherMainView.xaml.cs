using Canvas.MAUI.ViewModels;
using Canvas.Library.Model;
namespace Canvas.MAUI.Views
{
    public partial class TeacherMainView : ContentPage
    {
        public TeacherMainView()
        {
            InitializeComponent();
            BindingContext = new TeacherMainViewModel();
        }

        private void BackClicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//MainPage");
        }

        private void ManageStudentsClicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//TeacherStudentManagementView");
        }

        private async void CourseTapped(object sender, SelectionChangedEventArgs e)
        {
            var vm = BindingContext as TeacherMainViewModel;
            if (e.CurrentSelection.Count == 0) return;
            var selected = e.CurrentSelection[0] as Course;
            ((CollectionView)sender).SelectedItem = null;
            await Shell.Current.GoToAsync
            ($"TeacherCourseView?courseId={selected.Id}&teacherId={vm.TeacherId}&sectionNumber={selected.SectionNumber}");
        }
        private void ToggleFilterClicked(object sender, EventArgs e)
        {
            (BindingContext as TeacherMainViewModel)?.ToggleFilter();
        }

        private void ClearFiltersClicked(object sender, EventArgs e)
        {
            (BindingContext as TeacherMainViewModel)?.ClearFilters();
        }
        private void ToggleAddCourseClicked(object sender, EventArgs e)
        {
            (BindingContext as TeacherMainViewModel)?.ToggleAddCourseForm();
        }

        private void AddCourseClicked(object sender, EventArgs e)
        {
            (BindingContext as TeacherMainViewModel)?.AddCourse();
        }

        private async void CopyCourseClicked(object sender, EventArgs e)
        {
            var vm = BindingContext as TeacherMainViewModel;
            if (vm == null) return;

            var courses = vm.GetAllCourses();
            if (!courses.Any())
            {
                await DisplayAlert("No Courses", "There are no courses available to copy.", "OK");
                return;
            }

            var options = courses
                .Select(c => $"{c.Name} — {c.SemesterTaught?.Session} {c.SemesterTaught?.Year} (Section {c.SectionNumber})")
                .ToArray();

            var chosen = await DisplayActionSheet("Copy Which Course?", "Cancel", null, options);
            if (chosen == null || chosen == "Cancel") return;

            var index = Array.IndexOf(options, chosen);
            if (index < 0) return;
            var source = courses[index];

            var semesterOptions = Enum.GetNames(typeof(SemesterType));
            var chosenSemester = await DisplayActionSheet("Select Semester", "Cancel", null, semesterOptions);
            if (chosenSemester == null || chosenSemester == "Cancel") return;
            var semesterType = Enum.Parse<SemesterType>(chosenSemester);

            var yearOptions = Enumerable.Range(2026, 10).Select(y => y.ToString()).ToArray();
            var chosenYear = await DisplayActionSheet("Select Year", "Cancel", null, yearOptions);
            if (chosenYear == null || chosenYear == "Cancel") return;
            var year = int.Parse(chosenYear);

            var sectionInput = await DisplayPromptAsync(
                "Section Number",
                "Enter the section number for the new course:",
                keyboard: Keyboard.Numeric);

            if (string.IsNullOrWhiteSpace(sectionInput)) return;
            if (!int.TryParse(sectionInput, out int sectionNumber) || sectionNumber <= 0)
            {
                await DisplayAlert("Invalid Input", "Section number must be a positive number.", "OK");
                return;
            }

            var copy = vm.CopyCourseWithDetails(source.Id, sectionNumber, year, semesterType);

            if (copy != null)
                await DisplayAlert("Course Copied", $"\"{source.Name}\" has been copied to {chosenSemester} {year} Section {sectionNumber}.", "OK");
            else
                await DisplayAlert("Error", "Course could not be copied.", "OK");
        }
        private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
        {
            (BindingContext as TeacherMainViewModel)?.Refresh();
        }
    }

}
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
    }
}
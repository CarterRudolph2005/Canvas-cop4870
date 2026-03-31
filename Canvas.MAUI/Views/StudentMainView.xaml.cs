using Canvas.MAUI.ViewModels;
using Canvas.Library.Model;
namespace Canvas.MAUI.Views
{
    public partial class StudentMainView : ContentPage
    {
        public StudentMainView()
        {
            InitializeComponent();
            BindingContext = new StudentMainViewModel();
        }
        private async void CourseTapped(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count == 0) return;
            var selected = e.CurrentSelection[0] as Course;
            ((CollectionView)sender).SelectedItem = null;
            await Shell.Current.GoToAsync($"StudentCourseMenu?courseId={selected.Id}&studentId={(BindingContext as StudentMainViewModel).StudentId}");
        }
        private async void BackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}
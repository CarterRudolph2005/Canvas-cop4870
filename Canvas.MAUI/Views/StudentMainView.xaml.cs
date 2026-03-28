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
        // public void ApplyQueryAttributes(IDictionary<string, object> query)
        // {
        //     ViewModels.StudentMainViewModel.ApplyQueryAttributes(query);
        // }
        private async void CourseTapped(object sender, SelectionChangedEventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
            // if (e.CurrentSelection.Count == 0) return;
            // var selected = e.CurrentSelection[0] as Course;
            // ((CollectionView)sender).SelectedItem = null;
            // await Shell.Current.GoToAsync($"//CourseView?courseId={selected.Id}");
        }
        private async void LeaveMenuClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}
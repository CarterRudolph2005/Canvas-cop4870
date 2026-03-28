using Canvas.Library.Model;
using Canvas.MAUI.ViewModels;
namespace Canvas.MAUI.Views
{
    public partial class StudentCourseView : ContentPage
    {
        public StudentCourseView()
        {
            InitializeComponent();
            BindingContext = new StudentCourseViewModel();
        }
        private async void BackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//StudentMainView");
        }
    }
    
}
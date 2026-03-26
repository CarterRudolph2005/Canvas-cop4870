using Canvas.MAUI.ViewModels;
namespace Canvas.MAUI.Views
{
    public partial class TeacherStudentManagementView : ContentPage
    {
        public TeacherStudentManagementView()
        {
            InitializeComponent();
            BindingContext = new TeacherStudentManagementViewModel();
        }

        private async void AddStudentClicked(object sender, EventArgs e)
        {
            Console.WriteLine("Here");
            await Shell.Current.GoToAsync("//StudentDetailPage?studentId=0");
        }
        private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
        {
            (BindingContext as TeacherStudentManagementViewModel).Refresh();
        }
    }
}
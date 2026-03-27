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
            await Shell.Current.GoToAsync("//StudentDetailPage?studentId=0");
        }
        private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
        {
            (BindingContext as TeacherStudentManagementViewModel).Refresh();
        }

        
        private void StudentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DeleteBtn.IsEnabled = e.CurrentSelection.Count > 0;
            DeleteBtn.Opacity = e.CurrentSelection.Count > 0 ? 1 : 0.5;
        }
        
        private async void DeleteStudentClicked(object sender, EventArgs e)
        {
            var context = (BindingContext as TeacherStudentManagementViewModel);
            if (context != null)
            {
                var studnet = await context.Delete();
                if(studnet == null)
                {
                    DisplayAlert("No Student Found", "There is no student selected to delete", "Okay");
                }else{
                DisplayAlert("Success", "Student deleted!", "Okay");}
            }
            else
            {
                await DisplayAlert("Error", "Could not load student management context.", "Okay");
            }
        }
    }
}
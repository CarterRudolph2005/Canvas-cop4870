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
        private async void EditStudentClicked(object sender, EventArgs e)
        {
            var context = BindingContext as TeacherStudentManagementViewModel;
            if (context?.SelectedStudent != null)
            {
                await Shell.Current.GoToAsync($"//StudentDetailPage?studentId={context.SelectedStudent.Id}");
            }
            context?.SelectedStudent = null;
        }
        private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
        {
            (BindingContext as TeacherStudentManagementViewModel).Refresh();
        }

        
        private void StudentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DeleteBtn.IsEnabled = e.CurrentSelection.Count > 0;
            DeleteBtn.Opacity = e.CurrentSelection.Count > 0 ? 1 : 0.5;
            EditBtn.IsEnabled = e.CurrentSelection.Count > 0;
            EditBtn.Opacity = e.CurrentSelection.Count > 0 ? 1 : 0.5;
        }
        
        private async void DeleteStudentClicked(object sender, EventArgs e)
        {
            var context = (BindingContext as TeacherStudentManagementViewModel);
            if (context != null)
            {
                var studnet = await context.Delete();
                if(studnet == null)
                {
                    await DisplayAlertAsync("No Student Found", "There is no student selected to delete", "Okay");
                }else
                {
                    await DisplayAlertAsync("Success", "Student deleted!", "Okay");
                }
            }
            else
            {
                await DisplayAlertAsync("Error", "Could not load student management context.", "Okay");
            }
            context?.SelectedStudent = null;
        }
    }
}
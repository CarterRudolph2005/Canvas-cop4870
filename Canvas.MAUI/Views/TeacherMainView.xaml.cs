namespace Canvas.MAUI.Views
{
    public partial class TeacherMainView : ContentPage
    {
        public TeacherMainView()
        {
            InitializeComponent();
        }

        private void LeaveMenuClicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//MainPage");
        }

        private void ManageStudentsClicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//TeacherStudentManagementView");
        }
    }
}
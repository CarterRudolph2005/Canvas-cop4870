namespace Canvas.MAUI.Views
{
    public partial class StudentMainView : ContentPage
    {
        public StudentMainView()
        {
            InitializeComponent();
        }

        private void LeaveMenuClicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//MainPage");
        }
    }
}
namespace Canvas.MAUI
{
	public partial class MainPage : ContentPage
	{

		public MainPage()
		{
			InitializeComponent();
		}

		private  void OnTeacherClicked(object sender, EventArgs e)
		{
			Shell.Current.GoToAsync("//TeacherMainView");
		}

		private void OnStudentClicked(object sender, EventArgs e)
		{
			Shell.Current.GoToAsync("//StudentMainView");
		}
	}
}
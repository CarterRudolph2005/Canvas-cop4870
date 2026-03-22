namespace Canvas.MAUI;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private  void OnTeacherClicked(object sender, EventArgs e)
	{
		DisplayAlert("Teacher Mode", "Loading Teacher Dashboard...", "OK");
	}

	private void OnStudentClicked(object sender, EventArgs e)
	{
		DisplayAlert("Student Mode", "Loading Student Selection...", "OK");
	}
}

using System.Threading.Tasks;

namespace Canvas.MAUI
{
	public partial class MainPage : ContentPage
	{

		public MainPage()
		{
			InitializeComponent();
		}

		private async void OnTeacherClicked(object sender, EventArgs e)
		{
			await Shell.Current.GoToAsync($"//TeacherMainView?teacherId=3");
		}

		private async void OnStudentClicked(object sender, EventArgs e)
		{
			await Shell.Current.GoToAsync("//ProxyStudentSelection");
		}
	}
}
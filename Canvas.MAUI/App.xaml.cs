using Microsoft.Extensions.DependencyInjection;
using Canvas.MAUI.Views;
namespace Canvas.MAUI;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		Routing.RegisterRoute("StudentCourseMenu", typeof(StudentCourseView));
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}
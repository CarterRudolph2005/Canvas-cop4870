using Canvas.MAUI.Views;

namespace Canvas.MAUI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(CourseSettingsPage), typeof(CourseSettingsPage));
    }
}
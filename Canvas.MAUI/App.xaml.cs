using Microsoft.Extensions.DependencyInjection;
using Canvas.MAUI.Views;
namespace Canvas.MAUI;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		Routing.RegisterRoute("StudentCourseMenu", typeof(StudentCourseView));
		Routing.RegisterRoute("TeacherCourseView", typeof(TeacherCourseView));
        Routing.RegisterRoute("AssignmentDetailView", typeof(AssignmentDetailView));
		Routing.RegisterRoute("AssignmentSubmissionView", typeof(AssignmentSubmissionView));
		Routing.RegisterRoute("ContentPageView", typeof(ContentPageView));
		Routing.RegisterRoute("ModuleContentCreationView", typeof(ModuleContentCreationView));
		Routing.RegisterRoute("GradeSubmissionView", typeof(GradeSubmissionView));
		Routing.RegisterRoute(nameof(AssignmentGroupView), typeof(AssignmentGroupView));
		Routing.RegisterRoute(nameof(QuizEditorPage), typeof(QuizEditorPage));
		Routing.RegisterRoute(nameof(QuizAttemptPage), typeof(QuizAttemptPage));
		Routing.RegisterRoute(nameof(CourseSettingsPage), typeof(CourseSettingsPage));
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}
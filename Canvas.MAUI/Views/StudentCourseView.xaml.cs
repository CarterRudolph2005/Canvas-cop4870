using Canvas.Library.Model;
using Canvas.MAUI.Models;
using Canvas.MAUI.ViewModels;
using CommunityToolkit.Maui.Storage;

namespace Canvas.MAUI.Views
{
    public partial class StudentCourseView : ContentPage
    {
        public StudentCourseView()
        {
            InitializeComponent();
            BindingContext = new StudentCourseViewModel();
        }

        private async void BackClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

private async void AssignmentTapped(object sender, SelectionChangedEventArgs e)
{
    if (e.CurrentSelection.Count == 0) return;
 
    var display = e.CurrentSelection[0] as StudentCourseViewModel.StudentAssignmentDisplay;
    ((CollectionView)sender).SelectedItem = null;
 
    if (display == null) return;
 
    var vm = BindingContext as StudentCourseViewModel;
    if (vm == null) return;
 
    if (display.Assignment.IsQuiz)
    {
        // Route to quiz attempt page
        await Shell.Current.GoToAsync(nameof(QuizAttemptPage), new Dictionary<string, object>
        {
            { "courseId", vm.CourseId },
            { "assignmentId", display.Assignment.Id },
            { "studentId", vm.StudentId }
        });
    }
    else
    {
        // Route to regular submission page — match your existing navigation pattern
        await Shell.Current.GoToAsync(
            $"AssignmentSubmissionView?courseId={vm.CourseId}&assignmentId={display.Assignment.Id}&studentId={vm.StudentId}");
    }
}

        private void ModuleHeaderTapped(object? sender, TappedEventArgs e)
        {
            var module = e.Parameter as ModuleViewModel;
            if (module == null) return;
            module.IsExpanded = !module.IsExpanded;
        }

        private async void ModuleContentTapped(object? sender, TappedEventArgs e)
        {
            var content = e.Parameter as ModuleContent;
            if (content == null) return;

            var vm = BindingContext as StudentCourseViewModel;

            switch (content)
            {
                case AssignmentContent a:
                    await Shell.Current.GoToAsync(
                        $"AssignmentSubmissionView?courseId={vm.CourseId}&assignmentId={a.AssignmentId}&studentId={vm.StudentId}");
                    break;

                case FileContent f:
                    var confirm = await DisplayAlert(f.Name, $"Download {f.Name}?", "Download", "Cancel");
                    if (!confirm) break;
                    try
                    {
                        var stream = await FileSystem.OpenAppPackageFileAsync(f.FilePath);
                        var result = await FileSaver.Default.SaveAsync(f.FilePath, stream, CancellationToken.None);
                        if (result.IsSuccessful)
                            await DisplayAlert("Downloaded", $"Saved to {result.FilePath}", "OK");
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Download Failed", ex.Message, "OK");
                    }
                    break;

                case PageContent:
                default:
                    await Shell.Current.GoToAsync(
                        $"ContentPageView?courseId={vm.CourseId}&contentId={content.Id}");
                    break;
            }
        }
    }
}
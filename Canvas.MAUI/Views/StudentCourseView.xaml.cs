using Canvas.Library.Model;
using Canvas.MAUI.ViewModels;
namespace Canvas.MAUI.Views
{
    public partial class StudentCourseView : ContentPage
    {
        public StudentCourseView()
        {
            InitializeComponent();
            BindingContext = new StudentCourseViewModel();
        }
        private async void BackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
        private void ModuleHeaderTapped(object sender, TappedEventArgs e)
        {
            var grid = sender as Grid;
            var parentStack = grid.Parent as VerticalStackLayout;
            var contentStack = parentStack.Children[1] as VerticalStackLayout;
            var arrowLabel = grid.Children[1] as Label;

            // populate content on first expand
            if (contentStack.Children.Count == 0)
            {
                var module = grid.BindingContext as Module;
                foreach (var item in module.Content ?? new List<string>())
                {
                    contentStack.Children.Add(new Label
                    {
                        Text = item,
                        FontSize = 12,
                        TextColor = Color.FromArgb("#888888"),
                        Margin = new Thickness(8, 2)
                    });
                }
            }

            contentStack.IsVisible = !contentStack.IsVisible;
            arrowLabel.Text = contentStack.IsVisible ? "▼" : "▶";
        }

        private async void AssignmentTapped(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count == 0) return;
            var selected = e.CurrentSelection[0] as Assignment;
            ((CollectionView)sender).SelectedItem = null;

            var vm = BindingContext as StudentCourseViewModel;
            await Shell.Current.GoToAsync($"AssignmentSubmissionView?courseId={vm.CourseId}&assignmentId={selected.Id}&studentId={vm.StudentId}");
        }
    }
    
}
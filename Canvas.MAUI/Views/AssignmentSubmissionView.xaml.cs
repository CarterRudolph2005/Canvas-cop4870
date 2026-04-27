using Canvas.MAUI.ViewModels;

namespace Canvas.MAUI.Views
{
    public partial class AssignmentSubmissionView : ContentPage
    {
        public AssignmentSubmissionView()
        {
            InitializeComponent();
            BindingContext = new AssignmentSubmissionViewModel();
        }

        private async void BackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void PostCommentClicked(object sender, EventArgs e)
        {
            var vm = BindingContext as AssignmentSubmissionViewModel;
            if (vm == null) return;
            if (string.IsNullOrWhiteSpace(vm.NewCommentBody))
            {
                await DisplayAlert("Error", "Comment cannot be empty.", "OK");
                return;
            }
            vm.AddComment();
        }

    }
}
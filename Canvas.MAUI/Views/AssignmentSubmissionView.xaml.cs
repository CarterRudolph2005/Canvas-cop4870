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
    }
}
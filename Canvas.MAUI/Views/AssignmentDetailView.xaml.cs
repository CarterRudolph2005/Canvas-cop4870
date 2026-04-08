using Canvas.MAUI.ViewModels;

namespace Canvas.MAUI.Views
{
    public partial class AssignmentDetailView : ContentPage
    {
        private AssignmentDetailViewModel ViewModel => BindingContext as AssignmentDetailViewModel;

        public AssignmentDetailView()
        {
            InitializeComponent();
            BindingContext = new AssignmentDetailViewModel();
        }

        private async void SaveClicked(object sender, EventArgs e)
        {
            if (!ViewModel.Validate()) return;
            ViewModel.Save();
            await Shell.Current.GoToAsync("..");
        }

        private async void CancelClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
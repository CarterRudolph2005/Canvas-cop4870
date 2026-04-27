using Canvas.MAUI.ViewModels;

namespace Canvas.MAUI.Views
{
    public partial class AssignmentGroupView : ContentPage
    {
        private AssignmentGroupViewModel ViewModel => BindingContext as AssignmentGroupViewModel;

        public AssignmentGroupView()
        {
            InitializeComponent();
            BindingContext = new AssignmentGroupViewModel();
        }

        private async void SaveClicked(object sender, EventArgs e)
        {
            if (ViewModel.TrySave(out var savedGroup))
            {
                await Shell.Current.GoToAsync("..");
            }
        }

        private async void CancelClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
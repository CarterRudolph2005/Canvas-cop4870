using Canvas.Library.Model;
using Canvas.Library.Services;
using Canvas.MAUI.ViewModels;
namespace Canvas.MAUI.Views
{
    public partial class AssignmentDetailView : ContentPage
    {
        
        public AssignmentDetailView()
        {
            InitializeComponent();
            BindingContext = new AssignmentDetailViewModel();
        }

        private async void SaveClicked(object sender, EventArgs e)
        {
            if (!(BindingContext as AssignmentDetailViewModel).Validate()) return;
            (BindingContext as AssignmentDetailViewModel).Save();
            await Shell.Current.GoToAsync("..");
        }

        private async void CancelClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
        
    }
}
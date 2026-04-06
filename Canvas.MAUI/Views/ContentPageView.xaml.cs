using Canvas.MAUI.ViewModels;

namespace Canvas.MAUI.Views
{
    public partial class ContentPageView : ContentPage
    {
        public ContentPageView()
        {
            InitializeComponent();
            BindingContext = new ContentPageViewModel();
        }

        private async void BackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
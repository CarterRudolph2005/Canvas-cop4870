using Canvas.MAUI.ViewModels;
namespace Canvas.MAUI.Views
{
    public partial class ProxyStudentSelectionView : ContentPage
    {
        public ProxyStudentSelectionView()
        {
            InitializeComponent();
            BindingContext = new ProxyStudentSelectionViewModel();
        }

        private async void SelectStudentClicked(object sender, EventArgs e)
        {
            var id = (BindingContext as ProxyStudentSelectionViewModel).SelectedStudent.Id;
            await Shell.Current.GoToAsync($"//StudentMainView?studentId={id}");
        }
        private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
        {
            (BindingContext as ProxyStudentSelectionViewModel).Refresh();
            StudentList.SelectedItem = null;
            SelectBtn.IsEnabled = false;
            SelectBtn.Opacity = 0.5;
        }

        
        private void StudentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectBtn.IsEnabled = e.CurrentSelection.Count > 0;
            SelectBtn.Opacity = e.CurrentSelection.Count > 0 ? 1 : 0.5;
        }

        private async void ExitMenuClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
        
    }
}
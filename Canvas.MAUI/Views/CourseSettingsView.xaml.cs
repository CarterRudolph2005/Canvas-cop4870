using Canvas.MAUI.ViewModels;

namespace Canvas.MAUI.Views
{
    public partial class CourseSettingsPage : ContentPage
    {
        private CourseSettingsViewModel _vm;

        public CourseSettingsPage()
        {
            InitializeComponent();
            _vm = new CourseSettingsViewModel();
            BindingContext = _vm;
        }

        private async void SaveClicked(object sender, EventArgs e)
        {
            await _vm.SaveGradeScale();
            _vm.SaveSemesterDates();
            await DisplayAlert("Saved", "Settings saved successfully.", "OK");
        }
        
        // private async void SaveClicked(object sender, EventArgs e)
        // {
        //     await _vm.SaveGradeScale();
        //     await DisplayAlert("Saved", "Grade scale saved successfully.", "OK");
        // }

        private void PresetColorTapped(object sender, TappedEventArgs e)
        {
            if (_vm.SelectedRow == null) return;
            if (sender is Border border &&
                border.GestureRecognizers.FirstOrDefault() is TapGestureRecognizer tap)
            {
                _vm.SelectedRow.HexColor = tap.CommandParameter?.ToString() ?? "#888888";
            }
        }
    }
}
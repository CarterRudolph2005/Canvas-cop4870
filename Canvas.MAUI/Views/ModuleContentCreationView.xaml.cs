using Canvas.MAUI.ViewModels;
using Canvas.Library.Services;

namespace Canvas.MAUI.Views
{
    public partial class ModuleContentCreationView : ContentPage
    {
        public ModuleContentCreationView()
        {
            InitializeComponent();
            BindingContext = new ModuleContentCreationViewModel();
        }

        private async void BackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void PickFileClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync();
                if (result == null) return;

                var vm = BindingContext as ModuleContentCreationViewModel;
                if (vm == null) return;

                using var stream = await result.OpenReadAsync();
                var mimeType = result.ContentType ?? "application/octet-stream";

                var (filePath, returnedMime) = await CourseServiceProxy.Current
                    .UploadFileAsync(stream, result.FileName, mimeType);

                if (string.IsNullOrEmpty(filePath))
                {
                    await DisplayAlert("Upload Failed", "Could not upload the file.", "OK");
                    return;
                }

                vm.FilePath = filePath;
                vm.MimeType = returnedMime;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void SaveClicked(object sender, EventArgs e)
        {
            var vm = BindingContext as ModuleContentCreationViewModel;
            if (vm == null) return;
            try
            {
                if (!vm.TrySave())
                {
                    await DisplayAlert("Missing Fields", "Please fill in all required fields.", "OK");
                    return;
                }
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Save Failed", ex.Message, "OK");
            }
        }
    }
}
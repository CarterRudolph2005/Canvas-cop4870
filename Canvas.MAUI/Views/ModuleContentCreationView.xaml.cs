using Canvas.MAUI.ViewModels;
using CommunityToolkit.Maui.Storage;

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
                vm.FilePath = result.FileName;
                vm.MimeType = result.ContentType ?? "application/octet-stream";

                // copy to app package resources
                using var stream = await result.OpenReadAsync();
                var destPath = Path.Combine(FileSystem.AppDataDirectory, result.FileName);
                using var destStream = File.Create(destPath);
                await stream.CopyToAsync(destStream);

                vm.FilePath = result.FileName;
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
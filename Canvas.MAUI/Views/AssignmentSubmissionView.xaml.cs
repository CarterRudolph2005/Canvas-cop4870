using Canvas.Library.Services;
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

        private async void AttachFileClicked(object sender, EventArgs e)
        {
            var vm = BindingContext as AssignmentSubmissionViewModel;
            if (vm == null) return;

            try
            {
                var result = await FilePicker.Default.PickAsync();
                if (result == null) return;

                using var stream = await result.OpenReadAsync();
                var mimeType = result.ContentType ?? "application/octet-stream";

                var (filePath, returnedMime) = await CourseServiceProxy.Current
                    .UploadFileAsync(stream, result.FileName, mimeType);

                if (string.IsNullOrEmpty(filePath))
                {
                    await DisplayAlert("Upload Failed", "Could not upload the file.", "OK");
                    return;
                }

                vm.AttachedFilePath = filePath;
                vm.AttachedMimeType = returnedMime;
                vm.AttachedFileName = result.FileName;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
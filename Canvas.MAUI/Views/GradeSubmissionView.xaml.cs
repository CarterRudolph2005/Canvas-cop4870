using Canvas.Library.Model;
using Canvas.MAUI.ViewModels;

namespace Canvas.MAUI.Views
{
    public partial class GradeSubmissionView : ContentPage
    {
        private GradeSubmissionViewModel ViewModel => BindingContext as GradeSubmissionViewModel;

        public GradeSubmissionView()
        {
            InitializeComponent();
            BindingContext = new GradeSubmissionViewModel();
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            StudentPicker.ItemsSource = ViewModel.Submissions
                .Select(s => $"Student {s.StudentId}")
                .ToList();

            PointsLabel.Text = $"Points Awarded (out of {ViewModel.AvailablePoints})";
        }


        private async void SubmitGradeClicked(object sender, EventArgs e)
        {
            if (StudentPicker.SelectedIndex < 0)
            {
                await DisplayAlert("Error", "Please select a student.", "OK");
                return;
            }

            var submission = ViewModel.Submissions[StudentPicker.SelectedIndex];

            if (!int.TryParse(GradeEntry.Text, out int grade) || grade < 0 || grade > ViewModel.AvailablePoints)
            {
                await DisplayAlert("Invalid Grade", $"Please enter a number between 0 and {ViewModel.AvailablePoints}.", "OK");
                return;
            }

            ViewModel.GradeSubmission(submission.Id, grade);
            GradeEntry.Text = string.Empty;
            await DisplayAlert("Saved", $"Grade submitted for Student {submission.StudentId}.", "OK");
        }

        private async void BackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    
        private void StudentPickerChanged(object sender, EventArgs e)
        {
            if (StudentPicker.SelectedIndex < 0 || StudentPicker.SelectedIndex >= ViewModel.Submissions.Count) return;
            var submission = ViewModel.Submissions[StudentPicker.SelectedIndex];
            SubmissionText.Text = submission.Content ?? "No content submitted.";
            GradeEntry.Text = submission.PointsAwarded?.ToString() ?? string.Empty;
            ViewModel.LoadComments(submission.Id);
        }

        private async void PostCommentClicked(object sender, EventArgs e)
        {
            if (ViewModel.GetSelectedSubmissionId() == 0)
            {
                await DisplayAlert("Error", "Please select a student first.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(ViewModel.NewCommentBody))
            {
                await DisplayAlert("Error", "Comment cannot be empty.", "OK");
                return;
            }
            // Replace with actual logged-in instructor id/name when auth is available
            ViewModel.AddComment(2, "Dr. Smith");
        }

    }
}
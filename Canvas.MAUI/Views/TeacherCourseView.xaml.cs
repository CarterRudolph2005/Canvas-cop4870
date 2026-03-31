using Canvas.Library.Model;
using Canvas.Library.Services;
using Canvas.MAUI.Models;
using Canvas.MAUI.ViewModels;

namespace Canvas.MAUI.Views
{
    public partial class TeacherCourseView : ContentPage
    {
        private TeacherCourseViewModel ViewModel =>
            BindingContext as TeacherCourseViewModel;

        public TeacherCourseView()
        {
            InitializeComponent();
            BindingContext = new TeacherCourseViewModel();
            UpdateTabStyles(0);
        }

        private async void BackClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("//MainPage");

        // ── Tab switching ──────────────────────────────────────────
        private void UpdateTabStyles(int selectedIndex)
        {
            var tabs = new[] { HomeTabBtn, ModulesTabBtn, AssignmentsTabBtn, RosterTabBtn };
            for (int i = 0; i < tabs.Length; i++)
            {
                tabs[i].BackgroundColor = i == selectedIndex
                    ? Color.FromArgb("#1565C0")
                    : Colors.Transparent;
                tabs[i].TextColor = i == selectedIndex
                    ? Colors.White
                    : Color.FromArgb("#888888");
            }
        }

        private void AnnouncementsTabClicked(object sender, EventArgs e)
        {
            ViewModel.SelectTab(0);
            UpdateTabStyles(0);
        }
        private void ModulesTabClicked(object sender, EventArgs e)
        {
            ViewModel.SelectTab(1);
            UpdateTabStyles(1);
        }
        private void AssignmentsTabClicked(object sender, EventArgs e)
        {
            ViewModel.SelectTab(2);
            UpdateTabStyles(2);
        }
        private void RosterTabClicked(object sender, EventArgs e)
        {
            ViewModel.SelectTab(3);
            UpdateTabStyles(3);
        }

        // ── Announcements ──────────────────────────────────────────
        private void ToggleAnnouncementFormClicked(object sender, EventArgs e)
            => ViewModel.ToggleAnnouncementForm();

        private void SubmitAnnouncementClicked(object sender, EventArgs e)
            => ViewModel.SubmitAnnouncement();

        private void DeleteAnnouncementClicked(object sender, EventArgs e)
        {
            var announcement = (sender as Button)?.CommandParameter as Announcement;
            ViewModel.DeleteAnnouncement(announcement);
        }

        private async void EditAnnouncementClicked(object sender, EventArgs e)
        {
            var announcement = (sender as Button)?.CommandParameter as Announcement;
            // placeholder for edit navigation
        }

        // ── Modules ────────────────────────────────────────────────
        private void ToggleModuleFormClicked(object sender, EventArgs e)
            => ViewModel.ToggleModuleForm();

        private void SubmitModuleClicked(object sender, EventArgs e)
            => ViewModel.SubmitModule();

        private void DeleteModuleClicked(object sender, EventArgs e)
        {
            var module = (sender as Button)?.CommandParameter as ModuleViewModel;
            ViewModel.DeleteModule(module);
        }

        private async void AddModuleContentClicked(object sender, EventArgs e)
        {
            var module = (sender as Button)?.CommandParameter as ModuleViewModel;
            var content = await DisplayPromptAsync("Add Content", "Enter content item:");
            if (!string.IsNullOrWhiteSpace(content))
                ViewModel.AddModuleContent(module.Id, content);
        }
        private async void EditModuleClicked(object sender, EventArgs e)
        {
            var module = (sender as Button)?.CommandParameter as ModuleViewModel;
            var newName = await DisplayPromptAsync("Edit Module", "Enter new name:", initialValue: module.ModuleName);
            if (!string.IsNullOrWhiteSpace(newName))
                ViewModel.UpdateModuleName(module.Id, newName);
        }

        private async void EditModuleContentClicked(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var content = btn?.CommandParameter as string;
            var rowGrid = btn?.Parent as Grid;
            var contentStack = rowGrid?.Parent as VerticalStackLayout;
            var module = contentStack?.BindingContext as ModuleViewModel;

            if (module == null || content == null) return;

            var newContent = await DisplayPromptAsync("Edit Content", "Enter new content:", initialValue: content);
            if (!string.IsNullOrWhiteSpace(newContent))
            {
                var index = module.Content.IndexOf(content);
                if (index >= 0)
                    ViewModel.UpdateModuleContent(module.Id, index, newContent);
            }
        }

        private void DeleteModuleContentClicked(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var content = btn?.CommandParameter as string;
            var rowGrid = btn?.Parent as Grid;
            var contentStack = rowGrid?.Parent as VerticalStackLayout;
            var module = contentStack?.BindingContext as ModuleViewModel;

            if (module != null && content != null)
            {
                var index = module.Content.IndexOf(content);
                if (index >= 0)
                    ViewModel.DeleteModuleContent(module.Id, index);
            }
        }

        // ── Assignments ────────────────────────────────────────────
        private async void AddAssignmentClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync($"AssignmentDetailView?courseId={ViewModel.CourseId}");

        private async void EditAssignmentClicked(object sender, EventArgs e)
        {
            var assignment = (sender as Button)?.CommandParameter as Assignment;
            await Shell.Current.GoToAsync($"AssignmentDetailView?courseId={ViewModel.CourseId}&assignmentId={assignment.Id}");
        }

        private void DeleteAssignmentClicked(object sender, EventArgs e)
        {
            var assignment = (sender as Button)?.CommandParameter as Assignment;
            ViewModel.DeleteAssignment(assignment);
        }
        private async void CopyAssignmentClicked(object sender, EventArgs e)
        {
            var assignment = (sender as Button)?.CommandParameter as Assignment;
            if (assignment == null) return;

            var vm = BindingContext as TeacherCourseViewModel;
            if (vm == null) return;

            // Get all other courses this teacher owns, excluding current
             var otherCourses = CourseServiceProxy.Current.Courses
                .Where(c => c.Id != vm.CourseId && 
                            c.Instructors != null && 
                            c.Instructors.Any(i => i.Id == vm.TeacherId))
                .ToList();

            if (!otherCourses.Any())
            {
                await DisplayAlert("No Other Courses", "You have no other courses to copy to.", "OK");
                return;
            }

            var courseNames = otherCourses.Select(c => c.Name).ToArray();

            var chosen = await DisplayActionSheet(
                $"Copy \"{assignment.Name}\" to...",
                "Cancel",
                null,
                courseNames);

            if (chosen == null || chosen == "Cancel") return;

            var target = otherCourses.FirstOrDefault(c => c.Name == chosen);
            if (target == null) return;

            CourseServiceProxy.Current.CopyAssignmentToCourse(assignment.Id, vm.CourseId, target.Id);

            await DisplayAlert("Done", $"Copied to {target.Name}.", "OK");
        }

        // ── Roster ─────────────────────────────────────────────────
        private void ToggleEnrollFormClicked(object sender, EventArgs e)
            => ViewModel.ToggleEnrollForm();

        private void EnrollStudentClicked(object sender, EventArgs e)
            => ViewModel.EnrollStudent();

        private async void UnenrollStudentClicked(object sender, EventArgs e)
        {
            bool isConfirmed = await DisplayAlertAsync("Confirm Action", "Are you sure you want remove this student from the system?", "Yes", "No");
            if(isConfirmed)
            {
                var student = (sender as Button)?.CommandParameter as Student;
                ViewModel.UnenrollStudent(student);
            }
            else
            {
                return;
            }
        }
    }
}
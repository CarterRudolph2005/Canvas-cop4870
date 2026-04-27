using Canvas.Library.Model;
using Canvas.Library.Services;
using Canvas.MAUI.Models;
using Canvas.MAUI.ViewModels;
using CommunityToolkit.Maui.Storage; // For FileSaver
using System.Text;                   // For Encoding
using System.IO;                     // For MemoryStream
using System.Threading;              // For CancellationToken

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

        private void OnNavigatedTo(object sender, NavigatedToEventArgs e)
        {
            base.OnNavigatedTo(e);
            CourseServiceProxy.Current.InvalidateCache();
            ViewModel.LoadCourse();
        }   

        private async void ImportAssignmentsClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Select Assignment CSV",
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.MacCatalyst, new[] { "public.comma-separated-values-text" } },
                        { DevicePlatform.WinUI,       new[] { ".csv" } }
                    })
                });

                if (result == null) return;

                string[] lines;
                using (var stream = await result.OpenReadAsync())
                using (var reader = new StreamReader(stream))
                {
                    var content = await reader.ReadToEndAsync();
                    lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                }

                if (lines.Length < 2)
                {
                    await DisplayAlert("Empty File", "CSV has no data rows.", "OK");
                    return;
                }

                var assignments = ViewModel.ParseAssignmentCsv(lines);
                var (imported, skipped) = ViewModel.ImportAssignments(assignments, lines.Length - 1);

                var summary = $"✓ {imported} imported";
                if (skipped > 0) summary += $"\n✕ {skipped} row(s) skipped (invalid format)";

                await DisplayAlert("Import Complete", summary, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Import Failed", ex.Message, "OK");
            }
        }

        private async void ExportAssignmentsClicked(object sender, EventArgs e)
        {
            try
            {
                var vm = BindingContext as TeacherCourseViewModel;
                if (vm == null) return;

                var assignments = vm.Assignments;
                if (!assignments.Any())
                {
                    await DisplayAlert("No Assignments", "There are no assignments to export.", "OK");
                    return;
                }

                var sb = new StringBuilder();
                sb.AppendLine("Name,DueDate,Points,GroupId,IsQuiz,Description");
                foreach (var a in assignments)
                {
                    var name        = (a.Name ?? "").Replace(",", " ");
                    var description = (a.Description ?? "").Replace(",", " ");
                    sb.AppendLine($"{name},{a.DueDate:yyyy-MM-dd},{a.AvailablePoints},{a.GroupId},{a.IsQuiz.ToString().ToLower()},{description}");
                }

                var fileName = $"{vm.Code}_assignments.csv";
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
                var result = await FileSaver.Default.SaveAsync(fileName, stream, CancellationToken.None);

                if (result.IsSuccessful)
                    await DisplayAlert("Exported", $"Saved to {result.FilePath}", "OK");
                else
                    await DisplayAlert("Cancelled", "File was not saved.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Export Failed", ex.Message, "OK");
            }
        }

        private async void ExportGradebookClicked(object sender, EventArgs e)
        {
            var vm = BindingContext as TeacherCourseViewModel;
            if (vm == null) return;

            if (!vm.Roster.Any())
            {
                await DisplayAlert("Empty Roster", "There are no students to export.", "OK");
                return;
            }

            try
            {
                var csv = vm.BuildGradebookCsv();
                var fileName = $"{vm.Code}_gradebook.csv";
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
                var result = await FileSaver.Default.SaveAsync(fileName, stream, CancellationToken.None);

                if (result.IsSuccessful)
                    await DisplayAlert("Exported", $"Saved to {result.FilePath}", "OK");
                else
                    await DisplayAlert("Cancelled", "File was not saved.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Export Failed", ex.Message, "OK");
            }
        }

        private async void BackClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("//MainPage");
        
        private async void SettingsClicked(object sender, EventArgs e)
        {
            var vm = BindingContext as TeacherCourseViewModel;
            if (vm == null) return;
            await Shell.Current.GoToAsync(
                $"{nameof(CourseSettingsPage)}?courseId={vm.CourseId}");
        }

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
        }

        private void ToggleModuleFormClicked(object sender, EventArgs e)
            => ViewModel.ToggleModuleForm();

        private void SubmitModuleClicked(object sender, EventArgs e)
            => ViewModel.SubmitModule();

        private void DeleteModuleClicked(object sender, EventArgs e)
        {
            var module = (sender as Button)?.CommandParameter as ModuleViewModel;
            ViewModel.DeleteModule(module);
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
            var content = btn?.CommandParameter as ModuleContent;

            var grid = btn?.Parent as Grid;           // Grid (ColumnDefs Auto,*,44)
            var border = grid?.Parent as Border;      // Border (the content row border)
            var contentStack = border?.Parent as VerticalStackLayout;  // VerticalStackLayout (ModuleContents)
            var module = contentStack?.BindingContext as ModuleViewModel;

            if (module == null || content == null) return;

            ViewModel.DeleteModuleContent(module.Id, content);
            ViewModel.LoadCourse();
        }
        private async void ModuleContentTapped(object sender, TappedEventArgs e)
        {
            var content = e.Parameter as ModuleContent;
            if (content == null) return;

            var vm = BindingContext as TeacherCourseViewModel; // or TeacherCourseViewModel
            
            switch (content)
            {
                case AssignmentContent a:
                    return;

                case FileContent f:
                    var confirm = await DisplayAlert(f.Name, $"Open {f.Name}?", "Open", "Cancel");
                    if (!confirm) break;
                    try
                    {
                        var url = $"http://localhost:5258{f.FilePath}";
                        await Launcher.OpenAsync(url);
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", ex.Message, "OK");
                    }
                    break;

                case PageContent p:
                default:
                    await Shell.Current.GoToAsync(
                        $"ContentPageView?contentId={content.Id}&courseId={vm.CourseId}");
                    break;
            }
        }

        private void ModuleHeaderTapped(object sender, TappedEventArgs e)
        {
            var module = e.Parameter as ModuleViewModel;
            if (module == null) return;
            module.IsExpanded = !module.IsExpanded;
        }

        private async void AddModuleContentClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var module = button?.CommandParameter as ModuleViewModel
                    ?? button?.BindingContext as ModuleViewModel;

            if (module == null) return;

            var vm = BindingContext as TeacherCourseViewModel;
            if (vm == null) return;

            try
            {
                await Shell.Current.GoToAsync(
                    $"ModuleContentCreationView?courseId={vm.CourseId}&moduleId={module.Id}");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Navigation Error", ex.Message, "OK");
            }
        }

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
            ViewModel.LoadCourse();
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

        private async void GradeAssignmentClicked(object sender, TappedEventArgs e)
        {
            var assignment = e.Parameter as Assignment;
            if (assignment == null) return;

            if (assignment.Submissions == null || !assignment.Submissions.Any())
            {
                await DisplayAlert("No Submissions", "No students have submitted this assignment yet.", "OK");
                return;
            }

            await Shell.Current.GoToAsync($"GradeSubmissionView?courseId={ViewModel.CourseId}&assignmentId={assignment.Id}");
        }

        private async void AddQuizClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(QuizEditorPage), new Dictionary<string, object>
            {
                { "courseId", ViewModel.CourseId }
            });
        }

        private async void EditQuizClicked(object sender, EventArgs e)
        {
            if (sender is VisualElement el && el.BindingContext is Assignment assignment)
            {
                await Shell.Current.GoToAsync(nameof(QuizEditorPage), new Dictionary<string, object>
                {
                    { "courseId", ViewModel.CourseId },
                    { "assignmentId", assignment.Id }
                });
            }
        }

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
        private async void ExportRosterClicked(object sender, EventArgs e)
        {
            var vm = BindingContext as TeacherCourseViewModel;
            if (vm == null) return;

            var csv = CourseServiceProxy.Current.ExportRoster(vm.CourseId);
            if (string.IsNullOrEmpty(csv))
            {
                await DisplayAlert("Empty Roster", "There are no students to export.", "OK");
                return;
            }

            try
            {
                var fileName = $"{vm.Code}_roster.csv";
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
                
                var fileSaver = FileSaver.Default;
                var result = await fileSaver.SaveAsync(fileName, stream, CancellationToken.None);

                if (result.IsSuccessful)
                    await DisplayAlert("Exported", $"Saved to {result.FilePath}", "OK");
                else
                    await DisplayAlert("Cancelled", "File was not saved.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Export Failed", ex.Message, "OK");
            }
        }
        private async void ImportRosterClicked(object sender, EventArgs e)
        {
            try
            {
                var vm = BindingContext as TeacherCourseViewModel;
                PickOptions options = new PickOptions
                {
                    PickerTitle = "Please select a roster CSV",
                };
                
                var result = await FilePicker.Default.PickAsync(options);

                if(result == null) {return;}

                if(result.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    string csvContent;
                    using (var stream = await result.OpenReadAsync())
                    using (var reader = new StreamReader(stream))
                    {
                        csvContent = await reader.ReadToEndAsync();
                    }

                    var (added, skipped, notFound) = CourseServiceProxy.Current.ImportRoster(vm.CourseId, csvContent);

                    vm.LoadCourse();

                    var summary = $"✓ {added} added\n⟳ {skipped} already enrolled";
                    if (notFound > 0)
                        summary += $"\n✕ {notFound} code(s) not found in system";

                    await DisplayAlertAsync("Import Complete", summary, "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Failure", $"There was an error: {ex.Message}", "okay :(");
            }
        } 

        private async void AddGroupClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(AssignmentGroupView), new Dictionary<string, object>
            {
                { "courseId", ViewModel.CourseId }
            });
        }

        private async void EditGroupClicked(object sender, EventArgs e)
        {
            var display = (TeacherCourseViewModel.AssignmentGroupDisplay)((Button)sender).CommandParameter;
            await Shell.Current.GoToAsync(nameof(AssignmentGroupView), new Dictionary<string, object>
            {
                { "courseId", ViewModel.CourseId },
                { "groupId", display.Id }
            });
        }

        private async void DeleteGroupClicked(object sender, EventArgs e)
        {
            var display = (TeacherCourseViewModel.AssignmentGroupDisplay)((Button)sender).CommandParameter;
            bool confirm = await DisplayAlert(
                "Delete Group",
                $"Delete \"{display.Name}\"? Assignments in this group will become ungrouped.",
                "Delete", "Cancel");

            if (confirm)
                ViewModel.DeleteGroup(display.Id);
        }

        private async void AddAssignmentToGroupClicked(object sender, EventArgs e)
        {
            var display = (TeacherCourseViewModel.AssignmentGroupDisplay)((Button)sender).CommandParameter;

            var ungrouped = ViewModel.UngroupedAssignments.ToList();
            if (!ungrouped.Any())
            {
                await DisplayAlert("No Assignments", "There are no ungrouped assignments to add.", "OK");
                return;
            }

            var options = ungrouped.Select(a => a.Name).ToArray();
            var chosen = await DisplayActionSheet(
                $"Add to \"{display.Name}\"", "Cancel", null, options);

            if (chosen == null || chosen == "Cancel") return;

            var assignment = ungrouped.FirstOrDefault(a => a.Name == chosen);
            if (assignment != null)
                ViewModel.AddAssignmentToGroup(display.Id, assignment.Id);
        }
    }
}
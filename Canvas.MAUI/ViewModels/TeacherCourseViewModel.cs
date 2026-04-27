using Canvas.Library.Model;
using Canvas.Library.Services;
using Canvas.MAUI.Models;
using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Canvas.MAUI.ViewModels
{
    internal class TeacherCourseViewModel : INotifyPropertyChanged, IQueryAttributable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public int CourseId => _courseId;
        private int _courseId;

        public TeacherCourseViewModel()
        {
            Announcements = new ObservableCollection<Announcement>();
            Modules = new ObservableCollection<ModuleViewModel>();
            Assignments = new ObservableCollection<Assignment>();
            Roster = new ObservableCollection<Student>();
            AssignmentGroups = new ObservableCollection<AssignmentGroupDisplay>();
            UngroupedAssignments = new ObservableCollection<Assignment>();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("teacherId", out var tId) && int.TryParse(tId?.ToString(), out int parsedTeacherId))
                teacherId = parsedTeacherId;

            if (query.TryGetValue("sectionNumber", out var sNum) && int.TryParse(sNum?.ToString(), out int parsedSectionNumber))
                sectionNumber = parsedSectionNumber;

            if (query.TryGetValue("courseId", out var value) && int.TryParse(value?.ToString(), out int id))
            {
                _courseId = id;
                LoadCourse();
            }
        }

        private int sectionNumber;
        public int SectionNumber
        {
            get => sectionNumber;
            set { sectionNumber = value; OnPropertyChanged(); }
        }

        private int teacherId;
        public int TeacherId
        {
            get => teacherId;
            set { teacherId = value; OnPropertyChanged(); }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private string _code;
        public string Code
        {
            get => _code;
            set { _code = value; OnPropertyChanged(); }
        }

        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set { _selectedTabIndex = value; OnPropertyChanged(); }
        }

        public bool IsAnnouncementsTab => SelectedTabIndex == 0;
        public bool IsModulesTab => SelectedTabIndex == 1;
        public bool IsAssignmentsTab => SelectedTabIndex == 2;
        public bool IsRosterTab => SelectedTabIndex == 3;

        private bool _showAnnouncementForm;
        public bool ShowAnnouncementForm
        {
            get => _showAnnouncementForm;
            set { _showAnnouncementForm = value; OnPropertyChanged(); }
        }

        private string _newAnnouncementTitle;
        public string NewAnnouncementTitle
        {
            get => _newAnnouncementTitle;
            set { _newAnnouncementTitle = value; OnPropertyChanged(); }
        }

        private string _newAnnouncementBody;
        public string NewAnnouncementBody
        {
            get => _newAnnouncementBody;
            set { _newAnnouncementBody = value; OnPropertyChanged(); }
        }

        private bool _showModuleForm;
        public bool ShowModuleForm
        {
            get => _showModuleForm;
            set { _showModuleForm = value; OnPropertyChanged(); }
        }

        private string _newModuleName;
        public string NewModuleName
        {
            get => _newModuleName;
            set { _newModuleName = value; OnPropertyChanged(); }
        }

        private bool _showEnrollForm;
        public bool ShowEnrollForm
        {
            get => _showEnrollForm;
            set { _showEnrollForm = value; OnPropertyChanged(); }
        }

        private string _enrollStudentCode;
        public string EnrollStudentCode
        {
            get => _enrollStudentCode;
            set { _enrollStudentCode = value; OnPropertyChanged(); }
        }

        private string _enrollError;
        public string EnrollError
        {
            get => _enrollError;
            set
            {
                _enrollError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasEnrollError));
            }
        }
        public bool HasEnrollError => !string.IsNullOrEmpty(EnrollError);

        public ObservableCollection<Announcement> Announcements { get; set; }
        public ObservableCollection<ModuleViewModel> Modules { get; set; }
        public ObservableCollection<Assignment> Assignments { get; set; }
        public ObservableCollection<Student> Roster { get; set; }
        public ObservableCollection<AssignmentGroupDisplay> AssignmentGroups { get; set; }
        public ObservableCollection<Assignment> UngroupedAssignments { get; set; }
        public bool HasUngroupedAssignments => UngroupedAssignments?.Count > 0;

        public void LoadCourse()
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
            if (course == null) return;

            Name = course.Name ?? string.Empty;
            Code = course.Code ?? string.Empty;

            Assignments.Clear();
            foreach (var a in course.Assignments ?? new List<Assignment>())
                Assignments.Add(a);
            Announcements.Clear();
            foreach (var a in course.Announcements ?? new List<Announcement>())
                Announcements.Add(a);
            Roster.Clear();
            foreach (var s in course.Roster ?? new List<Student>())
                Roster.Add(s);
            
            RefreshModules();
            Modules = new ObservableCollection<ModuleViewModel>(
                course.Modules?.Select(m => new ModuleViewModel
                {
                    Id = m.Id,
                    ModuleName = m.ModuleName,
                    Content = m.Content,
                    ModuleContents = m.ModuleContents ?? new List<ModuleContent>()
                }) ?? Enumerable.Empty<ModuleViewModel>()
            );

            foreach (var module in Modules)
            {
                foreach (var content in module.ModuleContents)
                {
                    if (content is AssignmentContent ac)
                    {
                        var assignment = course.Assignments?.FirstOrDefault(a => a.Id == ac.AssignmentId);
                        if (assignment != null)
                            ac.Name = assignment.Name;
                    }
                }
            }

            OnPropertyChanged(nameof(Announcements));
            OnPropertyChanged(nameof(Modules));
            OnPropertyChanged(nameof(Assignments));
            OnPropertyChanged(nameof(Roster));
            OnPropertyChanged(nameof(FilteredRoster));

            // only new additions
            RefreshAssignments();
            RefreshGroups();
        }

        public void SelectTab(int index)
        {
            SelectedTabIndex = index;
            OnPropertyChanged(nameof(IsAnnouncementsTab));
            OnPropertyChanged(nameof(IsModulesTab));
            OnPropertyChanged(nameof(IsAssignmentsTab));
            OnPropertyChanged(nameof(IsRosterTab));
        }

        public void ToggleAnnouncementForm()
            => ShowAnnouncementForm = !ShowAnnouncementForm;

        public void SubmitAnnouncement()
        {
            if (string.IsNullOrWhiteSpace(NewAnnouncementTitle)) return;
            var announcement = new Announcement
            {
                Title = NewAnnouncementTitle,
                Body = NewAnnouncementBody,
                PostedDate = DateTime.Now
            };
            CourseServiceProxy.Current.AddAnnouncement(_courseId, announcement);
            Announcements.Add(announcement);
            NewAnnouncementTitle = string.Empty;
            NewAnnouncementBody = string.Empty;
            ShowAnnouncementForm = false;
        }

        public void DeleteAnnouncement(Announcement announcement)
        {
            CourseServiceProxy.Current.DeleteAnnouncement(_courseId, announcement.Id);
            Announcements.Remove(announcement);
        }

        public void UpdateAnnouncement(Announcement announcement)
        {
            CourseServiceProxy.Current.UpdateAnnouncement(_courseId, announcement);
        }

        public void ToggleModuleForm()
            => ShowModuleForm = !ShowModuleForm;

        public void SubmitModule()
        {
            if (string.IsNullOrWhiteSpace(NewModuleName)) return;
            CourseServiceProxy.Current.AddModule(_courseId, NewModuleName);
            RefreshModules();
            NewModuleName = string.Empty;
            ShowModuleForm = false;
        }

        public void DeleteModule(ModuleViewModel module)
        {
            CourseServiceProxy.Current.DeleteModule(_courseId, module.Id);
            LoadCourse();
        }

        public void AddModuleContent(int moduleId, string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return;
            CourseServiceProxy.Current.AddModuleContent(_courseId, moduleId, content);
            RefreshModules();
        }

        public void DeleteModuleContent(int moduleId, ModuleContent content)
        {
            if (content is AssignmentContent a)
                CourseServiceProxy.Current.DeleteAssignment(_courseId, a.AssignmentId);

            CourseServiceProxy.Current.DeleteModuleContents(_courseId, moduleId, content.Id);
            RefreshModules();
        }

        public void UpdateModuleName(int moduleId, string newName)
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
            var module = course?.Modules?.FirstOrDefault(m => m.Id == moduleId);
            if (module == null) return;
            module.ModuleName = newName;
            RefreshModules();
        }

        public void UpdateModuleContent(int moduleId, int contentIndex, string newContent)
        {
            CourseServiceProxy.Current.UpdateModuleContent(_courseId, moduleId, contentIndex, newContent);
            RefreshModules();
        }

        private async void RefreshModules()
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
            var assignments = course?.Assignments ?? new List<Assignment>();

            Modules = new ObservableCollection<ModuleViewModel>(
                (course?.Modules ?? new List<Module>()).Select(m => new ModuleViewModel
                {
                    Id = m.Id,
                    ModuleName = m.ModuleName,
                    Content = m.Content ?? new List<string>(),
                    ModuleContents = m.ModuleContents ?? new List<ModuleContent>(),
                    IsExpanded = true
                })
            );


            foreach (var module in Modules)
                foreach (var content in module.ModuleContents)
                    if (content is AssignmentContent ac)
                    {
                        var assignment = assignments.FirstOrDefault(a => a.Id == ac.AssignmentId);
                        if (assignment != null)
                            ac.Name = assignment.Name;
                    }

            OnPropertyChanged(nameof(Modules));
        }

        public void DeleteAssignment(Assignment assignment)
        {
            // proxy now handles group cleanup internally
            CourseServiceProxy.Current.DeleteAssignment(_courseId, assignment.Id);
            DeleteAssignmentFromModuleContents(assignment.Id);
            RefreshAssignments();
            RefreshGroups();
        }

        private void DeleteAssignmentFromModuleContents(int assignmentId)
        {
            var course = CourseServiceProxy.Current.Courses?.FirstOrDefault(c => c.Id == _courseId);
            if (course == null) return;

            foreach (var module in course.Modules ?? new List<Module>())
            {
                var match = module.ModuleContents?
                    .FirstOrDefault(c => c is AssignmentContent a && a.AssignmentId == assignmentId);

                if (match != null)
                    CourseServiceProxy.Current.DeleteModuleContents(_courseId, module.Id, match.Id);
            }
        }

        public void RefreshAssignments()
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
            Assignments = new ObservableCollection<Assignment>(course?.Assignments ?? new List<Assignment>());
            OnPropertyChanged(nameof(Assignments));
        }

        public void ToggleEnrollForm()
            => ShowEnrollForm = !ShowEnrollForm;

        public void EnrollStudent()
        {
            var student = StudentServiceProxy.Current.Students
                .FirstOrDefault(s => s.Code == EnrollStudentCode?.Trim());

            if (student == null)
            {
                EnrollError = "No student found with that code.";
                return;
            }

            if (Roster.Any(s => s.Id == student.Id))
            {
                EnrollError = "Student is already enrolled.";
                return;
            }

            CourseServiceProxy.Current.EnrollStudent(student.Id, _courseId);
            Roster.Add(student);
            OnPropertyChanged(nameof(FilteredRoster));
            EnrollStudentCode = string.Empty;
            EnrollError = string.Empty;
            ShowEnrollForm = false;
        }

        public void UnenrollStudent(Student student)
        {
            CourseServiceProxy.Current.UnenrollStudent(_courseId, student.Id);
            Roster.Remove(student);
            OnPropertyChanged(nameof(FilteredRoster));
        }

        public class AssignmentGroupDisplay
        {
            public AssignmentGroup Group { get; set; }
            public ObservableCollection<Assignment> GroupAssignments { get; set; }
            public bool IsEmpty => GroupAssignments?.Count == 0;
            public string Name => Group.Name;
            public int TotalPoints => Group.TotalPoints;
            public int Id => Group.Id;
        }

        public void AddOrUpdateGroup(AssignmentGroup group)
        {
            var result = CourseServiceProxy.Current.AddOrUpdateAssignmentGroup(_courseId, group);
            if (result != null)
                RefreshGroups();
        }

        public void DeleteGroup(int groupId)
        {
            CourseServiceProxy.Current.DeleteAssignmentGroup(_courseId, groupId);
            RefreshGroups();
        }

        public void AddAssignmentToGroup(int groupId, int assignmentId)
        {
            CourseServiceProxy.Current.AddAssignmentToGroup(_courseId, groupId, assignmentId);
            RefreshGroups();
        }

        public string BuildGradebookCsv()
        {
            var assignments = Assignments.ToList();
            var roster = Roster.ToList();

            var sb = new StringBuilder();

            sb.Append("Student,Code");
            foreach (var a in assignments)
                sb.Append($",{(a.Name ?? "").Replace(",", " ")}");
            sb.AppendLine(",Total Points,Total Available");

            foreach (var student in roster)
            {
                var earned = 0;
                var available = 0;

                sb.Append($"{(student.Name ?? "").Replace(",", " ")},{student.Code}");

                foreach (var a in assignments)
                {
                    var submission = a.Submissions?.FirstOrDefault(s => s.StudentId == student.Id);
                    var points = submission?.PointsAwarded ?? 0;
                    sb.Append($",{points}");
                    earned += points;
                    available += a.AvailablePoints;
                }

                sb.AppendLine($",{earned},{available}");
            }

            return sb.ToString();
        }

        public void RemoveAssignmentFromGroup(int groupId, int assignmentId)
        {
            CourseServiceProxy.Current.RemoveAssignmentFromGroup(_courseId, groupId, assignmentId);
            RefreshGroups();
        }
        private string _importStatus;
        public string ImportStatus
        {
            get => _importStatus;
            set { _importStatus = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasImportStatus)); }
        }
        public bool HasImportStatus => !string.IsNullOrEmpty(ImportStatus);

        public List<Assignment> ParseAssignmentCsv(string[] lines)
        {
            var assignments = new List<Assignment>();

            foreach (var line in lines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var cols = line.Split(',');
                if (cols.Length < 6) continue;

                var name        = cols[0].Trim();
                var dueDateStr  = cols[1].Trim();
                var pointsStr   = cols[2].Trim();
                var groupIdStr  = cols[3].Trim();
                var isQuizStr   = cols[4].Trim();
                var description = cols[5].Trim();

                if (string.IsNullOrWhiteSpace(name)) continue;
                if (!DateTime.TryParse(dueDateStr, out var dueDate)) continue;
                if (!int.TryParse(pointsStr, out var points)) continue;
                if (!int.TryParse(groupIdStr, out var groupId)) groupId = 0;
                if (!bool.TryParse(isQuizStr, out var isQuiz)) isQuiz = false;

                assignments.Add(new Assignment
                {
                    Name            = name,
                    DueDate         = dueDate,
                    AvailablePoints = points,
                    GroupId         = groupId,
                    IsQuiz          = isQuiz,
                    Description     = description
                });
            }

            return assignments;
        }

        public (int imported, int skipped) ImportAssignments(List<Assignment> assignments, int totalRows)
        {
            if (assignments.Count == 0) return (0, totalRows);
            var result = CourseServiceProxy.Current.ImportAssignments(_courseId, assignments);
            RefreshAssignments();
            RefreshGroups();
            return (result.Count, totalRows - result.Count);
        }

        private void RefreshGroups()
        {
            var groups = CourseServiceProxy.Current.GetAssignmentGroups(_courseId);
            var allAssignments = CourseServiceProxy.Current.Courses
                .FirstOrDefault(c => c.Id == _courseId)?.Assignments ?? new List<Assignment>();

            AssignmentGroups = new ObservableCollection<AssignmentGroupDisplay>(
                groups.Select(g => new AssignmentGroupDisplay
                {
                    Group = g,
                    GroupAssignments = new ObservableCollection<Assignment>(
                        allAssignments.Where(a => g.AssignmentIds.Contains(a.Id))
                    )
                })
            );

            UngroupedAssignments = new ObservableCollection<Assignment>(
                allAssignments.Where(a => a.GroupId == 0)
            );

            OnPropertyChanged(nameof(AssignmentGroups));
            OnPropertyChanged(nameof(UngroupedAssignments));
            OnPropertyChanged(nameof(HasUngroupedAssignments));
        }
        
        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FilteredRoster));
            }
        }

        public IEnumerable<Student> FilteredRoster =>
            string.IsNullOrWhiteSpace(SearchQuery)
                ? Roster
                : Roster.Where(s =>
                    (s.Name?.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (s.Code?.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ?? false));
    }
}
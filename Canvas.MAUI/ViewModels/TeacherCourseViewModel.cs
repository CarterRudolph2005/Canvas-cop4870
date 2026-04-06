using Canvas.Library.Model;
using Canvas.Library.Services;
using Canvas.MAUI.Models;
using System;
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
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("teacherId", out var tId) && int.TryParse(tId?.ToString(), out int _teacherId))
            {
                teacherId = _teacherId;
            }
            if (query.TryGetValue("sectionNumber", out var sNum) && int.TryParse(tId?.ToString(), out int _sectionNumber))
            {
                sectionNumber = _sectionNumber;
            }
            if (query.TryGetValue("courseId", out var value) && int.TryParse(value?.ToString(), out int id))
            {
                _courseId = id;
                LoadCourse();
            }
        }

        // ── Properties ─────────────────────────────────────────────

        private int sectionNumber;
        public int SectionNumber
        {
            get => sectionNumber;
            set{ sectionNumber = value; OnPropertyChanged(); }
        }
        private int teacherId;
        public int TeacherId
        {
            get => teacherId;
            set{ teacherId = value; OnPropertyChanged(); }
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

        // Tab visibility
        public bool IsAnnouncementsTab => SelectedTabIndex == 0;
        public bool IsModulesTab => SelectedTabIndex == 1;
        public bool IsAssignmentsTab => SelectedTabIndex == 2;
        public bool IsRosterTab => SelectedTabIndex == 3;

        // Announcement form
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

        // Module form
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

        // Enroll form
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

        // ── Load ───────────────────────────────────────────────────

        public void LoadCourse()
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
            if (course == null) return;

            Name = course.Name ?? string.Empty;
            Code = course.Code ?? string.Empty;
            Announcements = new ObservableCollection<Announcement>(course.Announcements ?? new List<Announcement>());
            Assignments = new ObservableCollection<Assignment>(course.Assignments ?? new List<Assignment>());
            Roster = new ObservableCollection<Student>(course.Roster ?? new List<Student>());

            Modules = new ObservableCollection<ModuleViewModel>(
                course.Modules?.Select(m => new ModuleViewModel
                {
                    Id = m.Id,
                    ModuleName = m.ModuleName,
                    Content = m.Content,
                    ModuleContents = m.ModuleContents ?? new List<ModuleContent>()
                }) ?? Enumerable.Empty<ModuleViewModel>()
            );

            // Resolve AssignmentContent names from actual assignments
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
        }

        private void RefreshModules()
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
            Modules = new ObservableCollection<ModuleViewModel>(
                (course?.Modules ?? new List<Module>()).Select(m => new ModuleViewModel
                {
                    Id = m.Id,
                    ModuleName = m.ModuleName,
                    Content = m.Content ?? new List<string>(),
                    IsExpanded = true
                })
            );
            OnPropertyChanged(nameof(Modules));
        }

        // ── Tab switching ──────────────────────────────────────────

        public void SelectTab(int index)
        {
            SelectedTabIndex = index;
            OnPropertyChanged(nameof(IsAnnouncementsTab));
            OnPropertyChanged(nameof(IsModulesTab));
            OnPropertyChanged(nameof(IsAssignmentsTab));
            OnPropertyChanged(nameof(IsRosterTab));
        }

        // ── Announcements ──────────────────────────────────────────

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

        // ── Modules ────────────────────────────────────────────────

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
            Modules.Remove(module);
        }

        public void AddModuleContent(int moduleId, string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return;
            CourseServiceProxy.Current.AddModuleContent(_courseId, moduleId, content);
            RefreshModules();
        }

        public void DeleteModuleContent(int moduleId, int contentIndex)
        {
            CourseServiceProxy.Current.DeleteModuleContent(_courseId, moduleId, contentIndex);
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

        // ── Assignments ────────────────────────────────────────────

        public void DeleteAssignment(Assignment assignment)
        {
            CourseServiceProxy.Current.DeleteAssignment(_courseId, assignment.Id);
            Assignments.Remove(assignment);
        }

        public void RefreshAssignments()
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
            Assignments = new ObservableCollection<Assignment>(course?.Assignments ?? new List<Assignment>());
            OnPropertyChanged(nameof(Assignments));
        }

        // ── Roster ─────────────────────────────────────────────────

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
            EnrollStudentCode = string.Empty;
            EnrollError = string.Empty;
            ShowEnrollForm = false;
        }

        public void UnenrollStudent(Student student)
        {
            CourseServiceProxy.Current.UnenrollStudent(_courseId, student.Id);
            Roster.Remove(student);
        }
    }
}
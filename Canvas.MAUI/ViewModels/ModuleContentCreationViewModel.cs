using Canvas.Library.Model;
using Canvas.Library.Services;
using Canvas.MAUI.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Canvas.MAUI.ViewModels
{
    internal class ModuleContentCreationViewModel : INotifyPropertyChanged, IQueryAttributable
    {
        public ModuleContentCreationViewModel()
        {
            SaveCommand = new Command(Save, CanSave);
            ContentTypes = new List<string> { "Page", "File", "Assignment" };
            SelectedContentType = "Page";
            AssignmentDueDate = DateTime.Now.AddDays(7);
        }

        private int courseId;
        private int moduleId;

        public List<string> ContentTypes { get; }

        private string selectedContentType;
        public string SelectedContentType
        {
            get => selectedContentType;
            set
            {
                selectedContentType = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsPage));
                OnPropertyChanged(nameof(IsFile));
                OnPropertyChanged(nameof(IsAssignment));
                ((Command)SaveCommand).ChangeCanExecute();
            }
        }

        public bool IsPage => SelectedContentType == "Page";
        public bool IsFile => SelectedContentType == "File";
        public bool IsAssignment => SelectedContentType == "Assignment";

        // shared
        private string contentName;
        public string ContentName
        {
            get => contentName;
            set { contentName = value; OnPropertyChanged(); ((Command)SaveCommand).ChangeCanExecute(); }
        }

        // page fields
        private string pageBody;
        public string PageBody
        {
            get => pageBody;
            set { pageBody = value; OnPropertyChanged(); }
        }

        // file fields
        private string filePath;
        public string FilePath
        {
            get => filePath;
            set { filePath = value; OnPropertyChanged(); ((Command)SaveCommand).ChangeCanExecute(); }
        }

        private string mimeType;
        public string MimeType
        {
            get => mimeType;
            set { mimeType = value; OnPropertyChanged(); }
        }

        // assignment fields
        private string assignmentName;
        public string AssignmentName
        {
            get => assignmentName;
            set { assignmentName = value; OnPropertyChanged(); ((Command)SaveCommand).ChangeCanExecute(); }
        }

        private string assignmentDescription;
        public string AssignmentDescription
        {
            get => assignmentDescription;
            set { assignmentDescription = value; OnPropertyChanged(); }
        }

        private string assignmentPoints;
        public string AssignmentPoints
        {
            get => assignmentPoints;
            set { assignmentPoints = value; OnPropertyChanged(); ((Command)SaveCommand).ChangeCanExecute(); }
        }

        private DateTime assignmentDueDate = DateTime.Now.AddDays(7);
        public DateTime AssignmentDueDate
        {
            get => assignmentDueDate;
            set { assignmentDueDate = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("courseId", out var cId) && int.TryParse(cId?.ToString(), out int _courseId))
                courseId = _courseId;

            if (query.TryGetValue("moduleId", out var mId) && int.TryParse(mId?.ToString(), out int _moduleId))
                moduleId = _moduleId;
        }

        private bool CanSave()
        {
            if (string.IsNullOrWhiteSpace(ContentName)) return false;

            return SelectedContentType switch
            {
                "File"       => !string.IsNullOrWhiteSpace(FilePath),
                "Assignment" => !string.IsNullOrWhiteSpace(AssignmentName) &&
                                !string.IsNullOrWhiteSpace(AssignmentPoints) &&
                                int.TryParse(AssignmentPoints, out _),
                _            => true // Page only needs a name
            };
        }


        private void Save()
        {
            TrySave();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public bool TrySave()
        {
            if (string.IsNullOrWhiteSpace(ContentName)) return false;
            switch (SelectedContentType)
            {
                case "Page":
                    return CourseServiceProxy.Current.AddModuleContents(courseId, moduleId, new PageContent
                    {
                        Name = ContentName,
                        Body = PageBody ?? string.Empty
                    });

                case "File":
                    if (string.IsNullOrWhiteSpace(FilePath)) return false;
                    return CourseServiceProxy.Current.AddModuleContents(courseId, moduleId, new FileContent
                    {
                        Name = ContentName,
                        FilePath = FilePath,
                        MimeType = MimeType ?? "application/octet-stream"
                    });

                case "Assignment":
                    if (string.IsNullOrWhiteSpace(AssignmentName) ||
                        string.IsNullOrWhiteSpace(AssignmentPoints) ||
                        !int.TryParse(AssignmentPoints, out int points)) return false;

                    var newAssignment = new Assignment
                    {
                        Id = 0,
                        Name = AssignmentName,
                        Description = AssignmentDescription ?? string.Empty,
                        AvailablePoints = points,
                        DueDate = AssignmentDueDate
                    };

                    newAssignment = CourseServiceProxy.Current.AddOrUpdateAssignment(courseId, newAssignment);
                    var moduleAssignmentRef = new AssignmentContent
                    {
                    Name = newAssignment.Name,
                    Body = newAssignment.Description,
                    AssignmentId = newAssignment.Id  
                    };
                    return CourseServiceProxy.Current.AddModuleContents(courseId, moduleId, moduleAssignmentRef);

                default:
                    return false;
            }
        }

    }
}
using Canvas.Library.Model;
using Canvas.Library.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Canvas.MAUI.ViewModels
{
    internal class AssignmentGroupViewModel : INotifyPropertyChanged, IQueryAttributable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private int _courseId;
        private int _groupId; // 0 = creating new, >0 = editing existing

        public bool IsEditing => _groupId != 0;
        public string Title => IsEditing ? "Edit Group" : "New Group";

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanSave)); }
        }

        private string _totalPointsText;
        public string TotalPointsText
        {
            get => _totalPointsText;
            set { _totalPointsText = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanSave)); }
        }

        private string _validationError;
        public string ValidationError
        {
            get => _validationError;
            set
            {
                _validationError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasValidationError));
            }
        }
        public bool HasValidationError => !string.IsNullOrEmpty(ValidationError);

        public bool CanSave =>
            !string.IsNullOrWhiteSpace(Name) &&
            int.TryParse(TotalPointsText, out int pts) &&
            pts > 0;


        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("courseId", out var cId) && int.TryParse(cId?.ToString(), out int courseId))
                _courseId = courseId;

            if (query.TryGetValue("groupId", out var gId) && int.TryParse(gId?.ToString(), out int groupId))
            {
                _groupId = groupId;
                LoadExistingGroup();
            }

            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(IsEditing));
        }

        private void LoadExistingGroup()
        {
            var group = CourseServiceProxy.Current.GetAssignmentGroupById(_courseId, _groupId);
            if (group == null) return;

            Name = group.Name;
            TotalPointsText = group.TotalPoints.ToString();
        }


        public bool TrySave(out AssignmentGroup savedGroup)
        {
            savedGroup = null;
            ValidationError = string.Empty;

            if (string.IsNullOrWhiteSpace(Name))
            {
                ValidationError = "Group name is required.";
                return false;
            }

            if (!int.TryParse(TotalPointsText, out int totalPoints) || totalPoints <= 0)
            {
                ValidationError = "Total points must be a positive number.";
                return false;
            }

            var group = new AssignmentGroup
            {
                Id = _groupId, // 0 = new, >0 = update
                CourseId = _courseId,
                Name = Name.Trim(),
                TotalPoints = totalPoints
            };

            // preserve existing assignment IDs when editing
            if (IsEditing)
            {
                var existing = CourseServiceProxy.Current.GetAssignmentGroupById(_courseId, _groupId);
                if (existing != null)
                    group.AssignmentIds = new List<int>(existing.AssignmentIds);
            }

            savedGroup = CourseServiceProxy.Current.AddOrUpdateAssignmentGroup(_courseId, group);
            return savedGroup != null;
        }
    }
}
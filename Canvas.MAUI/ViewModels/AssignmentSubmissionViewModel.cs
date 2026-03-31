using Canvas.Library.Model;
using Canvas.Library.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Canvas.MAUI.ViewModels
{
    internal class AssignmentSubmissionViewModel : INotifyPropertyChanged, IQueryAttributable
    {
        public AssignmentSubmissionViewModel()
        {
            SubmitCommand = new Command(Submit, CanSubmit);
        }

        private int courseId;
        private int assignmentId;
        private Submission existingSubmission;

        private int studentId;
        public int StudentId
        {
            get => studentId;
            set { studentId = value; OnPropertyChanged(); }
        }

        private string assignmentName;
        public string AssignmentName
        {
            get => assignmentName;
            set { assignmentName = value; OnPropertyChanged(); }
        }

        private string description;
        public string Description
        {
            get => description;
            set { description = value; OnPropertyChanged(); }
        }

        private double availablePoints;
        public double AvailablePoints
        {
            get => availablePoints;
            set { availablePoints = value; OnPropertyChanged(); }
        }

        private string submissionContent;
        public string SubmissionContent
        {
            get => submissionContent;
            set
            {
                submissionContent = value;
                OnPropertyChanged();
                ((Command)SubmitCommand).ChangeCanExecute();
            }
        }

        private bool isSubmitted;
        public bool IsSubmitted
        {
            get => isSubmitted;
            set { isSubmitted = value; OnPropertyChanged(); }
        }

        private DateTime? submittedAt;
        public DateTime? SubmittedAt
        {
            get => submittedAt;
            set { submittedAt = value; OnPropertyChanged(); }
        }

        // Computed — no backing field, manually notify when existingSubmission changes
        public string PointsDisplay =>
            existingSubmission == null ? string.Empty :
            existingSubmission.PointsAwarded.HasValue
                ? $"{existingSubmission.PointsAwarded} / {AvailablePoints} pts"
                : "Pending";

        public ICommand SubmitCommand { get; }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("studentId", out var sId) && int.TryParse(sId?.ToString(), out int _studentId))
                StudentId = _studentId;

            if (query.TryGetValue("courseId", out var cId) && int.TryParse(cId?.ToString(), out int _courseId))
                courseId = _courseId;

            if (query.TryGetValue("assignmentId", out var aId) && int.TryParse(aId?.ToString(), out int _assignmentId))
            {
                assignmentId = _assignmentId;
                LoadAssignment();
            }
        }

        private void LoadAssignment()
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return;

            var assignment = course.Assignments?.FirstOrDefault(a => a.Id == assignmentId);
            if (assignment == null) return;

            AssignmentName = assignment.Name ?? string.Empty;
            Description = assignment.Description ?? string.Empty;
            AvailablePoints = assignment.AvailablePoints;

            existingSubmission = assignment.Submissions?
                .FirstOrDefault(s => s.StudentId == StudentId);

            if (existingSubmission != null)
            {
                SubmissionContent = existingSubmission.Content;
                SubmittedAt = existingSubmission.SubmissionDate;
                IsSubmitted = true;
                OnPropertyChanged(nameof(PointsDisplay));
            }
        }

        private bool CanSubmit() =>
            !string.IsNullOrWhiteSpace(SubmissionContent) && !IsSubmitted;

        private void Submit()
        {
            existingSubmission = new Submission
            {
                StudentId = StudentId,
                AssignmentId = assignmentId,
                Content = SubmissionContent,
                SubmissionDate = DateTime.Now
            };

            CourseServiceProxy.Current.SubmitAssignment(courseId, existingSubmission);

            SubmittedAt = existingSubmission.SubmissionDate;
            IsSubmitted = true;
            OnPropertyChanged(nameof(PointsDisplay));
            ((Command)SubmitCommand).ChangeCanExecute();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
using Canvas.Library.Model;
using Canvas.Library.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Canvas.MAUI.ViewModels
{
    internal class GradeSubmissionViewModel : INotifyPropertyChanged, IQueryAttributable
    {
        private int _courseId;
        private int _assignmentId;

        private string _assignmentName = string.Empty;
        public string AssignmentName
        {
            get => _assignmentName;
            set { _assignmentName = value; OnPropertyChanged(); }
        }

        private int _availablePoints;
        public int AvailablePoints
        {
            get => _availablePoints;
            set { _availablePoints = value; OnPropertyChanged(); }
        }

        private List<Submission> _submissions = new();
        public List<Submission> Submissions
        {
            get => _submissions;
            set { _submissions = value; OnPropertyChanged(); }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("courseId", out var cId) && int.TryParse(cId?.ToString(), out int courseId))
                _courseId = courseId;

            if (query.TryGetValue("assignmentId", out var aId) && int.TryParse(aId?.ToString(), out int assignmentId))
            {
                _assignmentId = assignmentId;
                LoadAssignment();
            }
        }

        private void LoadAssignment()
        {
            var course = CourseServiceProxy.Current.Courses?.FirstOrDefault(c => c.Id == _courseId);
            if (course == null) return;

            var assignment = course.Assignments?.FirstOrDefault(a => a.Id == _assignmentId);
            if (assignment == null) return;

            AssignmentName = assignment.Name ?? string.Empty;
            AvailablePoints = assignment.AvailablePoints;
            Submissions = assignment.Submissions ?? new List<Submission>();
        }

        public void GradeSubmission(int submissionId, int pointsAwarded)
        {
            CourseServiceProxy.Current.GradeSubmission(_courseId, _assignmentId, submissionId, pointsAwarded);

            // Update local copy so picker reflects re-grade immediately
            var submission = Submissions.FirstOrDefault(s => s.Id == submissionId);
            if (submission != null)
                submission.PointsAwarded = pointsAwarded;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
        public int GetSelectedSubmissionId() => _selectedSubmissionId;

        private List<AssignmentComment> _comments = new();
        public List<AssignmentComment> Comments
        {
            get => _comments;
            set { _comments = value; OnPropertyChanged(); }
        }

        private string _newCommentBody = string.Empty;
        public string NewCommentBody
        {
            get => _newCommentBody;
            set { _newCommentBody = value; OnPropertyChanged(); }
        }

        private int _selectedSubmissionId;

        public void LoadComments(int submissionId)
        {
            _selectedSubmissionId = submissionId;
            Comments = CourseServiceProxy.Current.GetComments(submissionId);
        }

        public void AddComment(int authorId, string authorName)
        {
            if (string.IsNullOrWhiteSpace(NewCommentBody)) return;
            var comment = new AssignmentComment
            {
                SubmissionId = _selectedSubmissionId,
                AuthorId = authorId,
                AuthorName = authorName,
                Body = NewCommentBody
            };
            CourseServiceProxy.Current.AddComment(_selectedSubmissionId, comment);
            NewCommentBody = string.Empty;
            LoadComments(_selectedSubmissionId);
        }

        public void DeleteComment(int commentId)
        {
            CourseServiceProxy.Current.DeleteComment(_selectedSubmissionId, commentId);
            LoadComments(_selectedSubmissionId);
        }

    }
}
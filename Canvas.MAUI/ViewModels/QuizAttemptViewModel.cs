using Canvas.Library.Model;
using Canvas.Library.Services;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Canvas.MAUI.ViewModels
{
    public class QuizAttemptViewModel : INotifyPropertyChanged, IQueryAttributable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private int _courseId;
        private int _assignmentId;
        private int _studentId;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("courseId", out var cId) && int.TryParse(cId?.ToString(), out int parsedCourseId))
                _courseId = parsedCourseId;

            if (query.TryGetValue("assignmentId", out var aId) && int.TryParse(aId?.ToString(), out int parsedAssignmentId))
                _assignmentId = parsedAssignmentId;

            if (query.TryGetValue("studentId", out var sId) && int.TryParse(sId?.ToString(), out int parsedStudentId))
                _studentId = parsedStudentId;

            if (_courseId > 0 && _assignmentId > 0)
                LoadQuiz();
        }

        private string _quizName;
        public string QuizName
        {
            get => _quizName;
            set { _quizName = value; OnPropertyChanged(); }
        }

        private string _timeLimitText;
        public string TimeLimitText
        {
            get => _timeLimitText;
            set { _timeLimitText = value; OnPropertyChanged(); }
        }

        private bool _hasTimeLimit;
        public bool HasTimeLimit
        {
            get => _hasTimeLimit;
            set { _hasTimeLimit = value; OnPropertyChanged(); }
        }

        private bool _alreadySubmitted;
        public bool AlreadySubmitted
        {
            get => _alreadySubmitted;
            set
            {
                _alreadySubmitted = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanAttempt));
            }
        }

        public bool CanAttempt => !AlreadySubmitted;

        private string _submittedScoreText;
        public string SubmittedScoreText
        {
            get => _submittedScoreText;
            set { _submittedScoreText = value; OnPropertyChanged(); }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasError));
            }
        }
        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public ObservableCollection<QuizAttemptQuestionViewModel> Questions { get; set; } = new();

        private void LoadQuiz()
        {
            var course = CourseServiceProxy.Current.GetCoursesForStudent(_studentId)
                .FirstOrDefault(c => c.Id == _courseId);
            var assignment = course?.Assignments?.FirstOrDefault(a => a.Id == _assignmentId);
            if (assignment == null) return;

            QuizName = assignment.Name;

            var quiz = CourseServiceProxy.Current.GetQuiz(_courseId, _assignmentId);
            if (quiz == null) return;

            HasTimeLimit = quiz.TimeLimitMinutes.HasValue;
            TimeLimitText = quiz.TimeLimitMinutes.HasValue
                ? $"Time limit: {quiz.TimeLimitMinutes} minutes"
                : string.Empty;

            Questions.Clear();
            foreach (var q in quiz.Questions)
            {
                var qvm = new QuizAttemptQuestionViewModel
                {
                    QuestionId = q.Id,
                    QuestionText = q.QuestionText,
                    Points = q.Points
                };
                foreach (var o in q.Options)
                    qvm.Options.Add(new QuizAttemptOptionViewModel
                    {
                        OptionId = o.Id,
                        OptionText = o.OptionText,
                        IsCorrect = o.IsCorrect,
                        ParentQuestion = qvm
                    });
                Questions.Add(qvm);
            }

            // Check if already submitted
            var existing = CourseServiceProxy.Current.GetStudentSubmission(_courseId, _assignmentId, _studentId);
            if (existing != null)
            {
                AlreadySubmitted = true;
                SubmittedScoreText = existing.PointsAwarded.HasValue
                    ? $"Your score: {existing.PointsAwarded} / {assignment.AvailablePoints} pts"
                    : "Submitted — awaiting grade";
            }
        }

        public Task<bool> Submit()
        {
            ErrorMessage = string.Empty;

            // Ensure every question has a selection
            foreach (var q in Questions)
            {
                if (!q.Options.Any(o => o.IsSelected))
                {
                    ErrorMessage = $"Please answer: \"{q.QuestionText}\"";
                    return Task.FromResult(false);
                }
            }

            // Auto-grade: sum points for each question where selected option IsCorrect
            int earned = 0;
            foreach (var q in Questions)
            {
                var selected = q.Options.FirstOrDefault(o => o.IsSelected);
                if (selected != null && selected.IsCorrect)
                    earned += q.Points;
            }

            // Get total available points from the assignment
            var course = CourseServiceProxy.Current.GetCoursesForStudent(_studentId)
                .FirstOrDefault(c => c.Id == _courseId);
            var assignment = course?.Assignments?.FirstOrDefault(a => a.Id == _assignmentId);
            if (assignment == null)
            { ErrorMessage = "Could not find assignment."; return Task.FromResult(false); }

            // Submit and immediately grade
            var submission = new Submission
            {
                Id = 0,
                AssignmentId = _assignmentId,
                StudentId = _studentId,
                SubmissionDate = DateTime.UtcNow,
                PointsAwarded = earned
            };

            CourseServiceProxy.Current.SubmitAssignment(_courseId, submission);

            // Find the submission that was just created to grade it
            var saved = CourseServiceProxy.Current.GetStudentSubmission(_courseId, _assignmentId, _studentId);
            if (saved != null)
                CourseServiceProxy.Current.GradeSubmission(_courseId, _assignmentId, saved.Id, earned);

            AlreadySubmitted = true;
            SubmittedScoreText = $"Your score: {earned} / {assignment.AvailablePoints} pts";
            CourseServiceProxy.Current.InvalidateCache();
            return Task.FromResult(true);
        }
    }

    public class QuizAttemptQuestionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public int QuestionId { get; set; }

        private string _questionText;
        public string QuestionText
        {
            get => _questionText;
            set { _questionText = value; OnPropertyChanged(); }
        }

        public int Points { get; set; }

        public ObservableCollection<QuizAttemptOptionViewModel> Options { get; set; } = new();
    }

    public class QuizAttemptOptionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public int OptionId { get; set; }
        public string OptionText { get; set; }
        public bool IsCorrect { get; set; } // used for grading, not shown to student
        public QuizAttemptQuestionViewModel ParentQuestion { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value && ParentQuestion != null)
                    foreach (var opt in ParentQuestion.Options)
                        if (opt != this) opt.SetSelectedSilent(false);

                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public void SetSelectedSilent(bool val)
        {
            _isSelected = val;
            OnPropertyChanged(nameof(IsSelected));
        }
    }
}
using Canvas.Library.Model;
using Canvas.Library.Services;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Canvas.MAUI.ViewModels
{
    public class QuizEditorViewModel : INotifyPropertyChanged, IQueryAttributable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private int _courseId;
        private int _assignmentId; // 0 = creating new, >0 = editing existing

        public bool IsEditing => _assignmentId > 0;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("courseId", out var cId) && int.TryParse(cId?.ToString(), out int parsedCourseId))
                _courseId = parsedCourseId;

            if (query.TryGetValue("assignmentId", out var aId) && int.TryParse(aId?.ToString(), out int parsedAssignmentId))
            {
                _assignmentId = parsedAssignmentId;
                LoadExistingQuiz();
            }
        }

        private string _quizName;
        public string QuizName
        {
            get => _quizName;
            set { _quizName = value; OnPropertyChanged(); }
        }

        private string _quizDescription;
        public string QuizDescription
        {
            get => _quizDescription;
            set { _quizDescription = value; OnPropertyChanged(); }
        }

        private string _availablePointsText;
        public string AvailablePointsText
        {
            get => _availablePointsText;
            set { _availablePointsText = value; OnPropertyChanged(); }
        }

        private DateTime _dueDate = DateTime.Now.AddDays(7);
        public DateTime DueDate
        {
            get => _dueDate;
            set { _dueDate = value; OnPropertyChanged(); }
        }

        private string _timeLimitText;
        public string TimeLimitText
        {
            get => _timeLimitText;
            set { _timeLimitText = value; OnPropertyChanged(); }
        }

        private string _allowedAttemptsText = "1";
        public string AllowedAttemptsText
        {
            get => _allowedAttemptsText;
            set { _allowedAttemptsText = value; OnPropertyChanged(); }
        }

        public ObservableCollection<QuestionViewModel> Questions { get; set; } = new();

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

        private void LoadExistingQuiz()
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == _courseId);
            var assignment = course?.Assignments?.FirstOrDefault(a => a.Id == _assignmentId);
            if (assignment == null) return;

            QuizName = assignment.Name;
            QuizDescription = assignment.Description;
            AvailablePointsText = assignment.AvailablePoints.ToString();
            DueDate = assignment.DueDate;

            var quiz = CourseServiceProxy.Current.GetQuiz(_courseId, _assignmentId);
            if (quiz == null) return;

            TimeLimitText = quiz.TimeLimitMinutes?.ToString() ?? string.Empty;
            AllowedAttemptsText = quiz.AllowedAttempts.ToString();

            Questions.Clear();
            foreach (var q in quiz.Questions)
            {
                var qvm = new QuestionViewModel { Id = q.Id, QuestionText = q.QuestionText, PointsText = q.Points.ToString() };
                foreach (var o in q.Options)
                    qvm.Options.Add(new OptionViewModel { Id = o.Id, OptionText = o.OptionText, IsCorrect = o.IsCorrect });
                Questions.Add(qvm);
            }
        }

        public void AddQuestion()
        {
            Questions.Add(new QuestionViewModel());
        }

        public void RemoveQuestion(QuestionViewModel question)
        {
            Questions.Remove(question);
        }

        public void AddOption(QuestionViewModel question)
        {
            question.Options.Add(new OptionViewModel());
        }

        public void RemoveOption(QuestionViewModel question, OptionViewModel option)
        {
            question.Options.Remove(option);
        }

        public Task<bool> Save()
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(QuizName))
            { ErrorMessage = "Quiz name is required."; return Task.FromResult(false); }

            if (!int.TryParse(AvailablePointsText, out int points) || points < 0)
            { ErrorMessage = "Enter a valid point value."; return Task.FromResult(false); }

            if (!int.TryParse(AllowedAttemptsText, out int attempts) || attempts < 1)
            { ErrorMessage = "Allowed attempts must be at least 1."; return Task.FromResult(false); }

            int? timeLimit = null;
            if (!string.IsNullOrWhiteSpace(TimeLimitText))
            {
                if (!int.TryParse(TimeLimitText, out int tl) || tl < 1)
                { ErrorMessage = "Time limit must be a positive number of minutes."; return Task.FromResult(false); }
                timeLimit = tl;
            }

            foreach (var q in Questions)
            {
                if (string.IsNullOrWhiteSpace(q.QuestionText))
                { ErrorMessage = "All questions must have text."; return Task.FromResult(false); }
                if (!int.TryParse(q.PointsText, out int qpts) || qpts < 0)
                { ErrorMessage = $"Question \"{q.QuestionText}\" has an invalid point value."; return Task.FromResult(false); }
                if (q.Options.Count < 2)
                { ErrorMessage = $"Question \"{q.QuestionText}\" needs at least 2 options."; return Task.FromResult(false); }
                if (!q.Options.Any(o => o.IsCorrect))
                { ErrorMessage = $"Question \"{q.QuestionText}\" needs at least one correct answer marked."; return Task.FromResult(false); }
                foreach (var o in q.Options)
                    if (string.IsNullOrWhiteSpace(o.OptionText))
                    { ErrorMessage = $"All options on \"{q.QuestionText}\" must have text."; return Task.FromResult(false); }
            }

            if (IsEditing)
                return Task.FromResult(UpdateExisting(points, timeLimit, attempts));
            else
                return Task.FromResult(CreateNew(points, timeLimit, attempts));
        }

        private bool CreateNew(int points, int? timeLimit, int attempts)
        {
            var assignment = new Assignment
            {
                Id = 0,
                Name = QuizName,
                Description = QuizDescription ?? string.Empty,
                AvailablePoints = points,
                DueDate = DueDate,
                IsQuiz = true,
                GroupId = 0,
                Submissions = new List<Submission>()
            };
            var savedAssignment = CourseServiceProxy.Current.AddOrUpdateAssignment(_courseId, assignment);
            if (savedAssignment == null || savedAssignment.Id == 0)
            { ErrorMessage = "Failed to save quiz assignment."; return false; }

            var quiz = CourseServiceProxy.Current.CreateQuiz(_courseId, savedAssignment.Id, timeLimit, attempts);
            if (quiz == null)
            { ErrorMessage = "Failed to create quiz."; return false; }

            foreach (var qvm in Questions)
            {
                var question = new QuizQuestion
                {
                    QuestionText = qvm.QuestionText,
                    Points = int.Parse(qvm.PointsText),
                    Options = qvm.Options.Select(o => new QuizQuestionOption
                    {
                        OptionText = o.OptionText,
                        IsCorrect = o.IsCorrect
                    }).ToList()
                };
                CourseServiceProxy.Current.AddQuestion(_courseId, savedAssignment.Id, question);
            }

            CourseServiceProxy.Current.InvalidateCache();
            return true;
        }

        private bool UpdateExisting(int points, int? timeLimit, int attempts)
        {
            var assignment = new Assignment
            {
                Id = _assignmentId,
                Name = QuizName,
                Description = QuizDescription ?? string.Empty,
                AvailablePoints = points,
                DueDate = DueDate,
                IsQuiz = true,
                GroupId = 0
            };
            CourseServiceProxy.Current.AddOrUpdateAssignment(_courseId, assignment);
            CourseServiceProxy.Current.UpdateQuiz(_courseId, _assignmentId, timeLimit, attempts);

            var existingQuiz = CourseServiceProxy.Current.GetQuiz(_courseId, _assignmentId);
            if (existingQuiz != null)
                foreach (var q in existingQuiz.Questions.ToList())
                    CourseServiceProxy.Current.DeleteQuestion(_courseId, _assignmentId, q.Id);

            foreach (var qvm in Questions)
            {
                var question = new QuizQuestion
                {
                    QuestionText = qvm.QuestionText,
                    Points = int.Parse(qvm.PointsText),
                    Options = qvm.Options.Select(o => new QuizQuestionOption
                    {
                        OptionText = o.OptionText,
                        IsCorrect = o.IsCorrect
                    }).ToList()
                };
                CourseServiceProxy.Current.AddQuestion(_courseId, _assignmentId, question);
            }

            CourseServiceProxy.Current.InvalidateCache();
            return true;
        }
    }

    public class QuestionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public int Id { get; set; } // 0 = new

        private string _questionText;
        public string QuestionText
        {
            get => _questionText;
            set { _questionText = value; OnPropertyChanged(); }
        }

        private string _pointsText = "1";
        public string PointsText
        {
            get => _pointsText;
            set { _pointsText = value; OnPropertyChanged(); }
        }

        public ObservableCollection<OptionViewModel> Options { get; set; } = new();
    }

    public class OptionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public int Id { get; set; } // 0 = new

        private string _optionText;
        public string OptionText
        {
            get => _optionText;
            set { _optionText = value; OnPropertyChanged(); }
        }

        private bool _isCorrect;
        public bool IsCorrect
        {
            get => _isCorrect;
            set { _isCorrect = value; OnPropertyChanged(); }
        }
    }
}
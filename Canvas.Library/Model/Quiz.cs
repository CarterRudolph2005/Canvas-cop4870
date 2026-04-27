using System.Collections.Generic;

namespace Canvas.Library.Model
{
    public class Quiz
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public int? TimeLimitMinutes { get; set; }
        public int AllowedAttempts { get; set; } = 1;
        public List<QuizQuestion> Questions { get; set; } = new();
    }

    public class QuizQuestion
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string? QuestionText { get; set; }
        public int Points { get; set; }
        public List<QuizQuestionOption> Options { get; set; } = new();
    }

    public class QuizQuestionOption
    {
        public int Id { get; set; }
        public int QuizQuestionId { get; set; }
        public string? OptionText { get; set; }
        public bool IsCorrect { get; set; }
    }
}
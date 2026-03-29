namespace Canvas.Library.Model
{
    public class Grade
    {
        public int StudentId { get; set; }
        public int AssignmentId { get; set; }
        public int CourseId { get; set; }
        public int SubmissionId { get; set; }
        public int TotalAvailablePoints { get; set; }
        public int TotalEarnedPoints { get; set; }
        public double Percentage => TotalAvailablePoints > 0
            ? (double)TotalEarnedPoints / TotalAvailablePoints * 100
            : 0;
    }
}
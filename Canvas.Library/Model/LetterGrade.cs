namespace Canvas.Library.Model
{
    public class LetterGrade
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Letter { get; set; } = string.Empty;
        public double MinPercentage { get; set; }
        public double MaxPercentage { get; set; }
        public string HexColor { get; set; } = "#888888";
    }
}
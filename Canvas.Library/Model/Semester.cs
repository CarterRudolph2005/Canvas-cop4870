namespace Canvas.Library.Model
{
    public enum SemesterType
    {
        Fall,
        Spring,
        SummerA,
        SummerB,
        SummerC
    }
    public class Semester
    {
        public SemesterType Type {get; set;}
        public int Year {get; set;}
        public override string ToString() => Type switch
        {
            SemesterType.SummerA => $"Summer A {Year}",
            SemesterType.SummerB => $"Summer B {Year}",
            SemesterType.SummerC => $"Summer C {Year}",
            _ => $"{Type} {Year}"
        };
    }
}
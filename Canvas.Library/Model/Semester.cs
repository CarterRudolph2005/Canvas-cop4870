namespace Canvas.Library.Model
{
    public enum SemesterType
    {
        Spring,   // 0 - earliest in a year
        SummerA,  // 1
        SummerB,  // 2
        SummerC,  // 3
        Fall      // 4 - latest in a year
    }
    public class Semester
    {
        public SemesterType Session {get; set;}
        public int Year {get; set;}
        public Semester(int y, SemesterType s)
        {
            Year = y;
            Session = s;
        }
        public override string ToString() => Session switch
        {
            SemesterType.SummerA => $"Summer A {Year}",
            SemesterType.SummerB => $"Summer B {Year}",
            SemesterType.SummerC => $"Summer C {Year}",
            _ => $"{Session} {Year}"
        };
    }
}
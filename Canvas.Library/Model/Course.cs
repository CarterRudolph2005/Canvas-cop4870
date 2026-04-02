/*
Course
    - Id
    - Code
    - Name
    - Description
    - Roster (list of students)
    - Modules (list of modules)
    - Assignments (list of assignments)
*/

using System.Collections.Generic;


namespace Canvas.Library.Model
{
    public class Course
    {
        public int Id {get; set;}
        public string? Code {get; set;}
        public string? Name {get; set;}
        public string? Description {get; set;}
        public List<Announcement> Announcements {get; set;}
        public List<Student>? Roster {get; set;}
        public List<Module>? Modules {get; set;}
        public List<Instructor>? Instructors { get; set; }
        public List<Assignment>? Assignments{get; set;} = new List<Assignment>();
        public int SectionNumber {get; set;}
        public Semester? SemesterTaught {get; set;} //required
        public string FullCourseHeader => $"{Code} {SectionNumber:D4}";
        //what printing a course will look like
        public override string ToString()
        {
            return $"{Id}. {Name} - {Code}: {Description}";
        }

        //for UI to find override
        public string Display => ToString() ?? string.Empty;
    }
}
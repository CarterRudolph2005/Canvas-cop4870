/*
Student Model
Student should be derived from User and add the following properties:
    - Classification
*/

namespace Canvas.Library.Model
{
    public enum Classification
    {
        None, Freshman, Sophomore, Junior, Senior, NonDegree
    }    

    public class Student: User
    {
        public Classification Classification{get; set;}
        public Student() {}
        public string Email { get; set; } = string.Empty;
        public Student(Student student)
        {
            this.Id = student.Id;
            this.Code = student.Code;
            this.Name = student.Name;
            this.Classification = student.Classification;
        }
       public override string ToString()
        {
            return $"{Id}. {Name} - {Code}: {Classification}";
        }
        //for UI to find override
        public string Display => ToString() ?? string.Empty;
    }
    
}
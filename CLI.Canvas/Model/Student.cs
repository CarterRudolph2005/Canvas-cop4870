/*
Student Model
Student should be derived from User and add the following properties:
    - Classification
*/

namespace CLI.Canvas.Model
{
    public enum Classification
        {
            None, Freshman, Sophomore, Junior, Senior, NonDegree
        }
    public class Student: User
    {
        public Classification Classification{get; set;}

       public override string ToString()
        {
            return $"{Id}. {Name} - {Code}: {Classification}";
        }
        //for UI to find override
        public string Display => ToString() ?? string.Empty;
    }
    
}
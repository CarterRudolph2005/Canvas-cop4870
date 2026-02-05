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

       
    }
}
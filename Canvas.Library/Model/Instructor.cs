/*
Instructor Model
Instructor should be derived from User and add the following properties:
    - YearsOfExperience
*/

namespace Canvas.Library.Model
{
    public class Instructor: User
    {
        public int YearsOfExperience{get; set;}
    }
}
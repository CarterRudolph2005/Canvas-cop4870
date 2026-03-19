using System.Runtime.InteropServices;
using Canvas.Library.Model;

namespace Canvas.Library.Services
{
    public class AssignmentServiceProxy
    {
        private static AssignmentServiceProxy? instance;
        private static readonly object instanceLock = new object();
        public static AssignmentServiceProxy Current
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new AssignmentServiceProxy();
                    }
                }
                return instance;
            }
        }
        public bool AddAssignment(int CourseID, Assignment assignment)
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(i => i.Id == CourseID);
            if (course == null) {return false;}
            if (assignment.Id == 0)
            {
                course.Assignments ??= new List<Assignment>();
                assignment.Id = NextKey(course);
                course.Assignments.Add(assignment);
            }
            return true;
        }

        private int NextKey(Course course) 
        {
            if (course.Assignments != null && course.Assignments.Any()) {
                return course.Assignments.Max(a => a.Id) + 1;
            }
            return 1;
        }

        public bool UpdateAssignment(int CourseID, int AssignmentID, Assignment assignmentClone)
        {
            var course = CourseServiceProxy.Current.Courses.FirstOrDefault(i => i.Id == CourseID);
            if (course == null) {return false;}
            var assignment = course.Assignments?.FirstOrDefault(i => i.Id == AssignmentID);
            if (assignment == null) {return false;}
            else
            {
                assignment.Name = assignmentClone.Name;
                assignment.Description = assignmentClone.Description;
                assignment.AvailablePoints = assignmentClone.AvailablePoints;
                assignment.DueDate = assignmentClone.DueDate;
                return true;   
            }
        }
    }
}
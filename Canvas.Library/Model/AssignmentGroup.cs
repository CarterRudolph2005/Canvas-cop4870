/*
AssignmentGroup
    - Id
    - CourseId
    - Name
    - TotalPoints (configured on creation, drives grade calculation)
    - AssignmentIds (list of assignment IDs belonging to this group)

    Grade Calculation:
        (sum of earned points across assignments / sum of available points across assignments)
        * TotalPoints = awarded points from this group
*/
using System.Collections.Generic;
namespace Canvas.Library.Model
{
    public class AssignmentGroup
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string? Name { get; set; }
        public int TotalPoints { get; set; }
        public List<int> AssignmentIds { get; set; } = new List<int>();
        public bool IsEmpty => AssignmentIds.Count == 0;

        public AssignmentGroup() {}

        public AssignmentGroup(AssignmentGroup other)
        {
            this.Id = other.Id;
            this.CourseId = other.CourseId;
            this.Name = other.Name;
            this.TotalPoints = other.TotalPoints;
            this.AssignmentIds = new List<int>(other.AssignmentIds);
        }
    }
}
/*
Submission
    - Id
    - StudentId
    - AssignmentId
    - Content (string)
    - SubmissionDate
*/

using System;
using System.Linq.Expressions;
using System.Net.Mime;

namespace Canvas.Library.Model
{
    public class Submission
    {
        public int Id {get; set;}
        public int StudentId {get; set;}
        public int AssignmentId {get; set;}
        public string? Content {get; set;}
        public DateTime SubmissionDate {get; set;}
        public List<AssignmentComment> Comments { get; set; } = new();
        public int? PointsAwarded {get; set;}

        public override string ToString()
        {
            string gradeDisplay = PointsAwarded.HasValue ? $"{PointsAwarded}" : "Pending";
            return $"[{Id}] Student {StudentId}: Points Awarded: {gradeDisplay}";
        }

    }
}
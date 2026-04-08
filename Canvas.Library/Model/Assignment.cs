/*
Assignment
    - Id
    - Name
    - Description
    - Available Points
    - DueDate
    - Submissions (list of submissions)
    - GroupId (0 = ungrouped)
*/
using System;
using System.Collections.Generic;
namespace Canvas.Library.Model
{
    public class Assignment
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int AvailablePoints { get; set; }
        public DateTime DueDate { get; set; }
        public List<Submission>? Submissions { get; set; }
        public int GroupId { get; set; } = 0; // 0 = ungrouped

        public Assignment() {}

        public Assignment(Assignment other)
        {
            this.Id = other.Id;
            this.Name = other.Name;
            this.Description = other.Description;
            this.DueDate = other.DueDate;
            this.AvailablePoints = other.AvailablePoints;
            this.GroupId = other.GroupId;
        }

        public override string ToString()
        {
            return $"Id:{Id}. {Name} - {Description} (Available points: {AvailablePoints})";
        }

        public string Display => ToString() ?? string.Empty;
    }
}
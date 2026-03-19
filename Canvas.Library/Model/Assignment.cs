/*
Assignment
    - Id
    - Name
    - Description
    - Available Points
    - DueDate
    - Submissions (list of submissions)
*/

using System;
using System.Collections.Generic;

namespace Canvas.Library.Model
{
    public class Assignment
    {
        public int Id {get; set;}
        public string? Name {get; set;}
        public string? Description {get; set;}
        public int AvailablePoints {get; set;}
        public DateTime DueDate {get; set;}
        public List<Submission>? Submissions {get; set;}
        public Assignment() {}                                  //required if implementing the copy constructor
        public Assignment(Assignment other)                     //copy constructor for cloning implementation
        {
            this.Id = other.Id;
            this.Name = other.Name;
            this.Description = other.Description;
            this.DueDate = other.DueDate;
            this.AvailablePoints = other.AvailablePoints;
        }
        public override string ToString()
        {
            return $"Id:{Id}. {Name} - {Description} (Available points: {AvailablePoints})";
        }

        //for UI to find override
        public string Display => ToString() ?? string.Empty;
    }
}
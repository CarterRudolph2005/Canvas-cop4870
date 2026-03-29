using Canvas.Library.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Canvas.Library.Services
{
    public class InstructorServiceProxy
    {
        public Instructor CurrentInstructor { get; set; }

        private List<Instructor> instructors;
        public List<Instructor> Instructors
        {
            get
            {
                return instructors;
            }
            set
            {
                if (instructors != value)
                {
                    instructors = value;
                }
            }
        }

        private static InstructorServiceProxy? instance;
        private static object instanceLock = new object();

        public static InstructorServiceProxy Current
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new InstructorServiceProxy();
                    }
                }
                return instance;
            }
        }

        private InstructorServiceProxy()
        {
            instructors = new List<Instructor>
            {
                new Instructor { Id = 1, Name = "Dr. Smith",   YearsOfExperience = 10 },
                new Instructor { Id = 2, Name = "Prof. Jones", YearsOfExperience = 5  },
                new Instructor { Id = 3, Name = "Dr. Taylor",  YearsOfExperience = 15 },
            };
        }

        public void ProxyAs(int id)
        {
            CurrentInstructor = Instructors.FirstOrDefault(i => i.Id == id);
        }

        public int NextKey
        {
            get
            {
                if (Instructors.Any())
                {
                    return Instructors.Select(i => i.Id).Max() + 1;
                }
                return 1;
            }
        }

        public Instructor? GetById(int id)
        {
            if (id == 0) return null;
            return Instructors.FirstOrDefault(i => i.Id == id);
        }

        public void AddOrUpdate(Instructor? instructor)
        {
            if (instructor == null) return;

            if (instructor.Id == 0)
            {
                instructor.Id = NextKey;
                Instructors.Add(instructor);
            }
            else
            {
                var existing = Instructors.FirstOrDefault(i => i.Id == instructor.Id);
                if (existing != null)
                {
                    existing.Name = instructor.Name;
                    existing.YearsOfExperience = instructor.YearsOfExperience;
                }
            }
        }

        public Task<Instructor> DeleteInstructor(int instructorId)
        {
            int index = Instructors.FindIndex(i => i.Id == instructorId);
            if (index >= 0)
            {
                var instructor = new Instructor
                {
                    Id = Instructors[index].Id,
                    Name = Instructors[index].Name,
                    YearsOfExperience = Instructors[index].YearsOfExperience
                };
                Instructors.RemoveAt(index);
                return Task.FromResult(instructor);
            }
            return Task.FromResult<Instructor>(null);
        }

        public List<Course> GetCoursesForInstructor(int instructorId)
        {
            return CourseServiceProxy.Current.Courses
                .Where(c => c.Instructors != null && c.Instructors.Any(i => i.Id == instructorId))
                .ToList();
        }
    }
}
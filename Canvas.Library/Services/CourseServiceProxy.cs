using Canvas.Library.Model;
// using Microsoft.VisualBasic;
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.ComponentModel;
// using System.Data.Common;
// using System.Net;
// using System.Net.ServerSentEvents;
// using System.Reflection;
// using System.Text;

namespace Canvas.Library.Services
{
    public class CourseServiceProxy
    {
        private List<Course> courses;

        public List<Course> Courses
        {
            get
            {
                return courses;
            }

            set
            {
                if(courses != value)
                {
                    courses = value;
                }
            }
        }
        private static CourseServiceProxy? instance;
        private static object instanceLock = new object();

        public static CourseServiceProxy Current
        {
            get
            {
                lock (instanceLock)
                {
                    if(instance == null)
                    {
                        instance = new CourseServiceProxy();
                    }
                }
                return instance;
            }
        }
        private CourseServiceProxy()
        {
            courses = new List<Course>
            {
                    new Course
                    {
                        Id = 1, 
                        Code = "COP4530", 
                        Name = "Data Structures II", 
                        Description = "Advanced data structures and algorithmic analysis.",
                        // 1. Pre-populated Assignments with varying Due Dates
                        Assignments = new List<Assignment>
                        {
                            new Assignment { 
                                Id = 1, 
                                Name = "Binary Search Tree Lab", 
                                Description = "Implement a thread-safe BST.", 
                                AvailablePoints = 100, 
                                DueDate = new DateTime(2026, 3, 10) // Past date
                            },
                            new Assignment { 
                                Id = 2, 
                                Name = "B-Tree Research Paper", 
                                Description = "Analyze disk-based data structures.", 
                                AvailablePoints = 50, 
                                DueDate = new DateTime(2026, 4, 15) // Future date
                            },
                            new Assignment { 
                                Id = 3, 
                                Name = "Final Project: Graph Database", 
                                Description = "Build a social network graph.", 
                                AvailablePoints = 200, 
                                DueDate = new DateTime(2026, 5, 1) // Further future
                            }
                        },
                        // 2. Pre-populated Modules with Content
                        Modules = new List<Module>
                        {
                            new Module { 
                                Id = 1, 
                                ModuleName = "Unit 1: Review of Linked Lists", 
                                Content = new List<string> { 
                                    "Singly Linked Lists Video", 
                                    "Doubly Linked Lists PDF", 
                                    "Big O Notation Cheat Sheet" 
                                }
                            },
                            new Module { 
                                Id = 2, 
                                ModuleName = "Unit 2: Trees and Graphs", 
                                Content = new List<string> { 
                                    "AVL Tree Visualization", 
                                    "Dijkstra's Algorithm Overview" 
                                }
                            }
                        },
                        // 3. Pre-populated Students (Assuming you have a Student model)
                        Roster = new List<Student>
                        {
                            StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 1),
                            StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 2),
                            StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 3),
                            StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 4),
                            StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 5),
                            StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 6),
                            StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 7),
                        }
                    }
                };
            }

        public void AddOrUpdate(Course? course)
        {
            if (course == null){
                return;
            }

            if (course.Id == 0){
                course.Id = NextKey;
                Courses.Add(course);
            }
        }

        public Course? Delete(Course? course)
        {
            if(course == null){
                return null;
            }

            Courses.Remove(course);

            return course;
        }

        public int NextKey
        {
            get
            {
                if(Courses.Any()){
                    return Courses.Select(i => i.Id).Max() + 1;
                }
                return 1;
            }
        }
        public void AddModule(int CourseID, string moduleName)
        {
            if(moduleName == null || CourseID == null)
            {
                return;
            }
            else
            {
                int NextModuleKey;
                var course = Courses.FirstOrDefault(c => c.Id == CourseID);
                if(course.Modules == null)
                {
                    course.Modules = new List<Canvas.Library.Model.Module>();
                }
                if(course.Modules.Any())
                    NextModuleKey = course.Modules.Select(i => i.Id).Max() + 1;
                else
                    NextModuleKey = 1;
                var module = new Canvas.Library.Model.Module
                {
                    ModuleName = moduleName,
                    Id = NextModuleKey
                };
                Courses.FirstOrDefault(c => c.Id == CourseID)?.Modules.Add(module);
            }
        }

        public bool AddModuleContent(int CourseID, int ModuleID, string newContent)
        {
            var module = Courses.FirstOrDefault(c => c.Id == CourseID)
                        ?.Modules?.FirstOrDefault(m => m.Id == ModuleID);

            if (module == null) return false;

            module.Content ??= new List<string>(); // Initialize if null
            module.Content.Add(newContent);
            return true;
        }

        public void UpdateModuleContent(int CourseId, int ModuleId, int ContentIndex, string newContent)
        {
            //there is also this error checking in the CLI because I had to search the Lists, but can't have too much (for now at least)
            var course = CourseServiceProxy.Current.Courses
                .FirstOrDefault(c => c.Id == CourseId);
            var module = course?.Modules?
                .FirstOrDefault(m => m.Id == ModuleId);
            if (module != null && module.Content != null)
            {
                if (ContentIndex >= 0 && ContentIndex < module.Content.Count)
                {
                    module.Content[ContentIndex] = newContent;
                }
            }
        }

        public void DeleteModuleContent(int CourseId, int ModuleId, int ContentIndex)
        {
            var course = Courses?.FirstOrDefault(c => c.Id == CourseId);
            if (course == null) return;
            var module = course.Modules?.FirstOrDefault(m => m.Id == ModuleId);
            if (module == null || module.Content == null) return;
            if (ContentIndex >= 0 && ContentIndex < module.Content.Count)
            {
                module.Content.RemoveAt(ContentIndex);
            }
        }

        public void UnenrollStudent(int CourseID, int StudnetID)
        {
            var course = Courses.FirstOrDefault(i => i.Id == CourseID);
            var studentToRemove = course.Roster.FirstOrDefault(i => i.Id == StudnetID);

            if (studentToRemove != null) 
                course.Roster.Remove(studentToRemove);
        }

        public bool UnenrollStudentFromAllCourses(int studentId)
        {
            DeleteAllStudentsSubmissions(studentId);
            bool found = false;
            foreach (var course in Courses)
            {
                var studentToRemove = course.Roster.FirstOrDefault(i => i.Id == studentId);
                if (studentToRemove != null)
                {
                    course.Roster.Remove(studentToRemove);
                    found = true;
                }
            }
            return found;
        }

        public void DeleteAllStudentsSubmissions(int studentID)
        {
            Courses.ForEach(m =>
            {
                m.Assignments.ForEach (n => {
                    var index = n.Submissions.FindIndex(i => i.StudentId == studentID);
                    if(index >= 0)
                    {
                        n.Submissions.RemoveAt(index);
                    }
                });
            });
        }
    }
}
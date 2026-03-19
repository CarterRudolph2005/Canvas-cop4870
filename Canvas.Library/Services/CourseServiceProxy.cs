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
                new Course{Id = 1, Code = "COP4530", Name = "Data structures II", Description = "This course is about Data Structures."}
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
    }
}
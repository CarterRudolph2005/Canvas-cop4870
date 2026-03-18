using CLI.Canvas.Model;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Net;
using System.Net.ServerSentEvents;
using System.Reflection;
using System.Text;

namespace CLI.Canvas.Services
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
                    course.Modules = new List<CLI.Canvas.Model.Module>();
                }
                if(course.Modules.Any())
                    NextModuleKey = course.Modules.Select(i => i.Id).Max() + 1;
                else
                    NextModuleKey = 1;
                var module = new CLI.Canvas.Model.Module
                {
                    ModuleName = moduleName,
                    Id = NextModuleKey
                };
                Courses.FirstOrDefault(c => c.Id == CourseID)?.Modules.Add(module);
            }
        }

        public bool AddModuleContent(int CourseID, int ModuleID, string newContent)
        {
            try
            {
                var course = Courses.FirstOrDefault(i => i.Id == CourseID);
                var module = course.Modules.FirstOrDefault(i => i.Id == ModuleID);
                if (module.Content == null)
                {
                    module.Content = new List<string>();
                }
                module.Content.Add(newContent);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
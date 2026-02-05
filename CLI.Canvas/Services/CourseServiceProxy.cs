using CLI.Canvas.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Net;
using System.Net.ServerSentEvents;
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
                new Course{Id = 1, Code = "24c", Name = "Data structures"}
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
    }
}
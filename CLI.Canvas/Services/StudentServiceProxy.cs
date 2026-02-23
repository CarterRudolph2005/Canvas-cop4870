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
    public class StudentServiceProxy
    {
        public Student CurrentStudent{get; set;}
        private List<Student> students;

        public List<Student> Students
        {
            get
            {
                return students;
            }

            set
            {
                if(students != value)
                {
                    students = value;
                }
            }
        }
        private static StudentServiceProxy? instance;
        private static object instanceLock = new object();

        public static StudentServiceProxy Current
        {
            get
            {
                lock (instanceLock)
                {
                    if(instance == null)
                    {
                        instance = new StudentServiceProxy();
                    }
                }
                return instance;
            }
        }
        private StudentServiceProxy()
        {
            students = new List<Student>
            {
                new Student{Id = 1, Code = "24c", Name = "Carter Rudolph", Classification = Classification.Sophomore}
            };
        }

        public void ProxyAs(int id) {
            CurrentStudent = Students.FirstOrDefault(s => s.Id == id);
        }

        public int NextKey
        {
            get
            {
                if(Students.Any()){
                    return Students.Select(i => i.Id).Max() + 1;
                }
                return 1;
            }
        }
    }
}
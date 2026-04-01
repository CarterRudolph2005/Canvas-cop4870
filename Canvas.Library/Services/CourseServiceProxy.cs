using System.Dynamic;
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
using System.Text;

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

        public void AddOrUpdate(Course? course)
        {
            if (course == null) return;
            if (course.Id == 0)
            {
                course.Id = NextKey;
                Courses.Add(course);
            }
            else 
            {
                var existing = Courses.FirstOrDefault(c => c.Id == course.Id);
                if (existing != null)
                {
                    existing.Name = course.Name;
                    existing.Code = course.Code;
                    existing.Description = course.Description;
                }
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

        public List<Course> GetCoursesForStudent(int studentID)
        {
            return Courses
                .Where(c => c.Roster != null && c.Roster.Any(m => m != null && m.Id == studentID))
                .ToList();
        }

        public void EnrollStudent(int studentId, int courseId)
        {
            var student = StudentServiceProxy.Current.GetById(studentId);
            var course = Courses.FirstOrDefault(i => i.Id == courseId);
            if (student == null || course == null) return;
            if (course.Roster?.Any(s => s.Id == studentId) == true) return;
            course.Roster?.Add(student);
        }


        public bool AddOrUpdateAssignment(int courseId, Assignment assignment)
        {
            var course = Courses.FirstOrDefault(i => i.Id == courseId);
            if (course == null) return false;

            course.Assignments ??= new List<Assignment>();

            if (assignment.Id == 0)
            {
                assignment.Id = AssignmentNextKey(course);
                course.Assignments.Add(assignment);
            }
            else
            {
                var existing = course.Assignments.FirstOrDefault(a => a.Id == assignment.Id);
                if (existing == null) return false;
                existing.Name = assignment.Name;
                existing.Description = assignment.Description;
                existing.AvailablePoints = assignment.AvailablePoints;
                existing.DueDate = assignment.DueDate;
            }
            return true;
        }
        private int AssignmentNextKey(Course course) 
        {
            if (course.Assignments != null && course.Assignments.Any()) {
                return course.Assignments.Max(a => a.Id) + 1;
            }
            return 1;
        }

        public bool DeleteAssignment(int courseId, int assignmentId)
        {
            var course = Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null || course.Assignments == null) return false;

            var assignmentToRemove = course.Assignments.FirstOrDefault(a => a.Id == assignmentId);
            if (assignmentToRemove == null) return false;

            // submissions are owned by the assignment so they die with it
            assignmentToRemove.Submissions?.Clear();
            course.Assignments.Remove(assignmentToRemove);
            return true;
        }
        public List<Submission> GetStudentSubmissions(int courseId, int studentId)
        {
            return Courses.FirstOrDefault(c => c.Id == courseId)
                ?.Assignments
                ?.SelectMany(a => a.Submissions)
                .Where(s => s.StudentId == studentId)
                .ToList() ?? new List<Submission>();
        }

        public double CalculateGrade(int courseId, int studentId)
        {
            var course = Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return 0;
            if (course.Assignments == null || !course.Assignments.Any()) return 0;

            var submittedAssignments = course.Assignments
                .Where(a => a.Submissions != null && 
                            a.Submissions.Any(s => s.StudentId == studentId))
                .ToList();

            if (!submittedAssignments.Any()) return 0;

            var totalAvailable = submittedAssignments.Sum(a => a.AvailablePoints);
            
            var totalEarned = submittedAssignments
                .SelectMany(a => a.Submissions)
                .Where(s => s.StudentId == studentId)
                .Sum(s => s.PointsAwarded ?? 0);

            return totalAvailable > 0 ? (double)totalEarned / totalAvailable * 100 : 0;
        }

        public void SubmitAssignment(int CourseID, Submission submission)
        {
            var course = Courses.FirstOrDefault(i => i.Id == CourseID);
            var assignment = course.Assignments.FirstOrDefault(i => i.Id == submission.AssignmentId);
            if(assignment.Submissions == null)  {assignment.Submissions = new List<Submission>();}
            int nextId = assignment.Submissions.Any() 
                ? assignment.Submissions.Max(s => s.Id) + 1 
                : 1;

            submission.Id = nextId;
            assignment.Submissions.Add(submission);
        }

        public void GradeSubmission(int CourseID, int AssignmentID, int SubmissionID, int Points)
        {
            var submission = Courses?.FirstOrDefault(i => i.Id == CourseID)
                .Assignments?.FirstOrDefault(i => i.Id == AssignmentID)
                .Submissions?.FirstOrDefault(i => i.Id == SubmissionID);
            if (submission != null)
                submission.PointsAwarded = Points;
        }
        public void AddAnnouncement(int courseId, Announcement announcement)
        {
            var course = Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return;

            course.Announcements ??= new List<Announcement>();
            announcement.Id = course.Announcements.Any()
                ? course.Announcements.Max(a => a.Id) + 1
                : 1;
            course.Announcements.Add(announcement);
        }

        public void DeleteAnnouncement(int courseId, int announcementId)
        {
            var course = Courses.FirstOrDefault(c => c.Id == courseId);
            var announcement = course?.Announcements?.FirstOrDefault(a => a.Id == announcementId);
            if (announcement != null)
                course.Announcements.Remove(announcement);
        }

        public void UpdateAnnouncement(int courseId, Announcement updated)
        {
            var course = Courses.FirstOrDefault(c => c.Id == courseId);
            var announcement = course?.Announcements?.FirstOrDefault(a => a.Id == updated.Id);
            if (announcement == null) return;
            announcement.Title = updated.Title;
            announcement.Body = updated.Body;
        }

        public void DeleteModule(int courseId, int moduleId)
        {
            var course = Courses.FirstOrDefault(c => c.Id == courseId);
            var module = course?.Modules?.FirstOrDefault(m => m.Id == moduleId);
            if (module != null)
                course.Modules.Remove(module);
        }

        public void CopyAssignmentToCourse(int assignmentId, int sourceCourseId, int targetCourseId)
        {
            var source = Courses.FirstOrDefault(c => c.Id == sourceCourseId);
            if (source == null) return;

            var assignment = source.Assignments?.FirstOrDefault(a => a.Id == assignmentId);
            if (assignment == null) return;

            var copy = new Assignment
            {
                Id = 0, // signals AddOrUpdate to treat it as new
                Name = assignment.Name,
                Description = assignment.Description,
                AvailablePoints = assignment.AvailablePoints,
                DueDate = assignment.DueDate,
                Submissions = new List<Submission>()
            };

            AddOrUpdateAssignment(targetCourseId, copy);
        }
        //**************** Import and Export Fuctions *********************** (REVIEW)
        public string ExportRoster(int courseId)
        {
            var course = Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine("StudentCode");

            foreach (var student in course.Roster ?? new List<Student>())
                sb.AppendLine(student.Code);

            return sb.ToString();
        }

        public (int added, int skipped, int notFound) ImportRoster(int courseId, string csvContent)
        {
            var course = Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return (0, 0, 0);

            if (course.Roster == null)
                course.Roster = new List<Student>();

            int added = 0, skipped = 0, notFound = 0;

            var lines = csvContent
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Skip(1) // skip header
                .ToList();

            foreach (var code in lines)
            {
                // find student in system by code
                var student = StudentServiceProxy.Current.Students.FirstOrDefault(s => s.Code == code);

                if (student == null)
                {
                    notFound++;
                    continue;
                }

                // idempotent — skip if already enrolled
                if (course.Roster.Any(s => s.Code == code))
                {
                    skipped++;
                    continue;
                }

                course.Roster.Add(student);
                added++;
            }

            return (added, skipped, notFound);
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
                    SectionNumber = 1,
                    Instructors = new List<Instructor>
                    {
                        InstructorServiceProxy.Current.Instructors.FirstOrDefault(i => i.Id == 1)
                    },
                    Assignments = new List<Assignment>
                    {
                        new Assignment { Id = 1,  Name = "Binary Search Tree Lab",       Description = "Implement a thread-safe BST.",                    AvailablePoints = 100, DueDate = new DateTime(2026, 3, 10) },
                        new Assignment { Id = 2,  Name = "B-Tree Research Paper",         Description = "Analyze disk-based data structures.",             AvailablePoints = 50,  DueDate = new DateTime(2026, 4, 15) },
                        new Assignment { Id = 3,  Name = "Final Project: Graph Database", Description = "Build a social network graph.",                   AvailablePoints = 200, DueDate = new DateTime(2026, 5, 1)  },
                        new Assignment { Id = 4,  Name = "Hash Table Implementation",     Description = "Implement open and closed hashing.",              AvailablePoints = 75,  DueDate = new DateTime(2026, 3, 25) },
                        new Assignment { Id = 5,  Name = "Heap Sort Analysis",            Description = "Compare heap sort vs merge sort performance.",    AvailablePoints = 60,  DueDate = new DateTime(2026, 4, 5)  },
                    },
                    Modules = new List<Module>
                    {
                        new Module { Id = 1,  ModuleName = "Unit 1: Review of Linked Lists",      Content = new List<string> { "Singly Linked Lists Video", "Doubly Linked Lists PDF", "Big O Notation Cheat Sheet" } },
                        new Module { Id = 2,  ModuleName = "Unit 2: Trees and Graphs",            Content = new List<string> { "AVL Tree Visualization", "Dijkstra's Algorithm Overview", "Tree Traversal PDF" } },
                        new Module { Id = 3,  ModuleName = "Unit 3: Hash Tables",                 Content = new List<string> { "Hash Functions Video", "Collision Resolution PDF", "Practice Problems" } },
                        new Module { Id = 4,  ModuleName = "Unit 4: Heaps and Priority Queues",   Content = new List<string> { "Min/Max Heap Video", "Priority Queue Use Cases", "Heap Implementation Guide" } },
                        new Module { Id = 5,  ModuleName = "Unit 5: Sorting Algorithms",          Content = new List<string> { "Merge Sort Video", "Quick Sort PDF", "Sorting Comparison Chart" } },
                        new Module { Id = 6,  ModuleName = "Unit 6: Graph Algorithms",            Content = new List<string> { "BFS Video", "DFS PDF", "Shortest Path Problems" } },
                        new Module { Id = 7,  ModuleName = "Unit 7: Dynamic Programming",         Content = new List<string> { "Memoization Video", "Tabulation PDF", "Classic DP Problems" } },
                        new Module { Id = 8,  ModuleName = "Unit 8: Tries",                       Content = new List<string> { "Trie Structure Video", "Autocomplete Implementation", "Trie vs Hash Table" } },
                        new Module { Id = 9,  ModuleName = "Unit 9: Disjoint Sets",               Content = new List<string> { "Union-Find Video", "Path Compression PDF", "Kruskal's Algorithm" } },
                        new Module { Id = 10, ModuleName = "Unit 10: Algorithm Design Review",    Content = new List<string> { "Final Exam Study Guide", "Past Exam Problems", "Review Slides" } },
                    },
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
                },

                new Course
                {
                    Id = 2,
                    Code = "COP3330",
                    Name = "Object Oriented Programming",
                    Description = "Principles of OOP including inheritance, polymorphism, and design patterns.",
                    SectionNumber = 1,
                    Instructors = new List<Instructor>
                    {
                        InstructorServiceProxy.Current.Instructors.FirstOrDefault(i => i.Id == 2)
                    },
                    Assignments = new List<Assignment>
                    {
                        new Assignment { Id = 6,  Name = "Class Hierarchy Design",        Description = "Design an inheritance hierarchy for a zoo.",      AvailablePoints = 80,  DueDate = new DateTime(2026, 3, 12) },
                        new Assignment { Id = 7,  Name = "Design Patterns Report",        Description = "Document three Gang of Four design patterns.",    AvailablePoints = 60,  DueDate = new DateTime(2026, 3, 28) },
                        new Assignment { Id = 8,  Name = "Polymorphism Lab",              Description = "Implement runtime polymorphism in C#.",           AvailablePoints = 90,  DueDate = new DateTime(2026, 4, 10) },
                        new Assignment { Id = 9,  Name = "Interface Segregation Quiz",    Description = "Quiz on SOLID principles.",                       AvailablePoints = 40,  DueDate = new DateTime(2026, 4, 22) },
                        new Assignment { Id = 10, Name = "Final OOP Project",             Description = "Build a fully object-oriented application.",      AvailablePoints = 200, DueDate = new DateTime(2026, 5, 3)  },
                        new Assignment { Id = 11, Name = "Abstract Classes Lab",          Description = "Implement abstract classes and interfaces.",      AvailablePoints = 70,  DueDate = new DateTime(2026, 3, 20) },
                    },
                    Modules = new List<Module>
                    {
                        new Module { Id = 11, ModuleName = "Unit 1: OOP Fundamentals",           Content = new List<string> { "Classes and Objects Video", "Encapsulation PDF", "OOP Quiz" } },
                        new Module { Id = 12, ModuleName = "Unit 2: Inheritance",                Content = new List<string> { "Inheritance Video", "Base and Derived Classes PDF", "Practice Exercises" } },
                        new Module { Id = 13, ModuleName = "Unit 3: Polymorphism",               Content = new List<string> { "Runtime Polymorphism Video", "Method Overriding PDF", "Polymorphism Examples" } },
                        new Module { Id = 14, ModuleName = "Unit 4: Abstract Classes",           Content = new List<string> { "Abstract vs Interface Video", "When to Use Each PDF", "Lab Instructions" } },
                        new Module { Id = 15, ModuleName = "Unit 5: Interfaces",                 Content = new List<string> { "Interface Design Video", "Multiple Interface Implementation", "Practice Problems" } },
                        new Module { Id = 16, ModuleName = "Unit 6: SOLID Principles",           Content = new List<string> { "SOLID Overview Video", "Single Responsibility PDF", "Open/Closed Principle Examples" } },
                        new Module { Id = 17, ModuleName = "Unit 7: Design Patterns Intro",      Content = new List<string> { "Creational Patterns Video", "Structural Patterns PDF", "Pattern Catalog" } },
                        new Module { Id = 18, ModuleName = "Unit 8: Singleton and Factory",      Content = new List<string> { "Singleton Pattern Video", "Factory Method PDF", "Implementation Lab" } },
                        new Module { Id = 19, ModuleName = "Unit 9: Observer Pattern",           Content = new List<string> { "Observer Overview Video", "Event-Driven Design PDF", "Observer Lab" } },
                        new Module { Id = 20, ModuleName = "Unit 10: Final Review",              Content = new List<string> { "OOP Review Slides", "Final Project Guidelines", "Past Exam Questions" } },
                    },
                    Roster = new List<Student>
                    {
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 1),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 3),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 5),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 6),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 7),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 8),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 9),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 10),
                    }
                },

                new Course
                {
                    Id = 3,
                    Code = "CDA3101",
                    Name = "Computer Organization",
                    Description = "Study of computer hardware organization, assembly language, and memory systems.",
                    SectionNumber = 1,
                    Instructors = new List<Instructor>
                    {
                        InstructorServiceProxy.Current.Instructors.FirstOrDefault(i => i.Id == 3)
                    },
                    Assignments = new List<Assignment>
                    {
                        new Assignment { Id = 12, Name = "MIPS Assembly Lab",             Description = "Write basic MIPS assembly programs.",            AvailablePoints = 100, DueDate = new DateTime(2026, 3, 15) },
                        new Assignment { Id = 13, Name = "Cache Memory Analysis",         Description = "Analyze cache hit/miss rates.",                   AvailablePoints = 75,  DueDate = new DateTime(2026, 3, 30) },
                        new Assignment { Id = 14, Name = "Pipeline Hazards Report",       Description = "Identify and resolve pipeline hazards.",          AvailablePoints = 60,  DueDate = new DateTime(2026, 4, 12) },
                        new Assignment { Id = 15, Name = "ALU Design Project",            Description = "Design a simple ALU in logic gates.",             AvailablePoints = 150, DueDate = new DateTime(2026, 4, 28) },
                        new Assignment { Id = 16, Name = "Memory Hierarchy Quiz",         Description = "Quiz covering RAM, cache, and virtual memory.",   AvailablePoints = 40,  DueDate = new DateTime(2026, 3, 22) },
                        new Assignment { Id = 17, Name = "Instruction Set Architecture",  Description = "Compare RISC vs CISC architectures.",             AvailablePoints = 55,  DueDate = new DateTime(2026, 4, 8)  },
                        new Assignment { Id = 18, Name = "Final Exam Review Project",     Description = "Comprehensive hardware design project.",          AvailablePoints = 200, DueDate = new DateTime(2026, 5, 5)  },
                    },
                    Modules = new List<Module>
                    {
                        new Module { Id = 21, ModuleName = "Unit 1: Binary and Data Representation",  Content = new List<string> { "Binary Numbers Video", "Two's Complement PDF", "Practice Problems" } },
                        new Module { Id = 22, ModuleName = "Unit 2: Logic Gates",                     Content = new List<string> { "AND/OR/NOT Video", "Circuit Diagrams PDF", "Gate Lab" } },
                        new Module { Id = 23, ModuleName = "Unit 3: MIPS Assembly",                   Content = new List<string> { "MIPS Instruction Set PDF", "Assembly Video Tutorial", "MIPS Simulator Guide" } },
                        new Module { Id = 24, ModuleName = "Unit 4: ALU Design",                      Content = new List<string> { "ALU Overview Video", "Adder Circuits PDF", "Design Lab Instructions" } },
                        new Module { Id = 25, ModuleName = "Unit 5: CPU Datapath",                    Content = new List<string> { "Datapath Video", "Control Unit PDF", "Single Cycle CPU Diagram" } },
                        new Module { Id = 26, ModuleName = "Unit 6: Pipelining",                      Content = new List<string> { "Pipeline Stages Video", "Hazard Detection PDF", "Pipeline Simulation" } },
                        new Module { Id = 27, ModuleName = "Unit 7: Cache Memory",                    Content = new List<string> { "Cache Basics Video", "Direct Mapped Cache PDF", "Cache Lab" } },
                        new Module { Id = 28, ModuleName = "Unit 8: Virtual Memory",                  Content = new List<string> { "Paging Video", "Page Tables PDF", "TLB Overview" } },
                        new Module { Id = 29, ModuleName = "Unit 9: I/O Systems",                     Content = new List<string> { "I/O Devices Video", "Interrupts PDF", "DMA Overview" } },
                        new Module { Id = 30, ModuleName = "Unit 10: Final Review",                   Content = new List<string> { "Exam Study Guide", "Past Exams", "Review Lecture Slides" } },
                    },
                    Roster = new List<Student>
                    {
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 2),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 3),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 4),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 5),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 7),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 8),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 9),
                    }
                },

                new Course
                {
                    Id = 4,
                    Code = "COP4020",
                    Name = "Programming Languages",
                    Description = "Survey of programming language concepts, paradigms, and implementation.",
                    SectionNumber = 1,
                    Instructors = new List<Instructor>
                    {
                        InstructorServiceProxy.Current.Instructors.FirstOrDefault(i => i.Id == 1)
                    },
                    Assignments = new List<Assignment>
                    {
                        new Assignment { Id = 19, Name = "Functional Programming Lab",    Description = "Write programs in Haskell.",                      AvailablePoints = 80,  DueDate = new DateTime(2026, 3, 18) },
                        new Assignment { Id = 20, Name = "Language Grammar Report",       Description = "Define a BNF grammar for a simple language.",     AvailablePoints = 60,  DueDate = new DateTime(2026, 4, 1)  },
                        new Assignment { Id = 21, Name = "Type Systems Quiz",             Description = "Quiz on static vs dynamic typing.",               AvailablePoints = 40,  DueDate = new DateTime(2026, 4, 14) },
                        new Assignment { Id = 22, Name = "Interpreter Project",           Description = "Build a simple expression interpreter.",          AvailablePoints = 175, DueDate = new DateTime(2026, 4, 30) },
                        new Assignment { Id = 23, Name = "Prolog Logic Lab",              Description = "Solve problems using Prolog.",                    AvailablePoints = 70,  DueDate = new DateTime(2026, 3, 26) },
                    },
                    Modules = new List<Module>
                    {
                        new Module { Id = 31, ModuleName = "Unit 1: Language Paradigms",         Content = new List<string> { "Paradigms Overview Video", "Imperative vs Declarative PDF", "Paradigm Quiz" } },
                        new Module { Id = 32, ModuleName = "Unit 2: Syntax and Grammars",        Content = new List<string> { "BNF Grammar Video", "Parse Trees PDF", "Grammar Exercises" } },
                        new Module { Id = 33, ModuleName = "Unit 3: Lexical Analysis",           Content = new List<string> { "Tokenization Video", "Regular Expressions PDF", "Lexer Lab" } },
                        new Module { Id = 34, ModuleName = "Unit 4: Parsing",                    Content = new List<string> { "Top-Down Parsing Video", "LL vs LR Parsers PDF", "Parser Lab" } },
                        new Module { Id = 35, ModuleName = "Unit 5: Type Systems",               Content = new List<string> { "Static vs Dynamic Video", "Type Inference PDF", "Type Checking Examples" } },
                        new Module { Id = 36, ModuleName = "Unit 6: Functional Programming",     Content = new List<string> { "Lambda Calculus Video", "Haskell Intro PDF", "Functional Lab" } },
                        new Module { Id = 37, ModuleName = "Unit 7: Logic Programming",          Content = new List<string> { "Prolog Basics Video", "Unification PDF", "Prolog Lab" } },
                        new Module { Id = 38, ModuleName = "Unit 8: Memory Management",          Content = new List<string> { "Garbage Collection Video", "Manual vs Automatic PDF", "Memory Lab" } },
                        new Module { Id = 39, ModuleName = "Unit 9: Concurrency",                Content = new List<string> { "Threads Video", "Race Conditions PDF", "Concurrency Examples" } },
                        new Module { Id = 40, ModuleName = "Unit 10: Final Review",              Content = new List<string> { "Language Concepts Summary", "Final Project Guide", "Past Exam Questions" } },
                    },
                    Roster = new List<Student>
                    {
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 1),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 2),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 4),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 6),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 8),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 9),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 10),
                    }
                },

                new Course
                {
                    Id = 5,
                    Code = "CNT4007",
                    Name = "Computer Networks",
                    Description = "Fundamentals of computer networking including protocols, routing, and security.",
                    SectionNumber = 1,
                    Instructors = new List<Instructor>
                    {
                        InstructorServiceProxy.Current.Instructors.FirstOrDefault(i => i.Id == 2)
                    },
                    Assignments = new List<Assignment>
                    {
                        new Assignment { Id = 24, Name = "OSI Model Report",              Description = "Describe each layer of the OSI model.",           AvailablePoints = 50,  DueDate = new DateTime(2026, 3, 14) },
                        new Assignment { Id = 25, Name = "Socket Programming Lab",        Description = "Build a TCP client-server application.",          AvailablePoints = 100, DueDate = new DateTime(2026, 3, 29) },
                        new Assignment { Id = 26, Name = "Routing Algorithm Analysis",    Description = "Compare Dijkstra and Bellman-Ford routing.",      AvailablePoints = 75,  DueDate = new DateTime(2026, 4, 11) },
                        new Assignment { Id = 27, Name = "Wireshark Lab",                 Description = "Capture and analyze network packets.",            AvailablePoints = 60,  DueDate = new DateTime(2026, 4, 20) },
                        new Assignment { Id = 28, Name = "Network Security Quiz",         Description = "Quiz on encryption and firewall concepts.",       AvailablePoints = 40,  DueDate = new DateTime(2026, 4, 27) },
                        new Assignment { Id = 29, Name = "HTTP Protocol Deep Dive",       Description = "Analyze HTTP request/response cycles.",           AvailablePoints = 55,  DueDate = new DateTime(2026, 3, 21) },
                        new Assignment { Id = 30, Name = "Final Network Design Project",  Description = "Design a scalable enterprise network.",           AvailablePoints = 200, DueDate = new DateTime(2026, 5, 6)  },
                    },
                    Modules = new List<Module>
                    {
                        new Module { Id = 41, ModuleName = "Unit 1: Network Fundamentals",       Content = new List<string> { "Network Types Video", "LAN vs WAN PDF", "Networking Quiz" } },
                        new Module { Id = 42, ModuleName = "Unit 2: OSI Model",                  Content = new List<string> { "OSI Layers Video", "Layer Functions PDF", "OSI Examples" } },
                        new Module { Id = 43, ModuleName = "Unit 3: TCP/IP",                     Content = new List<string> { "TCP vs UDP Video", "IP Addressing PDF", "TCP Lab" } },
                        new Module { Id = 44, ModuleName = "Unit 4: Application Layer",          Content = new List<string> { "HTTP Overview Video", "DNS PDF", "Application Layer Lab" } },
                        new Module { Id = 45, ModuleName = "Unit 5: Transport Layer",            Content = new List<string> { "Flow Control Video", "Congestion Control PDF", "Transport Lab" } },
                        new Module { Id = 46, ModuleName = "Unit 6: Network Layer",              Content = new List<string> { "IP Routing Video", "Subnetting PDF", "Routing Lab" } },
                        new Module { Id = 47, ModuleName = "Unit 7: Data Link Layer",            Content = new List<string> { "MAC Addresses Video", "Ethernet PDF", "Switch Lab" } },
                        new Module { Id = 48, ModuleName = "Unit 8: Wireless Networks",          Content = new List<string> { "WiFi Standards Video", "802.11 PDF", "Wireless Security" } },
                        new Module { Id = 49, ModuleName = "Unit 9: Network Security",           Content = new List<string> { "Encryption Video", "Firewalls PDF", "VPN Overview" } },
                        new Module { Id = 50, ModuleName = "Unit 10: Final Review",              Content = new List<string> { "Network Concepts Summary", "Final Project Guidelines", "Practice Exams" } },
                    },
                    Roster = new List<Student>
                    {
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 1),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 3),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 4),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 5),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 6),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 8),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 9),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 10),
                    }

                },
                new Course
                {
                    Id = 6,
                    Code = "CNT4007",
                    Name = "Computer Networks",
                    Description = "Fundamentals of computer networking including protocols, routing, and security.",
                    SectionNumber = 2,
                    Instructors = new List<Instructor>
                    {
                        InstructorServiceProxy.Current.Instructors.FirstOrDefault(i => i.Id == 2)
                    },
                    Assignments = new List<Assignment>
                    {
                        new Assignment { Id = 24, Name = "OSI Model Report",              Description = "Describe each layer of the OSI model.",           AvailablePoints = 50,  DueDate = new DateTime(2026, 3, 14) },
                        new Assignment { Id = 25, Name = "Socket Programming Lab",        Description = "Build a TCP client-server application.",          AvailablePoints = 100, DueDate = new DateTime(2026, 3, 29) },
                        new Assignment { Id = 26, Name = "Routing Algorithm Analysis",    Description = "Compare Dijkstra and Bellman-Ford routing.",      AvailablePoints = 75,  DueDate = new DateTime(2026, 4, 11) },
                        new Assignment { Id = 27, Name = "Wireshark Lab",                 Description = "Capture and analyze network packets.",            AvailablePoints = 60,  DueDate = new DateTime(2026, 4, 20) },
                        new Assignment { Id = 28, Name = "Network Security Quiz",         Description = "Quiz on encryption and firewall concepts.",       AvailablePoints = 40,  DueDate = new DateTime(2026, 4, 27) },
                        new Assignment { Id = 29, Name = "HTTP Protocol Deep Dive",       Description = "Analyze HTTP request/response cycles.",           AvailablePoints = 55,  DueDate = new DateTime(2026, 3, 21) },
                        new Assignment { Id = 30, Name = "Final Network Design Project",  Description = "Design a scalable enterprise network.",           AvailablePoints = 200, DueDate = new DateTime(2026, 5, 6)  },
                    },
                    Modules = new List<Module>
                    {
                        new Module { Id = 41, ModuleName = "Unit 1: Network Fundamentals",       Content = new List<string> { "Network Types Video", "LAN vs WAN PDF", "Networking Quiz" } },
                        new Module { Id = 42, ModuleName = "Unit 2: OSI Model",                  Content = new List<string> { "OSI Layers Video", "Layer Functions PDF", "OSI Examples" } },
                        new Module { Id = 43, ModuleName = "Unit 3: TCP/IP",                     Content = new List<string> { "TCP vs UDP Video", "IP Addressing PDF", "TCP Lab" } },
                        new Module { Id = 44, ModuleName = "Unit 4: Application Layer",          Content = new List<string> { "HTTP Overview Video", "DNS PDF", "Application Layer Lab" } },
                        new Module { Id = 45, ModuleName = "Unit 5: Transport Layer",            Content = new List<string> { "Flow Control Video", "Congestion Control PDF", "Transport Lab" } },
                        new Module { Id = 46, ModuleName = "Unit 6: Network Layer",              Content = new List<string> { "IP Routing Video", "Subnetting PDF", "Routing Lab" } },
                        new Module { Id = 47, ModuleName = "Unit 7: Data Link Layer",            Content = new List<string> { "MAC Addresses Video", "Ethernet PDF", "Switch Lab" } },
                        new Module { Id = 48, ModuleName = "Unit 8: Wireless Networks",          Content = new List<string> { "WiFi Standards Video", "802.11 PDF", "Wireless Security" } },
                        new Module { Id = 49, ModuleName = "Unit 9: Network Security",           Content = new List<string> { "Encryption Video", "Firewalls PDF", "VPN Overview" } },
                        new Module { Id = 50, ModuleName = "Unit 10: Final Review",              Content = new List<string> { "Network Concepts Summary", "Final Project Guidelines", "Practice Exams" } },
                    },
                    Roster = new List<Student>
                    {
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 1),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 3),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 4),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 6),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 8),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 9),
                        StudentServiceProxy.Current.Students.FirstOrDefault(i => i.Id == 10),
                    }
                }
            };
        }
    }
}
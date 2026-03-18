using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Transactions;
using CLI.Canvas.Model;
using CLI.Canvas.Services;


namespace CLI.Canvas
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var courses = CourseServiceProxy.Current.Courses;
            var students = StudentServiceProxy.Current.Students;
            var choice = string.Empty;

            //Main Program Goes Here!
            do{
                Console.WriteLine("Choose one of the following: ");
                Console.WriteLine("1. Teacher");
                Console.WriteLine("2. Student");
                Console.WriteLine("3. Leave Application");
                
                choice = Console.ReadLine();
                if(int.TryParse(choice, out int intChoice))
                {
                    var subChoice = string.Empty;
                    
                        switch (intChoice)
                        {
                        
                            case 1:
                                do{
                                    Console.WriteLine("Teacher Menu");
                                    Console.WriteLine("A. Add a new course");
                                    Console.WriteLine("S. Show all courses");
                                    Console.WriteLine("U. Edit an existing course");
                                    Console.WriteLine("E. Enter the course's Main Menu");
                                    Console.WriteLine("D. Delete course");
                                    Console.WriteLine("Q. Quit the teacher menu");

                                    //office hours
                                    do{
                                        subChoice = Console.ReadLine();
                                    } while(string.IsNullOrWhiteSpace(subChoice));
                                
                                
                                    if(subChoice.Equals("A", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        Console.WriteLine("Course Name:");
                                        var name = Console.ReadLine();
                                        Console.WriteLine("Description");
                                        var description = Console.ReadLine();
                                        Console.WriteLine("Course Code:");
                                        var code = Console.ReadLine();

                                        var course = new Course
                                        {
                                            Name = name,
                                            Description = description,
                                            Code = code
                                        };
                                        CourseServiceProxy.Current.AddOrUpdate(course);

                                        Console.WriteLine(course);
                                    }
                                    else if(subChoice.Equals("U", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        courses.ForEach(Console.WriteLine);

                                        //choose to update login
                                        string editInput;
                                        int editCourseID;

                                        do {
                                            Console.Write("Enter your choice: ");
                                            editInput = Console.ReadLine();
                                        } while (string.IsNullOrWhiteSpace(editInput) || !int.TryParse(editInput, out editCourseID));

                                        var courseToEdit = courses.FirstOrDefault(i => i.Id == editCourseID);

                                        if(courseToEdit != null)
                                        {
                                            Console.WriteLine("New Name:");
                                            var newName = Console.ReadLine();
                                            if(!string.IsNullOrEmpty(newName)){
                                                courseToEdit.Name = newName;
                                            }
                                            Console.WriteLine("New Code:");
                                            var code = Console.ReadLine();
                                            if(!string.IsNullOrEmpty(code)){
                                                courseToEdit.Code = code;
                                            }
                                            Console.WriteLine("New Description:");
                                            var newDesc = Console.ReadLine();
                                            if(!string.IsNullOrEmpty(newDesc)){
                                                courseToEdit.Description = newDesc;
                                            }
                                        }
                                    }
                                    else if(subChoice.Equals("S", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        courses.ForEach(Console.WriteLine);
                                    }
                                    else if(subChoice.Equals("Q", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        Console.WriteLine("Happy teaching!");
                                    }else if(subChoice.Equals("D", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        courses.ForEach(Console.WriteLine);
                                        string editInput;
                                        int deleteCourseID;
                                        do {
                                            Console.WriteLine("Enter the number of the course you woipuld like to delete:");
                                            editInput = Console.ReadLine();
                                        } while (string.IsNullOrWhiteSpace(editInput) || !int.TryParse(editInput, out deleteCourseID));
                                        var courseToDelete = courses.FirstOrDefault(i => i.Id == deleteCourseID);
                                        CourseServiceProxy.Current.Delete(courseToDelete);
                                    }
                                    else if(subChoice.Equals("E", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        Console.WriteLine("Your courses:");
                                        courses.ForEach(Console.WriteLine);
                                        string editInput;
                                        int CourseID;
                                        do {
                                            Console.WriteLine("Type the number of a course to explore:");
                                            editInput = Console.ReadLine();
                                        } while (string.IsNullOrWhiteSpace(editInput) || !int.TryParse(editInput, out CourseID));
                                        var SelectedCourse = CourseServiceProxy.Current.Courses.FirstOrDefault(i => i.Id == CourseID);
                                        if(SelectedCourse == null)  {Console.WriteLine("A course with that ID could not be found.");}
                                        else{
                                            var courseMenuOption = String.Empty;
                                            do{
                                                Console.WriteLine("Welcome to " + SelectedCourse.Name + "!");
                                                Console.WriteLine("Course code: " + SelectedCourse.Code);
                                                Console.WriteLine("\nCourse Description: " + SelectedCourse.Description);
                                                Console.WriteLine("\n\nCourse Modules: ");
                                                try 
                                                { 
                                                    // SelectedCourse.Modules.ForEach(Console.WriteLine);
                                                    
                                                    SelectedCourse.Modules.ForEach(m => 
                                                        {
                                                            Console.WriteLine(m); 
                                                            if (m.Content != null && m.Content.Any())
                                                            {
                                                                foreach (var item in m.Content)
                                                                {
                                                                    Console.WriteLine($"\t- {item}"); 
                                                                }
                                                            }
                                                            else 
                                                            {
                                                                Console.WriteLine("\t(No content added yet)");
                                                            }
                                                        });
                                                }
                                                    catch
                                                        {Console.WriteLine("No modules for this course.");}
                                                Console.WriteLine("\n\nCourse Assignments: ");
                                                try { SelectedCourse.Assignments.ForEach(Console.WriteLine);}
                                                    catch
                                                        { Console.WriteLine("No assignments for this course.");}
                                                Console.WriteLine("\n\nCourse Students: ");
                                                try{ SelectedCourse.Roster.ForEach(Console.WriteLine);}
                                                    catch{ Console.WriteLine("No students are enrolled in this course.");}
                                                Console.WriteLine("Course Menu:");
                                                Console.WriteLine("M. Add Module");
                                                Console.WriteLine("A. Add Content to a Module");
                                                Console.WriteLine("Q. Quit the " + SelectedCourse.Name + " course menu.");
                                                do{
                                                    courseMenuOption = Console.ReadLine();
                                                } while(string.IsNullOrWhiteSpace(courseMenuOption));
                                                if (courseMenuOption.Equals("M", StringComparison.InvariantCultureIgnoreCase))
                                                {
                                                    var newModuleName = String.Empty;
                                                    Console.WriteLine("Please enter the name of the new module:");
                                                    do{
                                                        newModuleName = Console.ReadLine();
                                                    } while(string.IsNullOrWhiteSpace(newModuleName));
                                                    CourseServiceProxy.Current.AddModule(CourseID, newModuleName);
                                                }
                                                else if (courseMenuOption.Equals("A", StringComparison.InvariantCultureIgnoreCase)){
                                                    try{ 
                                                        SelectedCourse.Modules.ForEach(Console.WriteLine);
                                                        int ModuleID;
                                                        do {
                                                            Console.WriteLine("Enter the Id of the module you'd like to add to.");
                                                            editInput = Console.ReadLine();
                                                        } while (string.IsNullOrWhiteSpace(editInput) || !int.TryParse(editInput, out ModuleID));
                                                        var SelectedModule = SelectedCourse.Modules.FirstOrDefault(i => i.Id == ModuleID);
                                                        if(SelectedModule == null)  {Console.WriteLine("No module exists with that Id.");}
                                                        else
                                                        {
                                                            // SelectedModule.ForEach(Console.WriteLine);
                                                            Console.WriteLine("Please enter the new content:");
                                                            var ModuleNewContent = String.Empty;
                                                            do{
                                                                ModuleNewContent = Console.ReadLine();
                                                            } while(string.IsNullOrWhiteSpace(ModuleNewContent));
                                                            CourseServiceProxy.Current.AddModuleContent(CourseID, ModuleID, ModuleNewContent);
                                                        }
                                                    }
                                                    catch
                                                    {
                                                       Console.WriteLine("\t\t\t******\nThere was an issue. Please ensure there are modules to add to.\n\t\t\t******");
                                                    }
                                                }
                                                else if (courseMenuOption.Equals("Q", StringComparison.InvariantCultureIgnoreCase)){
                                                    Console.WriteLine("Bye!");
                                                }
                                            }while (!courseMenuOption.Equals("Q", StringComparison.InvariantCultureIgnoreCase));
                                        }
                                        
                                    }
                            
                                } while(!subChoice.Equals("Q", StringComparison.InvariantCultureIgnoreCase));
                                break;
                            case 2:
                                Console.WriteLine("Hello Student!");
                                students.ForEach(Console.WriteLine);
                                string studnetIDInput;
                                int proxyStudentID;
                                do {
                                    Console.Write("Select your identity: ");
                                    studnetIDInput = Console.ReadLine();
                                } while (string.IsNullOrWhiteSpace(studnetIDInput) || !int.TryParse(studnetIDInput, out proxyStudentID));

                                StudentServiceProxy.Current.ProxyAs(proxyStudentID);
                                Console.WriteLine($"Hello {StudentServiceProxy.Current.CurrentStudent.Name}!");
                                do
                                {
                                    Console.WriteLine("Student Menu: ");
                                    Console.WriteLine("Q. Quit");
                                     do{
                                        subChoice = Console.ReadLine();
                                    } while(string.IsNullOrWhiteSpace(subChoice));

                                }while(!subChoice.Equals("Q", StringComparison.InvariantCultureIgnoreCase));

                                break;
                            case 3:
                                Console.WriteLine("Bye!");

                                break;
                            default:
                                Console.WriteLine("ERROR: Invalid input! Try again.");
                                
                                break;
                        }
                    
                }
            } while (!choice.Equals("3", StringComparison.OrdinalIgnoreCase));
        }
            
    }
}

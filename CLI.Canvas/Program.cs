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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Transactions;
using Canvas.Library.Model;
using Canvas.Library.Services;
using Microsoft.VisualBasic;


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
                                                    if (!SelectedCourse.Modules.Any())  {Console.WriteLine("No modules for this course.");}  
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
                                                try { SelectedCourse.Assignments.ForEach(Console.WriteLine);
                                                      if (!SelectedCourse.Assignments.Any()) {Console.WriteLine("No assignments for this course.");}}
                                                    catch
                                                        { Console.WriteLine("No assignments for this course.");}
                                                Console.WriteLine("\n\nCourse Students: ");
                                                try{ SelectedCourse.Roster.ForEach(Console.WriteLine);}
                                                    catch{ Console.WriteLine("No students are enrolled in this course.");}
                                                Console.WriteLine("Course Menu:");
                                                Console.WriteLine("A. Add an Assignment");
                                                Console.WriteLine("U. Update an Assignment");
                                                Console.WriteLine("D. Delete an Assignment");
                                                Console.WriteLine("M. Add Module");
                                                Console.WriteLine("R. Modify Content in a Module");
                                                Console.WriteLine("C. Add Content to a Module");
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
                                                else if (courseMenuOption.Equals("C", StringComparison.InvariantCultureIgnoreCase)){
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
                                                else if (courseMenuOption.Equals("A", StringComparison.InvariantCultureIgnoreCase))
                                                {
                                                    string? inputData;
                                                    Assignment assignmentClone = new Assignment();
                                                    Console.WriteLine("Enter the title of the assignment:");
                                                    do
                                                    {
                                                        inputData = Console.ReadLine();
                                                    } while(string.IsNullOrWhiteSpace(inputData));
                                                    assignmentClone.Name = inputData;
                                                    inputData = String.Empty;
                                                    Console.WriteLine("Enter the description:");
                                                    do
                                                    {
                                                        inputData = Console.ReadLine();
                                                    } while(string.IsNullOrWhiteSpace(inputData));
                                                    assignmentClone.Description = inputData;
                                                    inputData = String.Empty;
                                                    int assignmentPoints;
                                                    bool isValid;
                                                    do
                                                    {
                                                        Console.Write("Enter Total Points for this Assignment: ");
                                                        inputData = Console.ReadLine();
                                                        isValid = int.TryParse(inputData, out assignmentPoints);
                                                        if (!isValid)
                                                        {
                                                            Console.WriteLine("Invalid entry. Please enter a whole number (e.g., 50 or 100).");
                                                        }
                                                        else if (assignmentPoints < 0)
                                                        {
                                                            Console.WriteLine("Points cannot be negative.");
                                                            isValid = false; 
                                                        }
                                                    } while (!isValid);
                                                    assignmentClone.AvailablePoints = assignmentPoints;
                                                    Console.WriteLine("Enter Due Date (MM/DD/YYYY):");
                                                    string dateInput = Console.ReadLine();
                                                    if (DateTime.TryParse(dateInput, out DateTime dueDate))
                                                    {
                                                        assignmentClone.DueDate = dueDate;
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Invalid format. Using the default date.");
                                                    }
                                                    AssignmentServiceProxy.Current.AddAssignment(SelectedCourse.Id, assignmentClone);
                                                }
                                                else if (courseMenuOption.Equals("U", StringComparison.InvariantCultureIgnoreCase))
                                                {
                                                    Console.WriteLine("Existing Assignments:");
                                                    if(SelectedCourse.Assignments == null || !SelectedCourse.Assignments.Any())
                                                    {
                                                        Console.WriteLine("You have no existing assignments");
                                                    }
                                                    else
                                                    {
                                                        SelectedCourse.Assignments.ForEach(Console.WriteLine);
                                                        string? inputData;
                                                        int AssignmentID;
                                                        do {
                                                            Console.WriteLine("Enter the ID of the assignment you'd like to edit:");
                                                            inputData = Console.ReadLine();
                                                        } while (string.IsNullOrWhiteSpace(inputData) || !int.TryParse(inputData, out AssignmentID));
                                                        var originalAssignment = SelectedCourse.Assignments.FirstOrDefault(i => i.Id == AssignmentID);
                                                        if(originalAssignment != null){
                                                            Assignment assignmentClone = new Assignment(originalAssignment);
                                                            Console.WriteLine("Enter the updated title of the assignment:");
                                                            inputData = Console.ReadLine();
                                                            if (!string.IsNullOrWhiteSpace(inputData)){}
                                                            assignmentClone.Name = inputData;
                                                            inputData = String.Empty;
                                                            Console.WriteLine("Enter the description:");
                                                                inputData = Console.ReadLine();
                                                            if (!string.IsNullOrWhiteSpace(inputData)){}
                                                            assignmentClone.Description = inputData;
                                                            inputData = String.Empty;
                                                            int assignmentPoints = assignmentClone.AvailablePoints;
                                                            bool isValid;
                                                            do
                                                            {
                                                                Console.Write("Enter Total Points for this Assignment: ");
                                                                inputData = Console.ReadLine();
                                                                isValid = int.TryParse(inputData, out assignmentPoints);
                                                                if (isValid && assignmentPoints < 0)
                                                                {
                                                                    Console.WriteLine("Points cannot be negative.");
                                                                    isValid = false;
                                                                }
                                                                else
                                                                {
                                                                    if(!isValid) {isValid = true;}
                                                                }
                                                            } while (!isValid);
                                                            assignmentClone.AvailablePoints = assignmentPoints;
                                                            Console.WriteLine("Enter Due Date (MM/DD/YYYY):");
                                                            string dateInput = Console.ReadLine();
                                                            DateTime dueDate = assignmentClone.DueDate;
                                                            if (DateTime.TryParse(dateInput, out dueDate))
                                                            {
                                                                assignmentClone.DueDate = dueDate;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Invalid format. Using the existing date.");
                                                            }
                                                            AssignmentServiceProxy.Current.UpdateAssignment(SelectedCourse.Id, AssignmentID, assignmentClone);
                                                        }
                                                    }                                                    
                                                }
                                                else if (courseMenuOption.Equals("D", StringComparison.InvariantCultureIgnoreCase))
                                                {
                                                    Console.WriteLine("Existing Assignements:");
                                                    SelectedCourse.Assignments.ForEach(Console.WriteLine);
                                                    int AssignmentID;
                                                    string? inputData;
                                                    do{
                                                        Console.WriteLine("Enter the Id of the assignment you'd like to delete:");
                                                        inputData = Console.ReadLine();
                                                    } while(!int.TryParse(inputData, out AssignmentID));
                                                    if (AssignmentServiceProxy.Current.DeleteAssignment(SelectedCourse.Id, AssignmentID))
                                                    {
                                                        Console.WriteLine("Assignment deleted!");
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("There was an issue (check assignment Id). Nothing was deleted.");
                                                    }
                                                }
                                                else if (courseMenuOption.Equals("R", StringComparison.InvariantCultureIgnoreCase))
                                                {
                                                    Console.WriteLine("Existing Modules:");
                                                    if (SelectedCourse.Modules != null && SelectedCourse.Modules.Any())
                                                    {
                                                        SelectedCourse.Modules.ForEach(Console.WriteLine);
                                                        string? inputData;
                                                        int ModuleID;
                                                        do
                                                        {
                                                            Console.WriteLine("Please enter the number for the module with the content you'd like to edit:");
                                                            inputData = Console.ReadLine();
                                                        } while (!int.TryParse(inputData, out ModuleID));

                                                        var module = SelectedCourse.Modules.FirstOrDefault(i => i.Id == ModuleID);
                                                        if (module != null)
                                                        {
                                                            if (module.Content != null && module.Content.Any())
                                                            {
                                                                Console.WriteLine("Existing Content:");
                                                                module.Content.ForEach(m =>
                                                                {
                                                                    int i = 1;
                                                                    Console.WriteLine(i + ". " + m);
                                                                    i++;
                                                                });
                                                                inputData = String.Empty;
                                                                int ContentID;
                                                                do
                                                                {
                                                                    Console.WriteLine("Please enter the number associated with the content you'd like to edit:");
                                                                    inputData = Console.ReadLine();
                                                                } while (!int.TryParse(inputData, out ContentID) && ContentID - 1 > 0 && ContentID - 1 < module.Content.Count);
                                                                ContentID -= 1;
                                                                string newContent;
                                                                inputData = String.Empty;
                                                                Console.WriteLine("Enter the updated content");
                                                                inputData = Console.ReadLine();
                                                                if(!string.IsNullOrWhiteSpace(inputData)) 
                                                                {
                                                                    newContent = inputData;
                                                                    CourseServiceProxy.Current.UpdateModuleContent(CourseID, ModuleID, ContentID, newContent);
                                                                }
                                                            }

                                                        }
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

using CLI.Canvas;
// using CLI.CLI.LMS.Model;
using System;

namespace CLI.Canvas
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Main Program Goes Here!
            Console.WriteLine("Choose one of the following: ");
            Console.WriteLine("1. Teacher");
            Console.WriteLine("2. Student");
            
            var choice = Console.ReadLine();
            if(int.TryParse(choice, out int intChoice))
            {
                switch (intChoice)
                {
                    case 1:
                        Console.WriteLine("Hello Teacher!");


                        break;
                    case 2:
                        Console.WriteLine("Hello Student!");

                        break;
                    default:
                        Console.WriteLine("ERROR: Invalid input! Try again.");
                        
                        break;
                }
            }
            
        }
    }
}
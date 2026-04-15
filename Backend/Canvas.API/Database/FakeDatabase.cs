
using Canvas.Library.Model;
using Canvas.Library.Services;

namespace Canvas.API
{
    public static class FakeDatabase
    {
        public static List<Student> Students = new List<Student>
        {
                new Student { Id = 1,  Code = "cr24", Name = "Carter Rudolph", Classification = Classification.Sophomore },
                new Student { Id = 2,  Code = "jd24", Name = "John Doe", Classification = Classification.Junior },
                new Student { Id = 3,  Code = "js25", Name = "Jane Smith", Classification = Classification.Sophomore },
                new Student { Id = 4,  Code = "ab26", Name = "Alice Brown", Classification = Classification.Freshman },
                new Student { Id = 5,  Code = "bc24", Name = "Bob Clark", Classification = Classification.Senior },
                new Student { Id = 6,  Code = "dm25", Name = "Diana Miller", Classification = Classification.Sophomore },
                new Student { Id = 7,  Code = "ew26", Name = "Ethan Wright", Classification = Classification.Freshman },
                new Student { Id = 8,  Code = "fl24", Name = "Fiona Lewis", Classification = Classification.Junior },
                new Student { Id = 9,  Code = "gh25", Name = "George Hall", Classification = Classification.Sophomore },
                new Student { Id = 10, Code = "iy26", Name = "Isabel Young", Classification = Classification.Freshman }
        };

        public static List<Instructor> Instructors = new List<Instructor>
        {
                new Instructor { Id = 1, Name = "Dr. Smith",   YearsOfExperience = 10 },
                new Instructor { Id = 2, Name = "Prof. Jones", YearsOfExperience = 5  },
                new Instructor { Id = 3, Name = "Dr. Taylor",  YearsOfExperience = 15 },
        };

        public static List<Course> Courses = new List<Course>
            {
                new Course
                {
                    Id = 1,
                    Code = "COP4530",
                    Name = "Data Structures II",
                    Description = "Advanced data structures and algorithmic analysis.",
                    SectionNumber = 1,
                    SemesterTaught = new Semester(2026, SemesterType.Fall),
                    Instructors = new List<Instructor>
                    {
                        FakeDatabase.Instructors.FirstOrDefault(i => i.Id == 1)
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
                        new Module
                        {
                            Id = 1, ModuleName = "Unit 1: Review of Linked Lists",
                            Content = new List<string> { "Singly Linked Lists Video", "Doubly Linked Lists PDF", "Big O Notation Cheat Sheet" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent    { Id = 1, Name = "Singly Linked Lists Video",   Body = "This page covers singly linked list fundamentals including traversal, insertion, and deletion." },
                                new FileContent    { Id = 2, Name = "Doubly Linked Lists PDF",      FilePath = "doubly_linked.pdf", MimeType = "application/pdf" },
                                new PageContent    { Id = 3, Name = "Big O Notation Cheat Sheet",   Body = "O(1) - Constant\nO(log n) - Logarithmic\nO(n) - Linear\nO(n log n) - Linearithmic\nO(n²) - Quadratic" },
                            }
                        },
                        new Module
                        {
                            Id = 2, ModuleName = "Unit 2: Trees and Graphs",
                            Content = new List<string> { "AVL Tree Visualization", "Dijkstra's Algorithm Overview", "Tree Traversal PDF" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent    { Id = 4, Name = "AVL Tree Visualization",        Body = "AVL trees maintain balance by ensuring the height difference between left and right subtrees is at most 1." },
                                new PageContent    { Id = 5, Name = "Dijkstra's Algorithm Overview", Body = "Dijkstra's algorithm finds the shortest path from a source node to all other nodes in a weighted graph." },
                                new FileContent    { Id = 6, Name = "Tree Traversal PDF",            FilePath = "/resources/tree_traversal.pdf", MimeType = "application/pdf" },
                            }
                        },
                        new Module
                        {
                            Id = 3, ModuleName = "Unit 3: Hash Tables",
                            Content = new List<string> { "Hash Functions Video", "Collision Resolution PDF", "Practice Problems" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent    { Id = 7,  Name = "Hash Functions Video",      Body = "A hash function maps keys to indices in a hash table. Good hash functions distribute keys uniformly." },
                                new FileContent    { Id = 8,  Name = "Collision Resolution PDF",  FilePath = "/resources/collision_resolution.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 9, AssignmentId = 1 },
                            }
                        },
                        new Module
                        {
                            Id = 4, ModuleName = "Unit 4: Heaps and Priority Queues",
                            Content = new List<string> { "Min/Max Heap Video", "Priority Queue Use Cases", "Heap Implementation Guide" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 10, Name = "Min/Max Heap Video",          Body = "A min-heap ensures the parent node is always smaller than its children. A max-heap is the reverse." },
                                new PageContent { Id = 11, Name = "Priority Queue Use Cases",    Body = "Priority queues are used in Dijkstra's algorithm, A* search, CPU scheduling, and Huffman coding." },
                                new AssignmentContent { Id = 12, AssignmentId = 2 },
                            }
                        },
                        new Module
                        {
                            Id = 5, ModuleName = "Unit 5: Sorting Algorithms",
                            Content = new List<string> { "Merge Sort Video", "Quick Sort PDF", "Sorting Comparison Chart" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 13, Name = "Merge Sort Video",           Body = "Merge sort divides the array in half recursively, then merges sorted halves. Time complexity: O(n log n)." },
                                new FileContent { Id = 14, Name = "Quick Sort PDF",             FilePath = "/resources/quicksort.pdf", MimeType = "application/pdf" },
                                new PageContent { Id = 15, Name = "Sorting Comparison Chart",  Body = "Bubble: O(n²) | Selection: O(n²) | Insertion: O(n²) | Merge: O(n log n) | Quick: O(n log n) avg" },
                            }
                        },
                        new Module
                        {
                            Id = 6,  ModuleName = "Unit 6: Graph Algorithms",
                            Content = new List<string> { "BFS Video", "DFS PDF", "Shortest Path Problems" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 16, Name = "BFS Video",                 Body = "Breadth-First Search explores all neighbors at the current depth before moving to the next level." },
                                new FileContent { Id = 17, Name = "DFS PDF",                   FilePath = "/resources/dfs.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 18, AssignmentId = 3 },
                            }
                        },
                        new Module
                        {
                            Id = 7,  ModuleName = "Unit 7: Dynamic Programming",
                            Content = new List<string> { "Memoization Video", "Tabulation PDF", "Classic DP Problems" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 19, Name = "Memoization Video",         Body = "Memoization stores results of expensive function calls and returns the cached result when the same inputs occur again." },
                                new FileContent { Id = 20, Name = "Tabulation PDF",            FilePath = "/resources/tabulation.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 21, AssignmentId = 4 },
                            }
                        },
                        new Module
                        {
                            Id = 8,  ModuleName = "Unit 8: Tries",
                            Content = new List<string> { "Trie Structure Video", "Autocomplete Implementation", "Trie vs Hash Table" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 22, Name = "Trie Structure Video",          Body = "A trie is a tree-like data structure used to store strings where each node represents a character." },
                                new PageContent { Id = 23, Name = "Autocomplete Implementation",   Body = "Autocomplete uses a trie to find all words with a given prefix in O(k) time where k is the prefix length." },
                                new PageContent { Id = 24, Name = "Trie vs Hash Table",            Body = "Tries offer O(k) lookup vs O(1) hash table, but tries support prefix search and are more memory intensive." },
                            }
                        },
                        new Module
                        {
                            Id = 9,  ModuleName = "Unit 9: Disjoint Sets",
                            Content = new List<string> { "Union-Find Video", "Path Compression PDF", "Kruskal's Algorithm" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 25, Name = "Union-Find Video",          Body = "Union-Find tracks a set of elements partitioned into disjoint subsets using union and find operations." },
                                new FileContent { Id = 26, Name = "Path Compression PDF",      FilePath = "/resources/path_compression.pdf", MimeType = "application/pdf" },
                                new PageContent { Id = 27, Name = "Kruskal's Algorithm",       Body = "Kruskal's builds a minimum spanning tree by sorting edges by weight and adding them if they don't form a cycle." },
                            }
                        },
                        new Module
                        {
                            Id = 10, ModuleName = "Unit 10: Algorithm Design Review",
                            Content = new List<string> { "Final Exam Study Guide", "Past Exam Problems", "Review Slides" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new FileContent { Id = 28, Name = "Final Exam Study Guide",   FilePath = "/resources/cop4530_study_guide.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 29, AssignmentId = 5 },
                                new FileContent { Id = 30, Name = "Review Slides",            FilePath = "/resources/cop4530_review.pdf", MimeType = "application/pdf" },
                            }
                        },
                    },
                    Roster = new List<Student>
                    {
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 1),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 2),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 3),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 4),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 5),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 6),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 7),
                    }
                },

                new Course
                {
                    Id = 2,
                    Code = "COP3330",
                    Name = "Object Oriented Programming",
                    Description = "Principles of OOP including inheritance, polymorphism, and design patterns.",
                    SectionNumber = 1,
                    SemesterTaught = new Semester(2026, SemesterType.Spring),
                    Instructors = new List<Instructor>
                    {
                        FakeDatabase.Instructors.FirstOrDefault(i => i.Id == 2)
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
                        new Module
                        {
                            Id = 11, ModuleName = "Unit 1: OOP Fundamentals",
                            Content = new List<string> { "Classes and Objects Video", "Encapsulation PDF", "OOP Quiz" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 31, Name = "Classes and Objects Video", Body = "A class is a blueprint for creating objects. Objects are instances of classes with state and behavior." },
                                new FileContent { Id = 32, Name = "Encapsulation PDF",         FilePath = "/resources/encapsulation.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 33, Name = "This shouldn't be there", AssignmentId = 6 },
                            }
                        },
                        new Module
                        {
                            Id = 12, ModuleName = "Unit 2: Inheritance",
                            Content = new List<string> { "Inheritance Video", "Base and Derived Classes PDF", "Practice Exercises" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 34, Name = "Inheritance Video",              Body = "Inheritance allows a class to acquire properties and methods of another class, promoting code reuse." },
                                new FileContent { Id = 35, Name = "Base and Derived Classes PDF",   FilePath = "/resources/inheritance.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 36, AssignmentId = 7 },
                            }
                        },
                        new Module
                        {
                            Id = 13, ModuleName = "Unit 3: Polymorphism",
                            Content = new List<string> { "Runtime Polymorphism Video", "Method Overriding PDF", "Polymorphism Examples" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 37, Name = "Runtime Polymorphism Video",  Body = "Runtime polymorphism allows a base class reference to call overridden methods in derived classes." },
                                new FileContent { Id = 38, Name = "Method Overriding PDF",        FilePath = "/resources/method_overriding.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 39, AssignmentId = 8 },
                            }
                        },
                        new Module
                        {
                            Id = 14, ModuleName = "Unit 4: Abstract Classes",
                            Content = new List<string> { "Abstract vs Interface Video", "When to Use Each PDF", "Lab Instructions" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 40, Name = "Abstract vs Interface Video", Body = "Abstract classes can have implementation; interfaces define contracts. Use abstract classes for shared behavior." },
                                new FileContent { Id = 41, Name = "When to Use Each PDF",        FilePath = "/resources/abstract_vs_interface.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 42, AssignmentId = 9 },
                            }
                        },
                        new Module
                        {
                            Id = 15, ModuleName = "Unit 5: Interfaces",
                            Content = new List<string> { "Interface Design Video", "Multiple Interface Implementation", "Practice Problems" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 43, Name = "Interface Design Video",              Body = "Interfaces define a contract that implementing classes must fulfill. They support multiple inheritance." },
                                new PageContent { Id = 44, Name = "Multiple Interface Implementation",   Body = "A class can implement multiple interfaces in C#, enabling flexible and decoupled designs." },
                                new AssignmentContent { Id = 45, AssignmentId = 10 },
                            }
                        },
                        new Module
                        {
                            Id = 16, ModuleName = "Unit 6: SOLID Principles",
                            Content = new List<string> { "SOLID Overview Video", "Single Responsibility PDF", "Open/Closed Principle Examples" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 46, Name = "SOLID Overview Video",               Body = "SOLID: Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion." },
                                new FileContent { Id = 47, Name = "Single Responsibility PDF",          FilePath = "/resources/solid.pdf", MimeType = "application/pdf" },
                                new PageContent { Id = 48, Name = "Open/Closed Principle Examples",     Body = "Classes should be open for extension but closed for modification. Use inheritance and interfaces to extend." },
                            }
                        },
                        new Module
                        {
                            Id = 17, ModuleName = "Unit 7: Design Patterns Intro",
                            Content = new List<string> { "Creational Patterns Video", "Structural Patterns PDF", "Pattern Catalog" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 49, Name = "Creational Patterns Video",  Body = "Creational patterns deal with object creation: Singleton, Factory, Abstract Factory, Builder, Prototype." },
                                new FileContent { Id = 50, Name = "Structural Patterns PDF",    FilePath = "/resources/structural_patterns.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 51, AssignmentId = 11 },
                            }
                        },
                        new Module
                        {
                            Id = 18, ModuleName = "Unit 8: Singleton and Factory",
                            Content = new List<string> { "Singleton Pattern Video", "Factory Method PDF", "Implementation Lab" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 52, Name = "Singleton Pattern Video",    Body = "The Singleton pattern ensures a class has only one instance and provides a global access point to it." },
                                new FileContent { Id = 53, Name = "Factory Method PDF",         FilePath = "/resources/factory_method.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 54, AssignmentId = 10 },
                            }
                        },
                        new Module
                        {
                            Id = 19, ModuleName = "Unit 9: Observer Pattern",
                            Content = new List<string> { "Observer Overview Video", "Event-Driven Design PDF", "Observer Lab" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 55, Name = "Observer Overview Video",    Body = "The Observer pattern defines a one-to-many dependency so when one object changes state, dependents are notified." },
                                new FileContent { Id = 56, Name = "Event-Driven Design PDF",    FilePath = "/resources/observer.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 57, AssignmentId = 10 },
                            }
                        },
                        new Module
                        {
                            Id = 20, ModuleName = "Unit 10: Final Review",
                            Content = new List<string> { "OOP Review Slides", "Final Project Guidelines", "Past Exam Questions" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new FileContent { Id = 58, Name = "OOP Review Slides",          FilePath = "/resources/oop_review.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 59, AssignmentId = 10 },
                                new FileContent { Id = 60, Name = "Past Exam Questions",         FilePath = "/resources/oop_past_exams.pdf", MimeType = "application/pdf" },
                            }
                        },
                    },
                    Roster = new List<Student>
                    {
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 1),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 3),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 5),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 6),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 7),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 8),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 9),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 10),
                    }
                },

                new Course
                {
                    Id = 3,
                    Code = "CDA3101",
                    Name = "Computer Organization",
                    Description = "Study of computer hardware organization, assembly language, and memory systems.",
                    SectionNumber = 1,
                    SemesterTaught = new Semester(2025, SemesterType.SummerA),
                    Instructors = new List<Instructor>
                    {
                        FakeDatabase.Instructors.FirstOrDefault(i => i.Id == 3)
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
                        new Module
                        {
                            Id = 21, ModuleName = "Unit 1: Binary and Data Representation",
                            Content = new List<string> { "Binary Numbers Video", "Two's Complement PDF", "Practice Problems" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 61, Name = "Binary Numbers Video",       Body = "Binary uses base-2 with digits 0 and 1. Each position represents a power of 2." },
                                new FileContent { Id = 62, Name = "Two's Complement PDF",       FilePath = "/resources/twos_complement.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 63, AssignmentId = 12 },
                            }
                        },
                        new Module
                        {
                            Id = 22, ModuleName = "Unit 2: Logic Gates",
                            Content = new List<string> { "AND/OR/NOT Video", "Circuit Diagrams PDF", "Gate Lab" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 64, Name = "AND/OR/NOT Video",           Body = "Logic gates are the building blocks of digital circuits. AND outputs 1 only if both inputs are 1." },
                                new FileContent { Id = 65, Name = "Circuit Diagrams PDF",       FilePath = "/resources/circuit_diagrams.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 66, AssignmentId = 13 },
                            }
                        },
                        new Module
                        {
                            Id = 23, ModuleName = "Unit 3: MIPS Assembly",
                            Content = new List<string> { "MIPS Instruction Set PDF", "Assembly Video Tutorial", "MIPS Simulator Guide" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new FileContent { Id = 67, Name = "MIPS Instruction Set PDF",  FilePath = "/resources/mips_isa.pdf", MimeType = "application/pdf" },
                                new PageContent { Id = 68, Name = "Assembly Video Tutorial",    Body = "MIPS assembly uses 32 registers. $t0-$t9 are temporary, $s0-$s7 are saved, $a0-$a3 are arguments." },
                                new AssignmentContent { Id = 69, AssignmentId = 14 },
                            }
                        },
                        new Module
                        {
                            Id = 24, ModuleName = "Unit 4: ALU Design",
                            Content = new List<string> { "ALU Overview Video", "Adder Circuits PDF", "Design Lab Instructions" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 70, Name = "ALU Overview Video",         Body = "An ALU performs arithmetic and logic operations. It takes two operands and an operation code as input." },
                                new FileContent { Id = 71, Name = "Adder Circuits PDF",         FilePath = "/resources/adder_circuits.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 72, AssignmentId = 16 },
                            }
                        },
                        new Module
                        {
                            Id = 25, ModuleName = "Unit 5: CPU Datapath",
                            Content = new List<string> { "Datapath Video", "Control Unit PDF", "Single Cycle CPU Diagram" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 73, Name = "Datapath Video",             Body = "The datapath is the hardware that performs operations on data. It includes the ALU, registers, and buses." },
                                new FileContent { Id = 74, Name = "Control Unit PDF",           FilePath = "/resources/control_unit.pdf", MimeType = "application/pdf" },
                                new PageContent { Id = 75, Name = "Single Cycle CPU Diagram",   Body = "A single-cycle CPU executes each instruction in one clock cycle. Simple but inefficient for complex instructions." },
                            }
                        },
                        new Module
                        {
                            Id = 26, ModuleName = "Unit 6: Pipelining",
                            Content = new List<string> { "Pipeline Stages Video", "Hazard Detection PDF", "Pipeline Simulation" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 76, Name = "Pipeline Stages Video",      Body = "MIPS pipeline: IF (Instruction Fetch), ID (Decode), EX (Execute), MEM (Memory), WB (Write Back)." },
                                new FileContent { Id = 77, Name = "Hazard Detection PDF",       FilePath = "/resources/hazard_detection.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 78, AssignmentId = 17 },
                            }
                        },
                        new Module
                        {
                            Id = 27, ModuleName = "Unit 7: Cache Memory",
                            Content = new List<string> { "Cache Basics Video", "Direct Mapped Cache PDF", "Cache Lab" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 79, Name = "Cache Basics Video",         Body = "Cache memory stores frequently accessed data closer to the CPU to reduce latency." },
                                new FileContent { Id = 80, Name = "Direct Mapped Cache PDF",    FilePath = "/resources/cache.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 81, AssignmentId = 18 },
                            }
                        },
                        new Module
                        {
                            Id = 28, ModuleName = "Unit 8: Virtual Memory",
                            Content = new List<string> { "Paging Video", "Page Tables PDF", "TLB Overview" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 82, Name = "Paging Video",               Body = "Paging divides virtual memory into fixed-size pages mapped to physical frames by the OS." },
                                new FileContent { Id = 83, Name = "Page Tables PDF",            FilePath = "/resources/page_tables.pdf", MimeType = "application/pdf" },
                                new PageContent { Id = 84, Name = "TLB Overview",              Body = "The Translation Lookaside Buffer caches recent page table entries to speed up virtual-to-physical address translation." },
                            }
                        },
                        new Module
                        {
                            Id = 29, ModuleName = "Unit 9: I/O Systems",
                            Content = new List<string> { "I/O Devices Video", "Interrupts PDF", "DMA Overview" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 85, Name = "I/O Devices Video",          Body = "I/O devices communicate with the CPU through controllers, ports, and buses." },
                                new FileContent { Id = 86, Name = "Interrupts PDF",             FilePath = "/resources/interrupts.pdf", MimeType = "application/pdf" },
                                new PageContent { Id = 87, Name = "DMA Overview",              Body = "Direct Memory Access allows peripherals to transfer data to/from memory without CPU involvement." },
                            }
                        },
                        new Module
                        {
                            Id = 30, ModuleName = "Unit 10: Final Review",
                            Content = new List<string> { "Exam Study Guide", "Past Exams", "Review Lecture Slides" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new FileContent { Id = 88, Name = "Exam Study Guide",          FilePath = "/resources/cda3101_study_guide.pdf", MimeType = "application/pdf" },
                                new FileContent { Id = 89, Name = "Past Exams",                FilePath = "/resources/cda3101_past_exams.pdf", MimeType = "application/pdf" },
                                new FileContent { Id = 90, Name = "Review Lecture Slides",     FilePath = "/resources/cda3101_review.pdf", MimeType = "application/pdf" },
                            }
                        },
                    },
                    Roster = new List<Student>
                    {
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 2),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 3),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 4),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 5),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 7),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 8),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 9),
                    }
                },

                new Course
                {
                    Id = 4,
                    Code = "COP4020",
                    Name = "Programming Languages",
                    Description = "Survey of programming language concepts, paradigms, and implementation.",
                    SectionNumber = 1,
                    SemesterTaught = new Semester(2026, SemesterType.SummerB),
                    Instructors = new List<Instructor>
                    {
                        FakeDatabase.Instructors.FirstOrDefault(i => i.Id == 1)
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
                        new Module
                        {
                            Id = 31, ModuleName = "Unit 1: Language Paradigms",
                            Content = new List<string> { "Paradigms Overview Video", "Imperative vs Declarative PDF", "Paradigm Quiz" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 91, Name = "Paradigms Overview Video",       Body = "Programming paradigms include imperative, declarative, functional, object-oriented, and logic programming." },
                                new FileContent { Id = 92, Name = "Imperative vs Declarative PDF",  FilePath = "/resources/paradigms.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 93, AssignmentId = 19 },
                            }
                        },
                        new Module
                        {
                            Id = 32, ModuleName = "Unit 2: Syntax and Grammars",
                            Content = new List<string> { "BNF Grammar Video", "Parse Trees PDF", "Grammar Exercises" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 94, Name = "BNF Grammar Video",          Body = "Backus-Naur Form defines the syntax of a language using production rules with terminals and non-terminals." },
                                new FileContent { Id = 95, Name = "Parse Trees PDF",            FilePath = "/resources/parse_trees.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 96, AssignmentId = 20 },
                            }
                        },
                        new Module
                        {
                            Id = 33, ModuleName = "Unit 3: Lexical Analysis",
                            Content = new List<string> { "Tokenization Video", "Regular Expressions PDF", "Lexer Lab" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 97, Name = "Tokenization Video",         Body = "Lexical analysis converts source code into tokens — the smallest meaningful units like keywords and identifiers." },
                                new FileContent { Id = 98, Name = "Regular Expressions PDF",    FilePath = "/resources/regex.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 99, AssignmentId = 21 },
                            }
                        },
                        new Module
                        {
                            Id = 34, ModuleName = "Unit 4: Parsing",
                            Content = new List<string> { "Top-Down Parsing Video", "LL vs LR Parsers PDF", "Parser Lab" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 100, Name = "Top-Down Parsing Video",    Body = "Top-down parsers start from the root of the parse tree and work down to the leaves using the grammar rules." },
                                new FileContent { Id = 101, Name = "LL vs LR Parsers PDF",      FilePath = "/resources/ll_lr_parsers.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 102, AssignmentId = 22 },
                            }
                        },
                        new Module
                        {
                            Id = 35, ModuleName = "Unit 5: Type Systems",
                            Content = new List<string> { "Static vs Dynamic Video", "Type Inference PDF", "Type Checking Examples" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 103, Name = "Static vs Dynamic Video",   Body = "Static typing checks types at compile time. Dynamic typing checks at runtime. Both have tradeoffs." },
                                new FileContent { Id = 104, Name = "Type Inference PDF",        FilePath = "/resources/type_inference.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 105, AssignmentId = 23 },
                            }
                        },
                        new Module
                        {
                            Id = 36, ModuleName = "Unit 6: Functional Programming",
                            Content = new List<string> { "Lambda Calculus Video", "Haskell Intro PDF", "Functional Lab" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 106, Name = "Lambda Calculus Video",     Body = "Lambda calculus is the theoretical foundation of functional programming using anonymous functions and substitution." },
                                new FileContent { Id = 107, Name = "Haskell Intro PDF",         FilePath = "/resources/haskell_intro.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 108, AssignmentId = 19 },
                            }
                        },
                        new Module
                        {
                            Id = 37, ModuleName = "Unit 7: Logic Programming",
                            Content = new List<string> { "Prolog Basics Video", "Unification PDF", "Prolog Lab" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 109, Name = "Prolog Basics Video",       Body = "Prolog is a logic programming language. Programs consist of facts and rules. Queries find solutions via unification." },
                                new FileContent { Id = 110, Name = "Unification PDF",           FilePath = "/resources/unification.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 111, AssignmentId = 23 },
                            }
                        },
                        new Module
                        {
                            Id = 38, ModuleName = "Unit 8: Memory Management",
                            Content = new List<string> { "Garbage Collection Video", "Manual vs Automatic PDF", "Memory Lab" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 112, Name = "Garbage Collection Video",  Body = "Garbage collection automatically reclaims memory no longer in use. Common algorithms: mark-and-sweep, reference counting." },
                                new FileContent { Id = 113, Name = "Manual vs Automatic PDF",   FilePath = "/resources/memory_management.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 114, AssignmentId = 22 },
                            }
                        },
                        new Module
                        {
                            Id = 39, ModuleName = "Unit 9: Concurrency",
                            Content = new List<string> { "Threads Video", "Race Conditions PDF", "Concurrency Examples" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 115, Name = "Threads Video",             Body = "Threads are lightweight units of execution within a process. They share memory but have separate stacks." },
                                new FileContent { Id = 116, Name = "Race Conditions PDF",       FilePath = "/resources/race_conditions.pdf", MimeType = "application/pdf" },
                                new PageContent { Id = 117, Name = "Concurrency Examples",      Body = "Deadlock occurs when two threads wait on each other indefinitely. Use locks carefully to avoid circular dependencies." },
                            }
                        },
                        new Module
                        {
                            Id = 40, ModuleName = "Unit 10: Final Review",
                            Content = new List<string> { "Language Concepts Summary", "Final Project Guide", "Past Exam Questions" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new FileContent { Id = 118, Name = "Language Concepts Summary", FilePath = "/resources/cop4020_summary.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 119, AssignmentId = 22 },
                                new FileContent { Id = 120, Name = "Past Exam Questions",      FilePath = "/resources/cop4020_past_exams.pdf", MimeType = "application/pdf" },
                            }
                        },
                    },
                    Roster = new List<Student>
                    {
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 1),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 2),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 4),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 6),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 8),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 9),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 10),
                    }
                },

                new Course
                {
                    Id = 5,
                    Code = "CNT4007",
                    Name = "Computer Networks",
                    Description = "Fundamentals of computer networking including protocols, routing, and security.",
                    SectionNumber = 1,
                    SemesterTaught = new Semester(2025, SemesterType.Fall),
                    Instructors = new List<Instructor>
                    {
                        FakeDatabase.Instructors.FirstOrDefault(i => i.Id == 2)
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
                        new Module
                        {
                            Id = 41, ModuleName = "Unit 1: Network Fundamentals",
                            Content = new List<string> { "Network Types Video", "LAN vs WAN PDF", "Networking Quiz" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 121, Name = "Network Types Video",       Body = "Networks are classified by size: PAN, LAN, MAN, WAN. Each serves different geographic and functional scopes." },
                                new FileContent { Id = 122, Name = "LAN vs WAN PDF",            FilePath = "/resources/lan_wan.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 123, AssignmentId = 24 },
                            }
                        },
                        new Module
                        {
                            Id = 42, ModuleName = "Unit 2: OSI Model",
                            Content = new List<string> { "OSI Layers Video", "Layer Functions PDF", "OSI Examples" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 124, Name = "OSI Layers Video",          Body = "The OSI model has 7 layers: Physical, Data Link, Network, Transport, Session, Presentation, Application." },
                                new FileContent { Id = 125, Name = "Layer Functions PDF",       FilePath = "/resources/osi_layers.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 126, AssignmentId = 25 },
                            }
                        },
                        new Module
                        {
                            Id = 43, ModuleName = "Unit 3: TCP/IP",
                            Content = new List<string> { "TCP vs UDP Video", "IP Addressing PDF", "TCP Lab" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 127, Name = "TCP vs UDP Video",          Body = "TCP is connection-oriented and reliable. UDP is connectionless and faster but offers no delivery guarantee." },
                                new FileContent { Id = 128, Name = "IP Addressing PDF",         FilePath = "/resources/ip_addressing.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 129, AssignmentId = 26 },
                            }
                        },
                        new Module
                        {
                            Id = 44, ModuleName = "Unit 4: Application Layer",
                            Content = new List<string> { "HTTP Overview Video", "DNS PDF", "Application Layer Lab" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 130, Name = "HTTP Overview Video",       Body = "HTTP is the foundation of data communication on the web. It follows a request-response model over TCP." },
                                new FileContent { Id = 131, Name = "DNS PDF",                   FilePath = "/resources/dns.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 132, AssignmentId = 27 },
                            }
                        },
                        new Module
                        {
                            Id = 45, ModuleName = "Unit 5: Transport Layer",
                            Content = new List<string> { "Flow Control Video", "Congestion Control PDF", "Transport Lab" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 133, Name = "Flow Control Video",        Body = "Flow control prevents a fast sender from overwhelming a slow receiver using sliding window protocols." },
                                new FileContent { Id = 134, Name = "Congestion Control PDF",    FilePath = "/resources/congestion_control.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 135, AssignmentId = 28 },
                            }
                        },
                        new Module
                        {
                            Id = 46, ModuleName = "Unit 6: Network Layer",
                            Content = new List<string> { "IP Routing Video", "Subnetting PDF", "Routing Lab" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 136, Name = "IP Routing Video",          Body = "Routers forward packets based on IP addresses using routing tables built by protocols like OSPF and BGP." },
                                new FileContent { Id = 137, Name = "Subnetting PDF",            FilePath = "/resources/subnetting.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 138, AssignmentId = 29 },
                            }
                        },
                        new Module
                        {
                            Id = 47, ModuleName = "Unit 7: Data Link Layer",
                            Content = new List<string> { "MAC Addresses Video", "Ethernet PDF", "Switch Lab" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 139, Name = "MAC Addresses Video",       Body = "MAC addresses are 48-bit hardware identifiers assigned to network interfaces for communication on a local network." },
                                new FileContent { Id = 140, Name = "Ethernet PDF",              FilePath = "/resources/ethernet.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 141, AssignmentId = 27 },
                            }
                        },
                        new Module
                        {
                            Id = 48, ModuleName = "Unit 8: Wireless Networks",
                            Content = new List<string> { "WiFi Standards Video", "802.11 PDF", "Wireless Security" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 142, Name = "WiFi Standards Video",      Body = "IEEE 802.11 standards define WiFi. 802.11ac (WiFi 5) and 802.11ax (WiFi 6) offer gigabit speeds." },
                                new FileContent { Id = 143, Name = "802.11 PDF",                FilePath = "/resources/80211.pdf", MimeType = "application/pdf" },
                                new PageContent { Id = 144, Name = "Wireless Security",         Body = "WPA3 is the current WiFi security standard. It replaces WPA2 and provides stronger encryption and authentication." },
                            }
                        },
                        new Module
                        {
                            Id = 49, ModuleName = "Unit 9: Network Security",
                            Content = new List<string> { "Encryption Video", "Firewalls PDF", "VPN Overview" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 145, Name = "Encryption Video",          Body = "TLS encrypts data in transit. AES is used for symmetric encryption. RSA is used for key exchange." },
                                new FileContent { Id = 146, Name = "Firewalls PDF",             FilePath = "/resources/firewalls.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 147, AssignmentId = 30 },
                            }
                        },
                        new Module
                        {
                            Id = 50, ModuleName = "Unit 10: Final Review",
                            Content = new List<string> { "Network Concepts Summary", "Final Project Guidelines", "Practice Exams" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new FileContent { Id = 148, Name = "Network Concepts Summary",  FilePath = "/resources/cnt4007_summary.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 149, AssignmentId = 30 },
                                new FileContent { Id = 150, Name = "Practice Exams",            FilePath = "/resources/cnt4007_past_exams.pdf", MimeType = "application/pdf" },
                            }
                        },
                    },
                    Roster = new List<Student>
                    {
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 1),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 3),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 4),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 5),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 6),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 8),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 9),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 10),
                    }
                },

                new Course
                {
                    Id = 6,
                    Code = "CNT4007",
                    Name = "Computer Networks",
                    Description = "Fundamentals of computer networking including protocols, routing, and security.",
                    SectionNumber = 2,
                    SemesterTaught = new Semester(2024, SemesterType.Fall),
                    Instructors = new List<Instructor>
                    {
                        FakeDatabase.Instructors.FirstOrDefault(i => i.Id == 2)
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
                        new Module
                        {
                            Id = 51, ModuleName = "Unit 1: Network Fundamentals",
                            Content = new List<string> { "Network Types Video", "LAN vs WAN PDF", "Networking Quiz" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 151, Name = "Network Types Video",       Body = "Networks are classified by size: PAN, LAN, MAN, WAN. Each serves different geographic and functional scopes." },
                                new FileContent { Id = 152, Name = "LAN vs WAN PDF",            FilePath = "/resources/lan_wan.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 153, AssignmentId = 24 },
                            }
                        },
                        new Module
                        {
                            Id = 52, ModuleName = "Unit 2: OSI Model",
                            Content = new List<string> { "OSI Layers Video", "Layer Functions PDF", "OSI Examples" },
                            ModuleContents = new List<ModuleContent>
                            {
                                new PageContent { Id = 154, Name = "OSI Layers Video",          Body = "The OSI model has 7 layers: Physical, Data Link, Network, Transport, Session, Presentation, Application." },
                                new FileContent { Id = 155, Name = "Layer Functions PDF",       FilePath = "/resources/osi_layers.pdf", MimeType = "application/pdf" },
                                new AssignmentContent { Id = 156, AssignmentId = 25 },
                            }
                        },
                        new Module { Id = 53, ModuleName = "Unit 3: TCP/IP",           Content = new List<string> { "TCP vs UDP Video", "IP Addressing PDF", "TCP Lab" },                      ModuleContents = new List<ModuleContent> { new PageContent { Id = 157, Name = "TCP vs UDP Video", Body = "TCP is reliable and connection-oriented. UDP is fast but unreliable." }, new FileContent { Id = 158, Name = "IP Addressing PDF", FilePath = "/resources/ip_addressing.pdf", MimeType = "application/pdf" }, new AssignmentContent { Id = 159, Name = "TCP Lab", AssignmentId = 25 } } },
                        new Module { Id = 54, ModuleName = "Unit 4: Application Layer", Content = new List<string> { "HTTP Overview Video", "DNS PDF", "Application Layer Lab" },             ModuleContents = new List<ModuleContent> { new PageContent { Id = 160, Name = "HTTP Overview Video", Body = "HTTP follows a request-response model over TCP." }, new FileContent { Id = 161, Name = "DNS PDF", FilePath = "/resources/dns.pdf", MimeType = "application/pdf" }, new AssignmentContent { Id = 162, Name = "Application Layer Lab", AssignmentId = 29 } } },
                        new Module { Id = 55, ModuleName = "Unit 5: Transport Layer",   Content = new List<string> { "Flow Control Video", "Congestion Control PDF", "Transport Lab" },        ModuleContents = new List<ModuleContent> { new PageContent { Id = 163, Name = "Flow Control Video", Body = "Flow control prevents overwhelming the receiver." }, new FileContent { Id = 164, Name = "Congestion Control PDF", FilePath = "/resources/congestion_control.pdf", MimeType = "application/pdf" }, new AssignmentContent { Id = 165, Name = "Transport Lab", AssignmentId = 25 } } },
                        new Module { Id = 56, ModuleName = "Unit 6: Network Layer",     Content = new List<string> { "IP Routing Video", "Subnetting PDF", "Routing Lab" },                   ModuleContents = new List<ModuleContent> { new PageContent { Id = 166, Name = "IP Routing Video", Body = "Routers forward packets using routing tables." }, new FileContent { Id = 167, Name = "Subnetting PDF", FilePath = "/resources/subnetting.pdf", MimeType = "application/pdf" }, new AssignmentContent { Id = 168, Name = "Routing Lab", AssignmentId = 26 } } },
                        new Module { Id = 57, ModuleName = "Unit 7: Data Link Layer",   Content = new List<string> { "MAC Addresses Video", "Ethernet PDF", "Switch Lab" },                   ModuleContents = new List<ModuleContent> { new PageContent { Id = 169, Name = "MAC Addresses Video", Body = "MAC addresses are 48-bit hardware identifiers." }, new FileContent { Id = 170, Name = "Ethernet PDF", FilePath = "/resources/ethernet.pdf", MimeType = "application/pdf" }, new AssignmentContent { Id = 171, Name = "Switch Lab", AssignmentId = 27 } } },
                        new Module { Id = 58, ModuleName = "Unit 8: Wireless Networks", Content = new List<string> { "WiFi Standards Video", "802.11 PDF", "Wireless Security" },             ModuleContents = new List<ModuleContent> { new PageContent { Id = 172, Name = "WiFi Standards Video", Body = "802.11ax (WiFi 6) offers gigabit speeds." }, new FileContent { Id = 173, Name = "802.11 PDF", FilePath = "/resources/80211.pdf", MimeType = "application/pdf" }, new PageContent { Id = 174, Name = "Wireless Security", Body = "WPA3 is the current WiFi security standard." } } },
                        new Module { Id = 59, ModuleName = "Unit 9: Network Security",  Content = new List<string> { "Encryption Video", "Firewalls PDF", "VPN Overview" },                   ModuleContents = new List<ModuleContent> { new PageContent { Id = 175, Name = "Encryption Video", Body = "TLS encrypts data in transit using AES and RSA." }, new FileContent { Id = 176, Name = "Firewalls PDF", FilePath = "/resources/firewalls.pdf", MimeType = "application/pdf" }, new AssignmentContent { Id = 177, Name = "VPN Overview", AssignmentId = 28 } } },
                        new Module { Id = 60, ModuleName = "Unit 10: Final Review",     Content = new List<string> { "Network Concepts Summary", "Final Project Guidelines", "Practice Exams" }, ModuleContents = new List<ModuleContent> { new FileContent { Id = 178, Name = "Network Concepts Summary", FilePath = "/resources/cnt4007_summary.pdf", MimeType = "application/pdf" }, new AssignmentContent { Id = 179, Name = "Final Project Guidelines", AssignmentId = 30 }, new FileContent { Id = 180, Name = "Practice Exams", FilePath = "/resources/cnt4007_past_exams.pdf", MimeType = "application/pdf" } } },
                    },
                    Roster = new List<Student>
                    {
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 1),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 3),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 4),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 6),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 8),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 9),
                        FakeDatabase.Students.FirstOrDefault(i => i.Id == 10),
                    }
                },
            };
    }
}
-- ============================================================
-- Canvas LMS Seed Data
-- ============================================================
-- Instructors (5)
-- ============================================================
INSERT INTO "User" ("Name", "Code", "UserType", "Email")
VALUES
    ('Dr. Sarah Mitchell',   'SMITCHELL', 'Instructor', ''),
    ('Prof. James Hartwell', 'JHARTWELL', 'Instructor', ''),
    ('Dr. Elena Vasquez',    'EVASQUEZ',  'Instructor', ''),
    ('Prof. Marcus Webb',    'MWEBB',     'Instructor', ''),
    ('Dr. Priya Nair',       'PNAIR',     'Instructor', '');

-- ============================================================
-- Students (20) — all emails point to cartergeorge2005@gmail.com
-- ============================================================
INSERT INTO "User" ("Name", "Code", "UserType", "Email")
VALUES
    ('Alice Johnson',    'AJ1001', 'Student', 'cartergeorge2005@gmail.com'),
    ('Brian Carter',     'BC1002', 'Student', 'cartergeorge2005@gmail.com'),
    ('Chloe Davis',      'CD1003', 'Student', 'cartergeorge2005@gmail.com'),
    ('Derek Evans',      'DE1004', 'Student', 'cartergeorge2005@gmail.com'),
    ('Emma Foster',      'EF1005', 'Student', 'cartergeorge2005@gmail.com'),
    ('Felix Green',      'FG1006', 'Student', 'cartergeorge2005@gmail.com'),
    ('Grace Hill',       'GH1007', 'Student', 'cartergeorge2005@gmail.com'),
    ('Henry Ingram',     'HI1008', 'Student', 'cartergeorge2005@gmail.com'),
    ('Isabella Jones',   'IJ1009', 'Student', 'cartergeorge2005@gmail.com'),
    ('Jake Kim',         'JK1010', 'Student', 'cartergeorge2005@gmail.com'),
    ('Karen Lee',        'KL1011', 'Student', 'cartergeorge2005@gmail.com'),
    ('Liam Moore',       'LM1012', 'Student', 'cartergeorge2005@gmail.com'),
    ('Mia Nelson',       'MN1013', 'Student', 'cartergeorge2005@gmail.com'),
    ('Noah Owens',       'NO1014', 'Student', 'cartergeorge2005@gmail.com'),
    ('Olivia Parker',    'OP1015', 'Student', 'cartergeorge2005@gmail.com'),
    ('Peter Quinn',      'PQ1016', 'Student', 'cartergeorge2005@gmail.com'),
    ('Quinn Roberts',    'QR1017', 'Student', 'cartergeorge2005@gmail.com'),
    ('Rachel Scott',     'RS1018', 'Student', 'cartergeorge2005@gmail.com'),
    ('Samuel Turner',    'ST1019', 'Student', 'cartergeorge2005@gmail.com'),
    ('Tara Underwood',   'TU1020', 'Student', 'cartergeorge2005@gmail.com');

-- ============================================================
-- Courses (6)
-- ============================================================
INSERT INTO "Courses" (
    "Name", "Code", "Description", "SectionNumber",
    "SemesterTaught_Session", "SemesterTaught_Year",
    "SemesterTaught_StartDate", "SemesterTaught_EndDate"
)
VALUES
    ('Introduction to Computer Science', 'COP1000', 'Fundamentals of computing and programming.', 1, 1, 2026, '2026-01-13 00:00:00Z'::timestamp, '2026-05-02 00:00:00Z'::timestamp),
    ('Data Structures and Algorithms',   'COP2510', 'Arrays, linked lists, trees, sorting, and searching.', 1, 1, 2026, '2026-01-13 00:00:00Z'::timestamp, '2026-05-02 00:00:00Z'::timestamp),
    ('Database Systems',                 'COP3710', 'Relational databases, SQL, and data modeling.', 2, 1, 2026, '2026-01-13 00:00:00Z'::timestamp, '2026-05-02 00:00:00Z'::timestamp),
    ('Software Engineering',             'COP4331', 'SDLC, Agile, design patterns, and team projects.', 1, 2, 2026, '2026-08-25 00:00:00Z'::timestamp, '2026-12-11 00:00:00Z'::timestamp),
    ('Operating Systems',                'COP4600', 'Processes, memory management, file systems, and concurrency.', 1, 2, 2026, '2026-08-25 00:00:00Z'::timestamp, '2026-12-11 00:00:00Z'::timestamp),
    ('Web Application Development',      'COP4810', 'Full-stack web development with modern frameworks.', 3, 2, 2026, '2026-08-25 00:00:00Z'::timestamp, '2026-12-11 00:00:00Z'::timestamp);

-- ============================================================
-- Course–Instructor assignments (up to 2 per course)
-- Instructor IDs are looked up by code to avoid hardcoding
-- ============================================================
INSERT INTO "CourseInstructors" ("CourseId", "InstructorsId")
SELECT c."Id", u."Id"
FROM "Courses" c, "User" u
WHERE c."Code" = 'COP1000' AND u."Code" IN ('SMITCHELL', 'JHARTWELL') AND u."UserType" = 'Instructor';

INSERT INTO "CourseInstructors" ("CourseId", "InstructorsId")
SELECT c."Id", u."Id"
FROM "Courses" c, "User" u
WHERE c."Code" = 'COP2510' AND u."Code" IN ('JHARTWELL', 'EVASQUEZ') AND u."UserType" = 'Instructor';

INSERT INTO "CourseInstructors" ("CourseId", "InstructorsId")
SELECT c."Id", u."Id"
FROM "Courses" c, "User" u
WHERE c."Code" = 'COP3710' AND u."Code" IN ('EVASQUEZ') AND u."UserType" = 'Instructor';

INSERT INTO "CourseInstructors" ("CourseId", "InstructorsId")
SELECT c."Id", u."Id"
FROM "Courses" c, "User" u
WHERE c."Code" = 'COP4331' AND u."Code" IN ('MWEBB', 'PNAIR') AND u."UserType" = 'Instructor';

INSERT INTO "CourseInstructors" ("CourseId", "InstructorsId")
SELECT c."Id", u."Id"
FROM "Courses" c, "User" u
WHERE c."Code" = 'COP4600' AND u."Code" IN ('PNAIR') AND u."UserType" = 'Instructor';

INSERT INTO "CourseInstructors" ("CourseId", "InstructorsId")
SELECT c."Id", u."Id"
FROM "Courses" c, "User" u
WHERE c."Code" = 'COP4810' AND u."Code" IN ('SMITCHELL', 'MWEBB') AND u."UserType" = 'Instructor';

-- ============================================================
-- Roster enrollments (6–9 students per course)
-- ============================================================

-- COP1000 — 9 students
INSERT INTO "CourseStudents" ("CourseId", "RosterId")
SELECT c."Id", u."Id" FROM "Courses" c, "User" u
WHERE c."Code" = 'COP1000' AND u."Code" IN ('AJ1001','BC1002','CD1003','DE1004','EF1005','FG1006','GH1007','HI1008','IJ1009') AND u."UserType" = 'Student';

-- COP2510 — 8 students
INSERT INTO "CourseStudents" ("CourseId", "RosterId")
SELECT c."Id", u."Id" FROM "Courses" c, "User" u
WHERE c."Code" = 'COP2510' AND u."Code" IN ('JK1010','KL1011','LM1012','MN1013','NO1014','OP1015','PQ1016','QR1017') AND u."UserType" = 'Student';

-- COP3710 — 7 students
INSERT INTO "CourseStudents" ("CourseId", "RosterId")
SELECT c."Id", u."Id" FROM "Courses" c, "User" u
WHERE c."Code" = 'COP3710' AND u."Code" IN ('RS1018','ST1019','TU1020','AJ1001','BC1002','CD1003','DE1004') AND u."UserType" = 'Student';

-- COP4331 — 8 students
INSERT INTO "CourseStudents" ("CourseId", "RosterId")
SELECT c."Id", u."Id" FROM "Courses" c, "User" u
WHERE c."Code" = 'COP4331' AND u."Code" IN ('EF1005','FG1006','GH1007','HI1008','IJ1009','JK1010','KL1011','LM1012') AND u."UserType" = 'Student';

-- COP4600 — 6 students
INSERT INTO "CourseStudents" ("CourseId", "RosterId")
SELECT c."Id", u."Id" FROM "Courses" c, "User" u
WHERE c."Code" = 'COP4600' AND u."Code" IN ('MN1013','NO1014','OP1015','PQ1016','QR1017','RS1018') AND u."UserType" = 'Student';

-- COP4810 — 9 students
INSERT INTO "CourseStudents" ("CourseId", "RosterId")
SELECT c."Id", u."Id" FROM "Courses" c, "User" u
WHERE c."Code" = 'COP4810' AND u."Code" IN ('ST1019','TU1020','AJ1001','BC1002','EF1005','FG1006','NO1014','OP1015','PQ1016') AND u."UserType" = 'Student';

-- ============================================================
-- Assignments
-- IsQuiz = false (regular), IsQuiz = true (quiz) — 2 quizzes per course
-- at least 7 per course, 2 will be referenced by ModuleContent
-- ============================================================

-- COP1000
INSERT INTO "Assignments" ("CourseId", "Name", "Description", "AvailablePoints", "DueDate", "GroupId", "IsQuiz")
SELECT c."Id", a."Name", a."Description", a."Points", a."Due", 0, a."IsQuiz"
FROM "Courses" c,
(VALUES
    ('Syllabus Quiz',              'Quiz covering course policies and syllabus.',        20,  '2026-01-20 23:59:00Z'::timestamp, true),
    ('Hello World Program',        'Write your first program in Python.',                50,  '2026-01-27 23:59:00Z'::timestamp, false),
    ('Variables and Types',        'Exercises on data types and variable assignment.',   50,  '2026-02-03 23:59:00Z'::timestamp, false),
    ('Control Flow Midterm Quiz',  'Quiz on conditionals and loops.',                   30,  '2026-02-17 23:59:00Z'::timestamp, true),
    ('Functions Lab',              'Write and call functions with parameters.',          75,  '2026-03-03 23:59:00Z'::timestamp, false),
    ('Lists and Dictionaries',     'Practice with Python collections.',                 75,  '2026-03-17 23:59:00Z'::timestamp, false),
    ('File I/O Assignment',        'Read and write data to files.',                     80,  '2026-04-07 23:59:00Z'::timestamp, false),
    ('Final Project',              'Build a small Python application of your choice.',  150, '2026-04-28 23:59:00Z'::timestamp, false)
) AS a("Name","Description","Points","Due","IsQuiz")
WHERE c."Code" = 'COP1000';

-- COP2510
INSERT INTO "Assignments" ("CourseId", "Name", "Description", "AvailablePoints", "DueDate", "GroupId", "IsQuiz")
SELECT c."Id", a."Name", a."Description", a."Points", a."Due", 0, a."IsQuiz"
FROM "Courses" c,
(VALUES
    ('Arrays and Complexity Quiz', 'Quiz on Big-O and array operations.',               25,  '2026-01-22 23:59:00Z'::timestamp, true),
    ('Linked List Implementation', 'Implement a singly linked list in Java.',            75,  '2026-02-05 23:59:00Z'::timestamp, false),
    ('Stack and Queue Lab',        'Implement stack and queue using arrays.',            75,  '2026-02-19 23:59:00Z'::timestamp, false),
    ('Recursion Problems',         'Solve 5 recursive problems.',                       60,  '2026-03-05 23:59:00Z'::timestamp, false),
    ('Trees Midterm Quiz',         'Quiz on binary trees and traversals.',               30,  '2026-03-12 23:59:00Z'::timestamp, true),
    ('Binary Search Tree',         'Implement BST insert, delete, and search.',         90,  '2026-03-26 23:59:00Z'::timestamp, false),
    ('Sorting Algorithms Lab',     'Implement and benchmark merge sort and quicksort.', 80,  '2026-04-09 23:59:00Z'::timestamp, false),
    ('Graph Traversal Assignment', 'Implement BFS and DFS on an adjacency list.',       90,  '2026-04-23 23:59:00Z'::timestamp, false)
) AS a("Name","Description","Points","Due","IsQuiz")
WHERE c."Code" = 'COP2510';

-- COP3710
INSERT INTO "Assignments" ("CourseId", "Name", "Description", "AvailablePoints", "DueDate", "GroupId", "IsQuiz")
SELECT c."Id", a."Name", a."Description", a."Points", a."Due", 0, a."IsQuiz"
FROM "Courses" c,
(VALUES
    ('ER Diagram Assignment',      'Design an ER diagram for a given scenario.',         60,  '2026-01-28 23:59:00Z'::timestamp, false),
    ('SQL Basics Quiz',            'Quiz on SELECT, WHERE, and JOIN.',                   25,  '2026-02-04 23:59:00Z'::timestamp, true),
    ('Normalization Lab',          'Normalize a schema to 3NF.',                         75,  '2026-02-18 23:59:00Z'::timestamp, false),
    ('Advanced SQL Queries',       'Write complex queries with subqueries and views.',   80,  '2026-03-04 23:59:00Z'::timestamp, false),
    ('Transactions Quiz',          'Quiz on ACID properties and transactions.',          25,  '2026-03-18 23:59:00Z'::timestamp, true),
    ('Stored Procedures Lab',      'Write stored procedures and triggers.',              80,  '2026-04-01 23:59:00Z'::timestamp, false),
    ('Database Design Project',    'Design and implement a full relational database.',  150,  '2026-04-28 23:59:00Z'::timestamp, false)
) AS a("Name","Description","Points","Due","IsQuiz")
WHERE c."Code" = 'COP3710';

-- COP4331
INSERT INTO "Assignments" ("CourseId", "Name", "Description", "AvailablePoints", "DueDate", "GroupId", "IsQuiz")
SELECT c."Id", a."Name", a."Description", a."Points", a."Due", 0, a."IsQuiz"
FROM "Courses" c,
(VALUES
    ('Agile Concepts Quiz',        'Quiz on Scrum, sprints, and Agile principles.',      25,  '2026-09-05 23:59:00Z'::timestamp, true),
    ('Requirements Document',      'Write a software requirements specification.',       80,  '2026-09-16 23:59:00Z'::timestamp, false),
    ('UML Diagrams Lab',           'Create use case and class diagrams.',                70,  '2026-09-30 23:59:00Z'::timestamp, false),
    ('Design Patterns Quiz',       'Quiz on common Gang of Four patterns.',              25,  '2026-10-14 23:59:00Z'::timestamp, true),
    ('Sprint 1 Deliverable',       'First working sprint of your team project.',        100,  '2026-10-21 23:59:00Z'::timestamp, false),
    ('Code Review Lab',            'Conduct and document a formal code review.',         60,  '2026-11-04 23:59:00Z'::timestamp, false),
    ('Sprint 2 Deliverable',       'Second working sprint with new features.',          100,  '2026-11-18 23:59:00Z'::timestamp, false),
    ('Final Project Presentation', 'Present your completed software project.',          150,  '2026-12-08 23:59:00Z'::timestamp, false)
) AS a("Name","Description","Points","Due","IsQuiz")
WHERE c."Code" = 'COP4331';

-- COP4600
INSERT INTO "Assignments" ("CourseId", "Name", "Description", "AvailablePoints", "DueDate", "GroupId", "IsQuiz")
SELECT c."Id", a."Name", a."Description", a."Points", a."Due", 0, a."IsQuiz"
FROM "Courses" c,
(VALUES
    ('Processes Quiz',             'Quiz on process states and scheduling.',             25,  '2026-09-08 23:59:00Z'::timestamp, true),
    ('Shell Scripting Lab',        'Write Bash scripts to automate system tasks.',       70,  '2026-09-22 23:59:00Z'::timestamp, false),
    ('Process Scheduling Sim',     'Simulate FCFS and Round Robin scheduling.',          90,  '2026-10-06 23:59:00Z'::timestamp, false),
    ('Memory Management Quiz',     'Quiz on paging, segmentation, and virtual memory.', 25,  '2026-10-20 23:59:00Z'::timestamp, true),
    ('Memory Allocator Lab',       'Implement a simple memory allocator in C.',         100,  '2026-11-03 23:59:00Z'::timestamp, false),
    ('File System Assignment',     'Implement a basic file system structure.',          100,  '2026-11-17 23:59:00Z'::timestamp, false),
    ('Concurrency Lab',            'Solve producer-consumer with semaphores.',           90,  '2026-12-01 23:59:00Z'::timestamp, false)
) AS a("Name","Description","Points","Due","IsQuiz")
WHERE c."Code" = 'COP4600';

-- COP4810
INSERT INTO "Assignments" ("CourseId", "Name", "Description", "AvailablePoints", "DueDate", "GroupId", "IsQuiz")
SELECT c."Id", a."Name", a."Description", a."Points", a."Due", 0, a."IsQuiz"
FROM "Courses" c,
(VALUES
    ('HTML & CSS Quiz',            'Quiz on semantic HTML and CSS box model.',           25,  '2026-09-05 23:59:00Z'::timestamp, true),
    ('Static Portfolio Page',      'Build a personal portfolio with HTML and CSS.',      70,  '2026-09-19 23:59:00Z'::timestamp, false),
    ('JavaScript DOM Lab',         'Manipulate the DOM with vanilla JavaScript.',        75,  '2026-10-03 23:59:00Z'::timestamp, false),
    ('REST API Quiz',              'Quiz on HTTP methods, status codes, and REST.',      25,  '2026-10-17 23:59:00Z'::timestamp, true),
    ('Backend API Lab',            'Build a REST API with Node.js and Express.',        100,  '2026-10-31 23:59:00Z'::timestamp, false),
    ('Database Integration',       'Connect your API to a relational database.',        100,  '2026-11-14 23:59:00Z'::timestamp, false),
    ('Authentication Lab',         'Add JWT authentication to your application.',        90,  '2026-11-21 23:59:00Z'::timestamp, false),
    ('Full Stack Final Project',   'Deploy a complete full-stack web application.',     150,  '2026-12-08 23:59:00Z'::timestamp, false)
) AS a("Name","Description","Points","Due","IsQuiz")
WHERE c."Code" = 'COP4810';

-- ============================================================
-- Assignment Groups
-- ============================================================

-- COP1000: Labs group
INSERT INTO "AssignmentGroups" ("CourseId", "Name", "TotalPoints", "AssignmentIds")
SELECT c."Id", 'Labs', 100, '[]'::jsonb
FROM "Courses" c WHERE c."Code" = 'COP1000';

-- COP2510: Core Implementations group
INSERT INTO "AssignmentGroups" ("CourseId", "Name", "TotalPoints", "AssignmentIds")
SELECT c."Id", 'Core Implementations', 150, '[]'::jsonb
FROM "Courses" c WHERE c."Code" = 'COP2510';

-- COP4331: Sprint Deliverables group
INSERT INTO "AssignmentGroups" ("CourseId", "Name", "TotalPoints", "AssignmentIds")
SELECT c."Id", 'Sprint Deliverables', 200, '[]'::jsonb
FROM "Courses" c WHERE c."Code" = 'COP4331';

-- COP4810: Backend Work group
INSERT INTO "AssignmentGroups" ("CourseId", "Name", "TotalPoints", "AssignmentIds")
SELECT c."Id", 'Backend Work', 150, '[]'::jsonb
FROM "Courses" c WHERE c."Code" = 'COP4810';

-- Now populate AssignmentIds jsonb and set GroupId on assignments

-- COP1000 Labs: Functions Lab + Lists and Dictionaries + File I/O
UPDATE "AssignmentGroups" ag
SET "AssignmentIds" = (
    SELECT jsonb_agg(a."Id")
    FROM "Assignments" a
    WHERE a."CourseId" = ag."CourseId"
    AND a."Name" IN ('Functions Lab', 'Lists and Dictionaries', 'File I/O Assignment')
)
WHERE ag."Name" = 'Labs'
AND ag."CourseId" IN (SELECT "Id" FROM "Courses" WHERE "Code" = 'COP1000');

UPDATE "Assignments" a
SET "GroupId" = ag."Id"
FROM "AssignmentGroups" ag
JOIN "Courses" c ON c."Id" = ag."CourseId"
WHERE ag."Name" = 'Labs'
AND c."Code" = 'COP1000'
AND a."CourseId" = c."Id"
AND a."Name" IN ('Functions Lab', 'Lists and Dictionaries', 'File I/O Assignment');

-- COP2510 Core Implementations: Linked List + BST + Graph Traversal
UPDATE "AssignmentGroups" ag
SET "AssignmentIds" = (
    SELECT jsonb_agg(a."Id")
    FROM "Assignments" a
    WHERE a."CourseId" = ag."CourseId"
    AND a."Name" IN ('Linked List Implementation', 'Binary Search Tree', 'Graph Traversal Assignment')
)
WHERE ag."Name" = 'Core Implementations'
AND ag."CourseId" IN (SELECT "Id" FROM "Courses" WHERE "Code" = 'COP2510');

UPDATE "Assignments" a
SET "GroupId" = ag."Id"
FROM "AssignmentGroups" ag
JOIN "Courses" c ON c."Id" = ag."CourseId"
WHERE ag."Name" = 'Core Implementations'
AND c."Code" = 'COP2510'
AND a."CourseId" = c."Id"
AND a."Name" IN ('Linked List Implementation', 'Binary Search Tree', 'Graph Traversal Assignment');

-- COP4331 Sprint Deliverables: Sprint 1 + Sprint 2 + Final Presentation
UPDATE "AssignmentGroups" ag
SET "AssignmentIds" = (
    SELECT jsonb_agg(a."Id")
    FROM "Assignments" a
    WHERE a."CourseId" = ag."CourseId"
    AND a."Name" IN ('Sprint 1 Deliverable', 'Sprint 2 Deliverable', 'Final Project Presentation')
)
WHERE ag."Name" = 'Sprint Deliverables'
AND ag."CourseId" IN (SELECT "Id" FROM "Courses" WHERE "Code" = 'COP4331');

UPDATE "Assignments" a
SET "GroupId" = ag."Id"
FROM "AssignmentGroups" ag
JOIN "Courses" c ON c."Id" = ag."CourseId"
WHERE ag."Name" = 'Sprint Deliverables'
AND c."Code" = 'COP4331'
AND a."CourseId" = c."Id"
AND a."Name" IN ('Sprint 1 Deliverable', 'Sprint 2 Deliverable', 'Final Project Presentation');

-- COP4810 Backend Work: Backend API Lab + Database Integration + Authentication Lab
UPDATE "AssignmentGroups" ag
SET "AssignmentIds" = (
    SELECT jsonb_agg(a."Id")
    FROM "Assignments" a
    WHERE a."CourseId" = ag."CourseId"
    AND a."Name" IN ('Backend API Lab', 'Database Integration', 'Authentication Lab')
)
WHERE ag."Name" = 'Backend Work'
AND ag."CourseId" IN (SELECT "Id" FROM "Courses" WHERE "Code" = 'COP4810');

UPDATE "Assignments" a
SET "GroupId" = ag."Id"
FROM "AssignmentGroups" ag
JOIN "Courses" c ON c."Id" = ag."CourseId"
WHERE ag."Name" = 'Backend Work'
AND c."Code" = 'COP4810'
AND a."CourseId" = c."Id"
AND a."Name" IN ('Backend API Lab', 'Database Integration', 'Authentication Lab');

-- ============================================================
-- Announcements (4 per course)
-- ============================================================
INSERT INTO "Announcements" ("CourseId", "Title", "Body", "PostedDate")
SELECT c."Id", a."Title", a."Body", a."Posted"
FROM "Courses" c,
(VALUES
    ('Welcome to COP1000!',         'Welcome everyone! Please review the syllabus before our first class.', '2026-01-12 09:00:00Z'::timestamp),
    ('Office Hours Posted',         'My office hours are Mondays and Wednesdays 2–4pm in room 301.',        '2026-01-15 10:00:00Z'::timestamp),
    ('Midterm Reminder',            'The midterm quiz is coming up. Review chapters 1–5.',                  '2026-02-10 08:00:00Z'::timestamp),
    ('Final Project Guidelines',    'Final project guidelines have been posted to the modules section.',    '2026-03-20 09:00:00Z'::timestamp)
) AS a("Title","Body","Posted")
WHERE c."Code" = 'COP1000';

INSERT INTO "Announcements" ("CourseId", "Title", "Body", "PostedDate")
SELECT c."Id", a."Title", a."Body", a."Posted"
FROM "Courses" c,
(VALUES
    ('Welcome to Data Structures!', 'This course moves fast — stay on top of weekly labs.',                '2026-01-12 09:00:00Z'::timestamp),
    ('Java Environment Setup',      'Make sure you have JDK 21 installed before Thursday.',               '2026-01-14 11:00:00Z'::timestamp),
    ('Midterm Study Guide',         'A study guide for the trees quiz has been posted.',                   '2026-03-08 09:00:00Z'::timestamp),
    ('Graph Assignment Tips',       'Start the graph traversal assignment early — it is more involved.',   '2026-04-15 08:00:00Z'::timestamp)
) AS a("Title","Body","Posted")
WHERE c."Code" = 'COP2510';

INSERT INTO "Announcements" ("CourseId", "Title", "Body", "PostedDate")
SELECT c."Id", a."Title", a."Body", a."Posted"
FROM "Courses" c,
(VALUES
    ('Welcome to Database Systems!','We will be using PostgreSQL throughout this course.',                 '2026-01-12 09:00:00Z'::timestamp),
    ('pgAdmin Setup Guide',         'Instructions for setting up pgAdmin are in Module 1.',               '2026-01-16 10:00:00Z'::timestamp),
    ('Normalization Lab Extended',  'The normalization lab deadline has been extended by one week.',       '2026-02-12 08:00:00Z'::timestamp),
    ('Final Project Teams',         'Please submit your project team of 2–3 by end of week.',             '2026-03-25 09:00:00Z'::timestamp)
) AS a("Title","Body","Posted")
WHERE c."Code" = 'COP3710';

INSERT INTO "Announcements" ("CourseId", "Title", "Body", "PostedDate")
SELECT c."Id", a."Title", a."Body", a."Posted"
FROM "Courses" c,
(VALUES
    ('Welcome to Software Eng!',    'Teams will be assigned in week 2. Start thinking about project ideas.','2026-08-24 09:00:00Z'::timestamp),
    ('Agile Reading Posted',        'Please read chapters 1–3 of the Agile textbook before Thursday.',     '2026-08-28 10:00:00Z'::timestamp),
    ('Sprint 1 Check-In',           'Come to class with your sprint backlog ready for review.',             '2026-10-14 08:00:00Z'::timestamp),
    ('Presentation Schedule',       'Final presentation slots have been posted — check the schedule.',      '2026-11-20 09:00:00Z'::timestamp)
) AS a("Title","Body","Posted")
WHERE c."Code" = 'COP4331';

INSERT INTO "Announcements" ("CourseId", "Title", "Body", "PostedDate")
SELECT c."Id", a."Title", a."Body", a."Posted"
FROM "Courses" c,
(VALUES
    ('Welcome to OS!',              'This is a challenging course. Attend every lecture and lab.',         '2026-08-24 09:00:00Z'::timestamp),
    ('Linux VM Required',           'You will need a Linux VM for all labs. Ubuntu 22.04 recommended.',   '2026-08-27 10:00:00Z'::timestamp),
    ('Memory Quiz Prep',            'Review the paging and segmentation slides carefully.',                '2026-10-16 08:00:00Z'::timestamp),
    ('Concurrency Lab Hint',        'Use POSIX semaphores for the producer-consumer lab.',                 '2026-11-25 09:00:00Z'::timestamp)
) AS a("Title","Body","Posted")
WHERE c."Code" = 'COP4600';

INSERT INTO "Announcements" ("CourseId", "Title", "Body", "PostedDate")
SELECT c."Id", a."Title", a."Body", a."Posted"
FROM "Courses" c,
(VALUES
    ('Welcome to Web Dev!',         'We will cover HTML, CSS, JS, Node, and deployment this semester.',   '2026-08-24 09:00:00Z'::timestamp),
    ('Tools Setup',                 'Install VS Code, Node.js 20, and Postman before first lab.',         '2026-08-27 10:00:00Z'::timestamp),
    ('REST API Resources',          'Extra reading on REST design has been posted to Module 4.',           '2026-10-14 08:00:00Z'::timestamp),
    ('Final Project Deploy',        'Your final project must be deployed to a live URL — not localhost.', '2026-11-22 09:00:00Z'::timestamp)
) AS a("Title","Body","Posted")
WHERE c."Code" = 'COP4810';

-- ============================================================
-- Modules (6 per course)
-- ============================================================
INSERT INTO "Modules" ("CourseId", "ModuleName")
SELECT c."Id", m."Name"
FROM "Courses" c,
(VALUES
    ('Module 1: Getting Started'),
    ('Module 2: Core Concepts'),
    ('Module 3: Intermediate Topics'),
    ('Module 4: Advanced Concepts'),
    ('Module 5: Projects'),
    ('Module 6: Review and Wrap-Up')
) AS m("Name")
WHERE c."Code" = 'COP1000';

INSERT INTO "Modules" ("CourseId", "ModuleName")
SELECT c."Id", m."Name"
FROM "Courses" c,
(VALUES
    ('Module 1: Arrays and Complexity'),
    ('Module 2: Linked Structures'),
    ('Module 3: Stacks, Queues, and Recursion'),
    ('Module 4: Trees'),
    ('Module 5: Sorting and Searching'),
    ('Module 6: Graphs')
) AS m("Name")
WHERE c."Code" = 'COP2510';

INSERT INTO "Modules" ("CourseId", "ModuleName")
SELECT c."Id", m."Name"
FROM "Courses" c,
(VALUES
    ('Module 1: Intro to Databases'),
    ('Module 2: Relational Model and SQL'),
    ('Module 3: Normalization'),
    ('Module 4: Advanced SQL'),
    ('Module 5: Transactions and Concurrency'),
    ('Module 6: Final Project')
) AS m("Name")
WHERE c."Code" = 'COP3710';

INSERT INTO "Modules" ("CourseId", "ModuleName")
SELECT c."Id", m."Name"
FROM "Courses" c,
(VALUES
    ('Module 1: Agile and SDLC'),
    ('Module 2: Requirements Engineering'),
    ('Module 3: UML and Design'),
    ('Module 4: Design Patterns'),
    ('Module 5: Sprint Work'),
    ('Module 6: Testing and Delivery')
) AS m("Name")
WHERE c."Code" = 'COP4331';

INSERT INTO "Modules" ("CourseId", "ModuleName")
SELECT c."Id", m."Name"
FROM "Courses" c,
(VALUES
    ('Module 1: Intro to OS'),
    ('Module 2: Process Management'),
    ('Module 3: Scheduling'),
    ('Module 4: Memory Management'),
    ('Module 5: File Systems'),
    ('Module 6: Concurrency')
) AS m("Name")
WHERE c."Code" = 'COP4600';

INSERT INTO "Modules" ("CourseId", "ModuleName")
SELECT c."Id", m."Name"
FROM "Courses" c,
(VALUES
    ('Module 1: Web Foundations'),
    ('Module 2: JavaScript'),
    ('Module 3: Frontend Frameworks'),
    ('Module 4: Backend Development'),
    ('Module 5: Databases and Auth'),
    ('Module 6: Deployment and Final Project')
) AS m("Name")
WHERE c."Code" = 'COP4810';

-- ============================================================
-- Module Contents
-- PageContent = 'Page', AssignmentContent = 'Assignment'
-- Each module gets up to 3 content items; at least 2 per course
-- are AssignmentContent tied to real assignments
-- ============================================================

-- COP1000 ─────────────────────────────────────────────────

-- Module 1: Getting Started — Page + AssignmentContent (Hello World)
INSERT INTO "ModuleContents" ("ModuleId", "Name", "Body", "Discriminator")
SELECT m."Id", 'Course Overview', 'Welcome to COP1000. This page covers what to expect this semester, grading, and tools.', 'Page'
FROM "Modules" m JOIN "Courses" c ON c."Id" = m."CourseId"
WHERE c."Code" = 'COP1000' AND m."ModuleName" = 'Module 1: Getting Started';

INSERT INTO "ModuleContents" ("ModuleId", "Name", "Discriminator", "AssignmentId")
SELECT m."Id", 'Hello World Program', 'Assignment', a."Id"
FROM "Modules" m
JOIN "Courses" c ON c."Id" = m."CourseId"
JOIN "Assignments" a ON a."CourseId" = c."Id" AND a."Name" = 'Hello World Program'
WHERE c."Code" = 'COP1000' AND m."ModuleName" = 'Module 1: Getting Started';

-- Module 2: Core Concepts — Page + Page
INSERT INTO "ModuleContents" ("ModuleId", "Name", "Body", "Discriminator")
SELECT m."Id", 'Variables and Types Notes', 'Covers integers, floats, strings, and booleans in Python with examples.', 'Page'
FROM "Modules" m JOIN "Courses" c ON c."Id" = m."CourseId"
WHERE c."Code" = 'COP1000' AND m."ModuleName" = 'Module 2: Core Concepts';

INSERT INTO "ModuleContents" ("ModuleId", "Name", "Body", "Discriminator")
SELECT m."Id", 'Control Flow Lecture Notes', 'Detailed notes on if/else, for loops, and while loops.', 'Page'
FROM "Modules" m JOIN "Courses" c ON c."Id" = m."CourseId"
WHERE c."Code" = 'COP1000' AND m."ModuleName" = 'Module 2: Core Concepts';

-- Module 5: Projects — AssignmentContent (Final Project)
INSERT INTO "ModuleContents" ("ModuleId", "Name", "Discriminator", "AssignmentId")
SELECT m."Id", 'Final Project', 'Assignment', a."Id"
FROM "Modules" m
JOIN "Courses" c ON c."Id" = m."CourseId"
JOIN "Assignments" a ON a."CourseId" = c."Id" AND a."Name" = 'Final Project'
WHERE c."Code" = 'COP1000' AND m."ModuleName" = 'Module 5: Projects';

INSERT INTO "ModuleContents" ("ModuleId", "Name", "Body", "Discriminator")
SELECT m."Id", 'Project Rubric', 'Grading criteria and expectations for the final project submission.', 'Page'
FROM "Modules" m JOIN "Courses" c ON c."Id" = m."CourseId"
WHERE c."Code" = 'COP1000' AND m."ModuleName" = 'Module 5: Projects';

-- Module 6: Review — Page
INSERT INTO "ModuleContents" ("ModuleId", "Name", "Body", "Discriminator")
SELECT m."Id", 'Final Exam Study Guide', 'Key topics and sample questions for the final exam review.', 'Page'
FROM "Modules" m JOIN "Courses" c ON c."Id" = m."CourseId"
WHERE c."Code" = 'COP1000' AND m."ModuleName" = 'Module 6: Review and Wrap-Up';

-- COP2510 ─────────────────────────────────────────────────

INSERT INTO "ModuleContents" ("ModuleId", "Name", "Body", "Discriminator")
SELECT m."Id", 'Big-O Notation Reference', 'A reference sheet covering common time complexities with examples.', 'Page'
FROM "Modules" m JOIN "Courses" c ON c."Id" = m."CourseId"
WHERE c."Code" = 'COP2510' AND m."ModuleName" = 'Module 1: Arrays and Complexity';

INSERT INTO "ModuleContents" ("ModuleId", "Name", "Discriminator", "AssignmentId")
SELECT m."Id", 'Linked List Implementation', 'Assignment', a."Id"
FROM "Modules" m
JOIN "Courses" c ON c."Id" = m."CourseId"
JOIN "Assignments" a ON a."CourseId" = c."Id" AND a."Name" = 'Linked List Implementation'
WHERE c."Code" = 'COP2510' AND m."ModuleName" = 'Module 2: Linked Structures';

INSERT INTO "ModuleContents" ("ModuleId", "Name", "Body", "Discriminator")
SELECT m."Id", 'Recursion Patterns', 'Common recursive patterns: factorial, fibonacci, tree traversal.', 'Page'
FROM "Modules" m JOIN "Courses" c ON c."Id" = m."CourseId"
WHERE c."Code" = 'COP2510' AND m."ModuleName" = 'Module 3: Stacks, Queues, and Recursion';

INSERT INTO "ModuleContents" ("ModuleId", "Name", "Discriminator", "AssignmentId")
SELECT m."Id", 'Binary Search Tree', 'Assignment', a."Id"
FROM "Modules" m
JOIN "Courses" c ON c."Id" = m."CourseId"
JOIN "Assignments" a ON a."CourseId" = c."Id" AND a."Name" = 'Binary Search Tree'
WHERE c."Code" = 'COP2510' AND m."ModuleName" = 'Module 4: Trees';

INSERT INTO "ModuleContents" ("ModuleId", "Name", "Body", "Discriminator")
SELECT m."Id", 'Sorting Algorithm Comparison', 'Side-by-side complexity comparison of bubble, merge, and quicksort.', 'Page'
FROM "Modules" m JOIN "Courses" c ON c."Id" = m."CourseId"
WHERE c."Code" = 'COP2510' AND m."ModuleName" = 'Module 5: Sorting and Searching';

-- COP3710 ─────────────────────────────────────────────────

INSERT INTO "ModuleContents" ("ModuleId", "Name", "Body", "Discriminator")
SELECT m."Id", 'What is a Database?', 'Introduction to relational databases, tables, keys, and relationships.', 'Page'
FROM "Modules" m JOIN "Courses" c ON c."Id" = m."CourseId"
WHERE c."Code" = 'COP3710' AND m."ModuleName" = 'Module 1: Intro to Databases';

INSERT INTO "ModuleContents" ("ModuleId", "Name", "Discriminator", "AssignmentId")
SELECT m."Id", 'ER Diagram Assignment', 'Assignment', a."Id"
FROM "Modules" m
JOIN "Courses" c ON c."Id" = m."CourseId"
JOIN "Assignments" a ON a."CourseId" = c."Id" AND a."Name" = 'ER Diagram Assignment'
WHERE c."Code" = 'COP3710' AND m."ModuleName" = 'Module 1: Intro to Databases';

INSERT INTO "ModuleContents" ("ModuleId", "Name", "Body", "Discriminator")
SELECT m."Id", 'SQL JOIN Reference', 'Visual guide to INNER, LEFT, RIGHT, and FULL OUTER joins.', 'Page'
FROM "Modules" m JOIN "Courses" c ON c."Id" = m."CourseId"
WHERE c."Code" = 'COP3710' AND m."ModuleName" = 'Module 2: Relational Model and SQL';

INSERT INTO "ModuleContents" ("ModuleId", "Name", "Discriminator", "AssignmentId")
SELECT m."Id", 'Database Design Project', 'Assignment', a."Id"
FROM "Modules" m
JOIN "Courses" c ON c."Id" = m."CourseId"
JOIN "Assignments" a ON a."CourseId" = c."Id" AND a."Name" = 'Database Design Project'
WHERE c."Code" = 'COP3710' AND m."ModuleName" = 'Module 6: Final Project';

INSERT INTO "ModuleContents" ("ModuleId", "Name", "Body", "Discriminator")
SELECT m."Id", 'Project Requirements', 'Detailed requirements and grading rubric for the final database project.', 'Page'
FROM "Modules" m JOIN "Courses" c ON c."Id" = m."CourseId"
WHERE c."Code" = 'COP3710' AND m."ModuleName" = 'Module 6: Final Project';

-- COP4331 ─────────────────────────────────────────────────

INSERT INTO "ModuleContents" ("ModuleId", "Name", "Body", "Discriminator")
SELECT m."Id", 'Agile Manifesto Overview', 'Summary of the 12 Agile principles and how they apply to software teams.', 'Page'
FROM "Modules" m JOIN "Courses" c ON c."Id" = m."CourseId"
WHERE c."Code" = 'COP4331' AND m."ModuleName" = 'Module 1: Agile and SDLC';

INSERT INTO "ModuleContents" ("ModuleId", "Name", "Discriminator", "AssignmentId")
SELECT m."Id", 'Requirements Document', 'Assignment', a."Id"
FROM "Modules" m
JOIN "Courses" c ON c."Id" = m."CourseId"
JOIN "Assignments" a ON a."CourseId" = c."Id" AND a."Name" = 'Requirements Document'
WHERE c."Code" = 'COP4331' AND m."ModuleName" = 'Module 2: Requirements Engineering';

INSERT INTO "ModuleContents" ("ModuleId", "Name", "Body", "Discriminator")
SELECT m."Id", 'UML Quick Reference', 'Cheat sheet for class diagrams, sequence diagrams, and use case diagrams.', 'Page'
FROM "Modules" m JOIN "Courses" c ON c."Id" = m."CourseId"
WHERE c."Code" = 'COP4331' AND m."ModuleName" = 'Module 3: UML and Design';

INSERT INTO "ModuleContents" ("ModuleId", "Name", "Discriminator", "AssignmentId")
SELECT m."Id", 'Sprint 1 Deliverable', 'Assignment', a."Id"
FROM "Modules" m
JOIN "Courses" c ON c."Id" = m."CourseId"
JOIN "Assignments" a ON a."CourseId" = c."Id" AND a."Name" = 'Sprint 1 Deliverable'
WHERE c."Code" = 'COP4331' AND m."ModuleName" = 'Module 5: Sprint Work';

-- COP4600 ─────────────────────────────────────────────────

INSERT INTO "ModuleContents" ("ModuleId", "Name", "Body", "Discriminator")
SELECT m."Id", 'OS Architecture Overview', 'Introduction to kernel vs user space, system calls, and OS types.', 'Page'
FROM "Modules" m JOIN "Courses" c ON c."Id" = m."CourseId"
WHERE c."Code" = 'COP4600' AND m."ModuleName" = 'Module 1: Intro to OS';

INSERT INTO "ModuleContents" ("ModuleId", "Name", "Discriminator", "AssignmentId")
SELECT m."Id", 'Process Scheduling Sim', 'Assignment', a."Id"
FROM "Modules" m
JOIN "Courses" c ON c."Id" = m."CourseId"
JOIN "Assignments" a ON a."CourseId" = c."Id" AND a."Name" = 'Process Scheduling Sim'
WHERE c."Code" = 'COP4600' AND m."ModuleName" = 'Module 3: Scheduling';

INSERT INTO "ModuleContents" ("ModuleId", "Name", "Body", "Discriminator")
SELECT m."Id", 'Virtual Memory Explained', 'Diagrams and explanations of page tables, TLBs, and demand paging.', 'Page'
FROM "Modules" m JOIN "Courses" c ON c."Id" = m."CourseId"
WHERE c."Code" = 'COP4600' AND m."ModuleName" = 'Module 4: Memory Management';

INSERT INTO "ModuleContents" ("ModuleId", "Name", "Discriminator", "AssignmentId")
SELECT m."Id", 'Concurrency Lab', 'Assignment', a."Id"
FROM "Modules" m
JOIN "Courses" c ON c."Id" = m."CourseId"
JOIN "Assignments" a ON a."CourseId" = c."Id" AND a."Name" = 'Concurrency Lab'
WHERE c."Code" = 'COP4600' AND m."ModuleName" = 'Module 6: Concurrency';

-- COP4810 ─────────────────────────────────────────────────

INSERT INTO "ModuleContents" ("ModuleId", "Name", "Body", "Discriminator")
SELECT m."Id", 'How the Web Works', 'Overview of HTTP, DNS, browsers, and client-server architecture.', 'Page'
FROM "Modules" m JOIN "Courses" c ON c."Id" = m."CourseId"
WHERE c."Code" = 'COP4810' AND m."ModuleName" = 'Module 1: Web Foundations';

INSERT INTO "ModuleContents" ("ModuleId", "Name", "Discriminator", "AssignmentId")
SELECT m."Id", 'Static Portfolio Page', 'Assignment', a."Id"
FROM "Modules" m
JOIN "Courses" c ON c."Id" = m."CourseId"
JOIN "Assignments" a ON a."CourseId" = c."Id" AND a."Name" = 'Static Portfolio Page'
WHERE c."Code" = 'COP4810' AND m."ModuleName" = 'Module 1: Web Foundations';

INSERT INTO "ModuleContents" ("ModuleId", "Name", "Body", "Discriminator")
SELECT m."Id", 'JavaScript Fundamentals', 'Covers variables, functions, closures, and async/await in JS.', 'Page'
FROM "Modules" m JOIN "Courses" c ON c."Id" = m."CourseId"
WHERE c."Code" = 'COP4810' AND m."ModuleName" = 'Module 2: JavaScript';

INSERT INTO "ModuleContents" ("ModuleId", "Name", "Discriminator", "AssignmentId")
SELECT m."Id", 'Backend API Lab', 'Assignment', a."Id"
FROM "Modules" m
JOIN "Courses" c ON c."Id" = m."CourseId"
JOIN "Assignments" a ON a."CourseId" = c."Id" AND a."Name" = 'Backend API Lab'
WHERE c."Code" = 'COP4810' AND m."ModuleName" = 'Module 4: Backend Development';

INSERT INTO "ModuleContents" ("ModuleId", "Name", "Body", "Discriminator")
SELECT m."Id", 'Deployment Checklist', 'Steps for deploying a Node.js app to Railway, Render, or a VPS.', 'Page'
FROM "Modules" m JOIN "Courses" c ON c."Id" = m."CourseId"
WHERE c."Code" = 'COP4810' AND m."ModuleName" = 'Module 6: Deployment and Final Project';

-- ============================================================
-- Grade Scales (one per course, standard A–F)
-- ============================================================
INSERT INTO "LetterGrades" ("CourseId", "Letter", "MinPercentage", "MaxPercentage", "HexColor")
SELECT c."Id", g."Letter", g."Min", g."Max", g."Color"
FROM "Courses" c,
(VALUES
    ('A',  93.0, 100.0, '#2E7D32'),
    ('A-', 90.0,  92.9, '#388E3C'),
    ('B+', 87.0,  89.9, '#1565C0'),
    ('B',  83.0,  86.9, '#1976D2'),
    ('B-', 80.0,  82.9, '#00695C'),
    ('C+', 77.0,  79.9, '#F57F17'),
    ('C',  73.0,  76.9, '#E65100'),
    ('C-', 70.0,  72.9, '#BF360C'),
    ('D',  60.0,  69.9, '#C62828'),
    ('F',   0.0,  59.9, '#37474F')
) AS g("Letter","Min","Max","Color")
WHERE c."Code" IN ('COP1000','COP2510','COP3710','COP4331','COP4600','COP4810');
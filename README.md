# Full-Stack Canvas Clone/Learning Management System (LMS)

GUIDs, UUIDs, and offline idempotent synchronization, oh my! This cross-platform Learning Management System (LMS) was engineered to learn fundamental software design patterns, reliable distributed state management practices, and layered architecture. 

This application has too many features to type out. To sum them up: full courses with modules, assignments, announcements, and more for teachers and the student has the ability to chat with their instructors, submit files for assignments, and get email notifications when new assignments are created.

---

## App UI

<p align="center">
  <img width="1509" height="803" alt="LoginPage" src="https://github.com/user-attachments/assets/4681b726-33c9-47ab-b768-f622fd6a9dbe" />
  <br>
  <em>Login Page</em>
</p>

<p align="center">
  <img width="1509" height="803" alt="TeacherCoursesView" src="https://github.com/user-attachments/assets/3cf96067-773f-48f6-a622-605c41a0ecea" />
  <br>
  <em>Teacher's Courses View</em>
</p>

<p align="center">
  <img width="1509" height="803" alt="TeacherManagementView" src="https://github.com/user-attachments/assets/8175baad-7d40-45ce-acfb-80627be75ab3" />
  <br>
  <em>Teacher's View of their modules</em>
</p>

<p align="center">
  <img width="1509" height="803" alt="TeacherAssignmentsView" src="https://github.com/user-attachments/assets/fd9fb4ec-11d9-40cb-b42d-2b12b612d279" />
  <br>
  <em>Teacher's View of their assignments</em>
</p>

<p align="center">
  <img width="1509" height="803" alt="CanvasStudentView" src="https://github.com/user-attachments/assets/b9e4559f-cea2-4963-9cf5-5a098878598a" />
  <br>
  <em>Students Main Course View</em>
</p>

<p align="center">
  <img width="1509" height="803" alt="StudentAssigmentsView" src="https://github.com/user-attachments/assets/6be8f99d-17a7-4c93-b3a8-405a552c2aa2" />
  <br>
  <em>Student's Assignment View</em>
</p>

---

## Key Features

* **Cross-Platform Client:** A single, native codebase targeting multiple platforms smoothly.
* **Offline-First Synchronization:** Implements idempotent synchronization utilizing GUIDs/UUIDs to handle data consistency and prevent collisions during network drops.
* **Automated Event Notifications:** Integrated server-side system event handlers that trigger real-time communications to users via email.
* **Decoupled Architecture:** Clean separation of business logic, data presentation, and data persistence layers.

---

## System Architecture

The project is built on a highly modular blueprint designed to mimic professional production environments:

### Frontend (Client Layer)
* **Framework:** C# & .NET MAUI
* **Pattern:** Model-View-ViewModel (MVVM) for layered state management, data binding, and an optimized, responsive user experience.

### Backend (Service Layer)
* **API Style:** RESTful APIs facilitating secure, high-throughput communication between client and server layers.
* **Pattern:** Controller-Service-Repository architecture to cleanly isolate the entry points (Controllers) from the core business logic (Services) and data access operations (Repositories).

### Database & Integration Layer
* **Database:** PostgreSQL for reliable, relational data persistence handling complex user and enrollment schemas.
* **Protocols:** Integrated Gmail SMTP client acting as an event-driven engine for automated notification delivery.

---

## Tech Stack

* **Frontend:** C# | .NET MAUI
* **Architecture Patterns:** MVVM | Controller-Service-Repository
* **Database:** PostgreSQL
* **APIs & Protocols:** REST | SMTP (Gmail)
* **Concepts:** Idempotent Sync | UUID/GUID Data Modeling

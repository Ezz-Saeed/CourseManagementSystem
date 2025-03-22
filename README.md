Overview
The Course Management API is a RESTful web service built using .NET 8 that enables training companies to manage courses, 
trainers, and payments efficiently. The API supports CRUD operations for courses and trainers, links courses to trainers, handles payments, and provides basic reporting functionality.

Features
CRUD operations for courses and trainers
Linking courses to trainers
Payment management for trainers
Basic reporting functionality for course-trainer linkages
JWT-based authentication and authorization
Unit tests to ensure reliability
API documentation with Swagger
Technology Stack
Backend: .NET 8, C#
Database: Entity Framework Core
Authentication: JWT
Testing: xUnit/NUnit/MSTest (choose one based on implementation)
Documentation: Swagger
Installation & Setup
Prerequisites
.NET 8 SDK
SQL Server (or any configured database)
Postman or any API testing tool

API Endpoints

Courses
Get All Courses: GET /api/courses
Get Course by ID: GET /api/courses/{id}
Create Course: POST /api/courses
Update Course: PUT /api/courses/{id}
Delete Course: DELETE /api/courses/{id}
Assign Trainer to Course: POST /api/courses/{courseId}/assign/{trainerId}
Remove Trainer from Course: DELETE /api/courses/{courseId}/remove/{trainerId}

Trainers
Get All Trainers: GET /api/trainers
Get Trainer by ID: GET /api/trainers/{id}
Create Trainer: POST /api/trainers
Update Trainer: PUT /api/trainers/{id}
Delete Trainer: DELETE /api/trainers/{id}
Login: POST /api/auth/login
Register: POST /api/auth/register
Get Course-Trainer Report: GET /api/reports/reports

Payments
Process Payment for Trainer: POST /api/payments/{trainerId}
Get Trainer Payments: GET /api/payments/{trainerId}


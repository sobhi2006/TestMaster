# TestMaster API 🧠

TestMaster API is a fully featured web API built with ASP.NET Core, designed to manage exams, questions, users, and roles. It supports multiple API versions, JWT-based authentication, and OpenAPI documentation. Ideal for educational platforms, assessment systems, or any application requiring structured test management.

## 🚀 Features

- ✅ Question and Question Bank management
- ✅ Exam creation and evaluation
- ✅ User registration, activation, and role-based access
- ✅ API Versioning (v1, v2)
- ✅ JWT Authentication with role and policy support
- ✅ OpenAPI documentation via Swagger
- ✅ FluentValidation for request models
- ✅ Rate limiting and response compression

## 🛠️ Technologies Used

- ASP.NET Core
- Entity Framework Core
- SQL Server
- JWT Authentication
- FluentValidation
- API Versioning
- Swagger / OpenAPI
- Scalar (for advanced documentation)
- Global Exception Handling
- RateLimiting
- In-Memory Caching
- Response Compression (Gzip + Brotli)

## 📦 Getting Started

Clone the repository and run the project:

`bash
git clone https://github.com/sobhi2006/TestMaster
cd TestMaster
dotnet restore
dotnet run

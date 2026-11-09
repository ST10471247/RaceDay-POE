# RaceDay - Event Management System

## Overview
RaceDay is a comprehensive full-stack web-based event management system designed specifically for the South African road running, walking, and cycling community. The platform allows Event Organisers to create and manage events, categories, and participant results, while Participants can browse upcoming events, enter events, track their personal performance history, and prepare for race day using live weather and route information.

## System Roles
### Organiser
- Can create, edit, and delete events
- Manage event categories
- Capture participant results
- View all event enrolments

### Participant
- Create an account
- Browse events
- Enter an event by selecting a category
- View their own enrolments
- Track their personal results

## Technology Stack
- **Backend API:** C# .NET 8, Entity Framework Core, SQL Server
- **Frontend:** ASP.NET Core MVC, Razor Views, Bootstrap 5
- **Authentication:** JWT (JSON Web Tokens)
- **Cloud Storage:** Azure Blob Storage (for event posters)
- **Containerization:** Docker & Docker Compose
- **CI/CD:** GitHub Actions

## Project Structure
- `/docs` - Part 1 Planning (ERD, API Endpoint Plan, SQL Script)
- `/RaceDay.Api` - RESTful API backend
- `/RaceDay.Tests` - Unit tests for the API
- `/RaceDay.MVC` - Frontend web application
- `docker-compose.yml` - Orchestrates the API, MVC app, and SQL Database

## How to Run the Application

### Prerequisites
- Docker Desktop installed and running
- Visual Studio 2022

### Running with Docker (Recommended)
1. Clone the repository.
2. Open a terminal in the root folder (where `docker-compose.yml` is located).
3. Run the following command:
   ```bash
   docker-compose up --build

   Access the web application at: http://localhost:8081

Access the API Swagger documentation at: http://localhost:8080/swagger

Running Locally (Without Docker)
Open RaceDay.sln in Visual Studio.

Set both RaceDay.Api and RaceDay.MVC as startup projects.

Press F5.

CI/CD Pipeline
![CI/CD Build](docs/cicd-screenshot.png)

YouTube Video Presentation
(Replace these with your actual links once you record them)

Part 1: https://youtu.be/DnozwfZOdiA

Part 2: https://youtu.be/899_8_SpNTc

Part 3: https://youtu.be/Ve9zWd9EifQ

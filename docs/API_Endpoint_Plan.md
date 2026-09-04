# RaceDay API Endpoint Plan

## Overview
This document outlines all RESTful API endpoints for the RaceDay event management system. Each endpoint includes the HTTP method, route, description, required role, request body, and expected response.

---

## Authentication Endpoints

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|-------------|-------|-------------|---------------|--------------|-------------------|
| POST | /api/auth/register | Register a new user account | None (Public) | { "email": "user@example.com", "password": "securePass123", "firstName": "John", "lastName": "Doe", "role": "Participant" } | 201 Created - User object with token 400 Bad Request - Validation errors 409 Conflict - Email already exists |
| POST | /api/auth/login | Authenticate a user and return JWT token | None (Public) | { "email": "user@example.com", "password": "securePass123" } | 200 OK - { "token": "jwt_token", "user": { user details } } 401 Unauthorized - Invalid credentials |

---

## User Profile Endpoints

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|-------------|-------|-------------|---------------|--------------|-------------------|
| GET | /api/users/profile | Get current user's profile | Any (Logged in) | None | 200 OK - User profile object 401 Unauthorized - Not logged in |
| PUT | /api/users/profile | Update current user's profile | Any (Logged in) | { "firstName": "John", "lastName": "Smith", "email": "newemail@example.com" } | 200 OK - Updated user profile 400 Bad Request - Validation errors |
| GET | /api/users/{id}/enrolments | Get all enrolments for a specific participant | Participant or Organiser | None | 200 OK - List of enrolments 404 Not Found - User not found |

---

## Event Endpoints

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|-------------|-------|-------------|---------------|--------------|-------------------|
| GET | /api/events | Get all events with optional filtering | None (Public) | None | 200 OK - List of events |
| GET | /api/events/{id} | Get detailed information about a specific event | None (Public) | None | 200 OK - Event details including categories, weather, and route 404 Not Found - Event not found |
| POST | /api/events | Create a new event | Organiser | { "name": "Cape Town Cycle Tour", "description": "Annual cycling event", "date": "2026-03-08", "location": "Cape Town", "routeInfo": "Route details", "maxParticipants": 35000 } | 201 Created - Event object 400 Bad Request - Validation errors |
| PUT | /api/events/{id} | Update an existing event | Organiser | { "name": "Updated Event Name", "description": "Updated description" } | 200 OK - Updated event 404 Not Found - Event not found 403 Forbidden - Not your event |
| DELETE | /api/events/{id} | Soft delete an event (change status to cancelled) | Organiser | None | 204 No Content - Event deleted 404 Not Found - Event not found |

---

## Category Endpoints

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|-------------|-------|-------------|---------------|--------------|-------------------|
| GET | /api/events/{eventId}/categories | Get all categories for a specific event | None (Public) | None | 200 OK - List of categories 404 Not Found - Event not found |
| POST | /api/events/{eventId}/categories | Add a new category to an event | Organiser | { "name": "5km Fun Run", "distance": 5.0, "startTime": "07:00", "maxParticipants": 1000, "fee": 150.00 } | 201 Created - Category object 404 Not Found - Event not found |
| PUT | /api/categories/{id} | Update an existing category | Organiser | { "name": "10km Run", "distance": 10.0, "maxParticipants": 500 } | 200 OK - Updated category 404 Not Found - Category not found |
| DELETE | /api/categories/{id} | Delete a category | Organiser | None | 204 No Content - Category deleted 404 Not Found - Category not found |

---

## Enrolment Endpoints

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|-------------|-------|-------------|---------------|--------------|-------------------|
| GET | /api/enrolments | Get all enrolments (organisers see all, participants see their own) | Any (Logged in) | None | 200 OK - List of enrolments |
| POST | /api/events/{eventId}/enrol | Enrol a participant in an event category | Participant | { "categoryId": 1 } | 201 Created - Enrolment object 400 Bad Request - Event full or already enrolled 404 Not Found - Event not found |
| PUT | /api/enrolments/{id}/cancel | Cancel an enrolment | Participant or Organiser | None | 200 OK - Updated enrolment status 404 Not Found - Enrolment not found |
| GET | /api/events/{eventId}/participants | Get all participants enrolled in an event | Organiser | None | 200 OK - List of participants 404 Not Found - Event not found |

---

## Results Endpoints

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|-------------|-------|-------------|---------------|--------------|-------------------|
| GET | /api/events/{eventId}/results | Get results for a specific event (leaderboard) | None (Public) | None | 200 OK - List of results sorted by position 404 Not Found - Event not found |
| GET | /api/participants/{id}/results | Get all results for a specific participant | Participant or Organiser | None | 200 OK - List of results 404 Not Found - Participant not found |
| POST | /api/events/{eventId}/results | Capture results for participants in an event | Organiser | { "results": [ { "enrolmentId": 1, "finishTime": "01:30:45", "position": 1 } ] } | 201 Created - List of results 400 Bad Request - Validation errors 404 Not Found - Event not found |

---

## Weather Endpoints

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|-------------|-------|-------------|---------------|--------------|-------------------|
| GET | /api/events/{eventId}/weather | Get weather forecast for an event | None (Public) | None | 200 OK - Weather object 404 Not Found - Event not found |
| PUT | /api/events/{eventId}/weather | Update weather forecast for an event | Organiser | { "temperature": 22, "conditions": "Sunny", "windSpeed": 10, "humidity": 65 } | 200 OK - Updated weather 404 Not Found - Event not found |

---

## Route Information Endpoints

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|-------------|-------|-------------|---------------|--------------|-------------------|
| GET | /api/events/{eventId}/route | Get route information for an event | None (Public) | None | 200 OK - Route information 404 Not Found - Event not found |
| PUT | /api/events/{eventId}/route | Update route information for an event | Organiser | { "description": "Updated route", "distance": 42.2, "elevationGain": 500, "waypoints": "Waypoints here", "mapUrl": "http://map.url" } | 200 OK - Updated route 404 Not Found - Event not found |

---

## Summary of Endpoints by Category

| Category | Total Endpoints |
|----------|-----------------|
| Authentication | 2 |
| User Profile | 3 |
| Events | 5 |
| Categories | 4 |
| Enrolments | 4 |
| Results | 3 |
| Weather | 2 |
| Route Information | 2 |
| **Total** | **25** |
# BookClub Web App &nbsp;&nbsp; [![Build & Integration Test](https://github.com/alasdairhendry/BookClub/actions/workflows/build-and-integration-test.yml/badge.svg?branch=main)](https://github.com/alasdairhendry/BookClub/actions/workflows/build-and-integration-test.yml)

A modern platform for creating and joining book clubs online.  
Members can form private clubs with friends, assign books, and participate in discussions around their current read.

This project is designed as a **learning and portfolio piece** for brushing up on:
- ASP.NET Core Web API development
- Database design with EF Core
- Authentication & Authorization with JWT
- Caching strategies (in-memory & Redis)
- DevOps practices (Docker, CI/CD)

--- 

## Project Goals

1. **Practice Real-World Patterns**  
   Implement clean, layered architecture as industry standard.

2. **API-First Design**  
   Expose RESTful endpoints that could later support a web or mobile frontend.

3. **External Data Enrichment**  
   Integrate with the [OpenLibrary API](https://openlibrary.org/developers/api) for book metadata.

4. **Scalable Foundation**  
   Start small with private book clubs, but allow for future expansion into open communities and real-time chat.

---

## Tech Stack

**Backend**
- [ASP.NET Core 8.0](https://learn.microsoft.com/aspnet/core) — Web API
- [Entity Framework Core](https://learn.microsoft.com/ef/core) — ORM
- [ASP.NET Core Identity](https://learn.microsoft.com/en-gb/aspnet/core/security/authentication/identity?view=aspnetcore-9.0&tabs=visual-studio) — Authentication/Authorisation
- [JWT Bearer Authentication](https://jwt.io/) — Secure API access

**Database**
- PostgreSQL (primary)
- EF Core Migrations for schema management

**Caching**
- In-Memory cache for development
- Redis for distributed caching in production

**DevOps**
- Docker & Docker Compose (API + DB + Redis)
- GitHub Actions (CI/CD pipeline: build, test, lint)

**Testing**
- xUnit (unit + integration tests)
- FluentAssertions for readable assertions

---

## Architecture

This project follows a **Clean / Onion Architecture** approach:

src/BookClub.Api -> ASP.NET Core Web API (controllers, auth middleware)\
src/BookClub.Application -> Business logic (use cases, services, DTOs)\
src/BookClub.Domain -> Entities, enums, value objects, business rules\
src/BookClub.Infrastructure -> EF Core, repositories, caching, external APIs\

tests/BookClub.UnitTests\
tests/BookClub.IntegrationTests\

**Domain** → Pure business logic, framework-agnostic\
**Application** → Use cases, DTOs, service interfaces\
**Infrastructure** → EF Core, external services, Redis, implementations\
**Api** → Thin HTTP layer with controllers & OpenApi\

---

## Authentication Flow

- Users register/login with email + password
- On login:
  - **Access Token (JWT)** — 15 min expiry
  - **Refresh Token** — 7 day expiry, stored in DB
- API endpoints require `Authorization: Bearer <token>`
- Refresh token endpoint issues new access/refresh tokens

---

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/)
- [PostgreSQL](https://www.postgresql.org/) (local or via Docker)

### Run Locally
```bash
# Clone the repository
git clone https://github.com/your-username/book-club-platform.git
cd book-club-platform

# Run via Docker Compose
docker-compose up --build
```

API will be available at: http://localhost:5000 \
Swagger UI:               http://localhost:5000/swagger

## Roadmap
### MVP
- User registration & login
- Create & join private clubs
- Assign books to clubs
- Club-level discussion threads
- Basic book data via OpenLibrary API

### Future Enhancements
- Open/public communities
- Chapter-by-chapter discussions
- Real-time chat (SignalR)
- Hardcover.app API integration
- Notifications

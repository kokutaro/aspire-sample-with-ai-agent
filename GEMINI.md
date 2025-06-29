# Gemini Project Configuration

This document provides an overview of the project and guidelines for development.

## Project Overview

This project is a web application built using the .NET Aspire framework. It consists of a .NET backend and a React-based frontend. The architecture follows the principles of Clean Architecture, separating concerns into distinct projects for domain logic, application services, infrastructure, and presentation.

## Technology Stack

* **Backend**:
  * .NET 9
  * ASP.NET Core
  * .NET Aspire
  * Entity Framework Core
* **Frontend**:
  * React
  * TypeScript
  * Vite

## Project Structure

The solution is organized into the following main projects:

* `MyAspireApp.AppHost`: The .NET Aspire application host for orchestrating the different services.
* `MyAspireApp.ApiService`: The backend API service that exposes endpoints to the frontend.
* `MyAspireApp.Web`: A web project, possibly for server-side rendering or another web-related concern.
* `MyAspireApp.Application`: Contains application-specific logic and use cases.
* `MyAspireApp.Domain`: Core domain models, entities, and business rules.
* `MyAspireApp.Infrastructure`: Handles data persistence, external services, and other infrastructure concerns.
* `myaspireapp-frontend`: The React frontend application.

## Development Guidelines

Please adhere to the guidelines documented in the following files for consistent and high-quality development.

* **General AI Development Rules**:
  * [Coding Standards](./ai-rules/coding-standards.md)
  * [Development Process Guide](./ai-rules/development-process-guide.md)
  * [Pull Request Guide](./ai-rules/pr-guide.md)
  * [Tech Stack Guide](./ai-rules/tech-stack-guide.md)
  * [Testing & QA Guide](./ai-rules/testing-qa-guide.md)
* **Project-Specific Coding Guidelines**:
  * [Domain Layer Coding Guideline](./docs/domain/coding-guideline.md)
  * [Infrastructure Layer Coding Guideline](./docs/infrastructure/coding-guideline.md)

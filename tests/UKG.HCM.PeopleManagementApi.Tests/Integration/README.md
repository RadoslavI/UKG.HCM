# Integration Tests

This directory contains integration tests for the UKG.HCM.PeopleManagementApi project.

## Overview

Integration tests validate that different components of the application work together correctly. Unlike unit tests which isolate and test individual components, integration tests assess the interactions between components and systems.

## Test Structure

1. **TestWebApplicationFactory.cs**: Configures a test web server using ASP.NET Core's testing infrastructure, providing an in-memory database and mocked external dependencies.

2. **PeopleControllerIntegrationTests.cs**: Tests the full HTTP request pipeline for the PeopleController, verifying that API endpoints function correctly from HTTP request to database operation.

3. **Helpers**: Contains utilities for database seeding and other test support functions.

## Running the Tests

The integration tests can be run using the standard NUnit test runner in your IDE or via the command line:

```shell
dotnet test

## PRD: Comprehensive Unit \& Integration Testing for Backend (C\# API)

### 1. Overview

This feature introduces **automated unit and integration testing** across all controllers, services, and repositories in the backend API to ensure functionality, correctness, error handling, and data integrity.

The project uses **xUnit** as the primary testing framework, **Moq** for mocking dependencies, and **Testcontainers or a real DB connection (via connection string in `appsettings.json`)** for integration tests.

Every public endpoint, service, or repository method must have test coverage including **successful flows and expected failure scenarios (validation errors, unauthorized, forbidden, not found, exception handling, etc.)**.

***

### 2. Scope

#### In Scope

- Automated tests for all controller endpoints (AuthController, OtpController, UserController, DocumentController).
- Tests for underlying service and repository layers that get invoked by these controllers.
- Verification of custom error handling and `ApiResponse<T>` output structure.
- Integration tests using the actual configured connection string (via `appsettings.json` or environment variable injection).
- Continuous Test Execution (CTE) as part of CI/CD pipeline.


#### Out of Scope

- Frontend or UI testing.
- Performance and load testing (to be added later).
- External API mocks unless dependencies cannot run locally.

***

### 3. Objectives \& Success Metrics

| Objective | Metric of Success |
| :-- | :-- |
| Verify every controller endpoint | 100% endpoint coverage |
| Validate service and repository logic | ≥90% method coverage |
| Ensure consistent error handling | All defined custom exceptions covered |
| Confirm data consistency | Real DB integration passes all CRUD tests |
| Prevent regressions | All tests automatically run in CI pipeline |


***

### 4. Test Architecture

#### 4.1 Testing Frameworks

- **xUnit** for test orchestration.
- **Moq** for mocking service/repository dependencies in unit tests.
- **FluentAssertions** for expressive assertions.
- **Microsoft.AspNetCore.Mvc.Testing** for integration test hosting.
- **Testcontainers** or **SQL Test Database** for isolated DB tests (use appsettings.json real connection for dev snapshot).


#### 4.2 Project Layout

```
/Tests
  /Unit
    - AuthControllerTests.cs
    - OtpControllerTests.cs
    - UserControllerTests.cs
    - DocumentControllerTests.cs
    - UserServiceTests.cs
    - DocumentServiceTests.cs
    ...
  /Integration
    - AuthEndpointsTests.cs
    - OtpEndpointsTests.cs
    - UserEndpointsTests.cs
    - DocumentEndpointsTests.cs
    - DatabaseIntegrationTests.cs
```


***

### 5. Test Coverage Requirements

#### 5.1 Controllers

Each controller must have tests for:

- **Happy Paths**
    - Successful request with expected valid input.
    - Asserts correct HTTP status (usually 200 OK) and valid payload via `ApiResponse<T>`.
- **Non-Happy Paths**
    - Unauthorized access (`UNAUTHORIZED_ACCESS` response, status 401).
    - Forbidden access (`FORBIDDEN_ACCESS`, status 403).
    - Invalid credentials or bad input body (expect 400 or 401).
    - Resource not found (expect 404).
    - Unexpected server errors (exception simulation, expect 500).
- **Validation Cases**
    - Missing required parameters, null bodies, invalid query values.
- **Integration Scenarios**
    - Full request to endpoint invoking the actual DB and verifying persisted state.


#### Controller Example Matrix (UserController)

| Method | Happy Path | No-Happy Paths |
| :-- | :-- | :-- |
| GET /api/user/search | Find matching users | Empty query, exceeds limit |
| GET /api/user/me | Valid JWT returns profile | No token → 401 |
| PUT /api/user/update | Valid JSON updates data | Missing body, invalid field → 400/500 |


***

#### 5.2 Services

Test Categories:

- Input validation and exceptions (ArgumentNull, Format, etc.).
- Repository call correctness and output model verification.
- Exception propagation and logging for database or permission errors.
- Edge cases (empty lists, null DTOs, failed inserts/updates).

***

#### 5.3 Repositories

- CRUD consistency with DB.
- Transaction rollback behavior.
- Error handling when DB connection fails or constraints break.
- Integration against schema constraints (foreign keys, not null, etc.).

***

### 6. Database Configuration

All integration tests use **the same connection string** from `appsettings.json` under the section `"ConnectionStrings:DefaultConnection"`.

Tests must:

1. Create and migrate a temporary copy of the schema.
2. Clean data between tests (using `Database.EnsureDeleted()` or transactional rollback).
3. Support preloading of test fixtures (users, documents, OTPs).

***

### 7. Error Handling Validation

All errors must match the format:

```json
{
  "Success": false,
  "Message": "User authentication required.",
  "ErrorCode": "AUTH_001",
  "StatusCode": 401,
  "UserFriendlyMessage": "You must be logged in to access this resource."
}
```

For every controller, trigger and assert at least one case per defined error code:

- AUTH_001 Unauthorized
- AUTH_002 Forbidden
- AUTH_003 Invalid Credentials

***

### 8. CI/CD Integration

- Configure **GitHub Actions** or equivalent pipeline to:
    - Run unit tests on push and PR.
    - Run integration tests nightly using the same DB connection configured for testing.
- Coverage report uploaded to **Coverlet** and **ReportGenerator** for visualization.

***

### 9. Acceptance Criteria

- [ ] Each controller has unit tests covering all endpoints and response codes.
- [ ] Each service and repository method has tests validating behavior and failures.
- [ ] All tests pass locally and in CI environment.
- [ ] Application hits ≥90% coverage (excluding startup and DI code).
- [ ] Test reports integrated into CI/CD dashboard.

***

Would you like the PRD extended with **example xUnit test class templates** (e.g., `UserControllerTests.cs` skeleton with mocking and arrangement examples)?


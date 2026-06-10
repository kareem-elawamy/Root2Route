# Automated Testing Suite - Walkthrough

The project now has a robust testing foundation covering both Backend and Frontend logic.

## 🚀 Overview
All tests are successfully passing! We've achieved **100% pass rate**:
- **Backend:** 21 / 21 tests passing (xUnit + Moq)
- **Frontend:** 29 / 29 tests passing (Vitest + Angular TestBed)

## 🛠 Backend Tests
We focused on the core CQRS Command Handlers and Services in the `Tests` project:
- **OrganizationRoleServiceTests** & **OrganizationRoleCommandHandlerTests**: Validates duplicate role prevention, soft deletes, and update mappings.
- **OrganizationMemberServiceTests** & **OrganizationMemberCommandHandlerTests**: Validates cross-tenant block logic and role assignment functionality.
- We used `MockQueryable.Moq` to effectively mock `IQueryable` operations like `.AnyAsync()` against Entity Framework Core.

## 🌐 Frontend Tests
We implemented unit tests using **Vitest** for incredible speed alongside Angular's `TestBed`:
- **AuthService**: Validates JWT token parsing and permission logic (e.g., verifying `Owner`/`Admin` access vs specific claims).
- **OrgContextService**: Validates organization switching and `localStorage` syncing.
- **ToastService**: Used Vitest's `vi.useFakeTimers()` to ensure Toasts are queued correctly and automatically dismissed after their specified duration.
- **MembersService**: Used `HttpTestingController` to validate proper endpoints and payloads are sent.
- **ChatComponent**: Validated smart polling logic, DOM injection, message processing, and regex validation blocking emails and phone numbers from being sent.

## 💡 Running the Tests
To run these tests in the future:

**Backend:**
```bash
cd Root2Route-Backend\Tests
dotnet test
```

**Frontend:**
```bash
cd Root2Route-Frontend\FrontEnd
npx ng test --watch=false
```

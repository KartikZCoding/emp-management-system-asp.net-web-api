# 📘 Software Testing — Complete Beginner's Guide

## A Comprehensive Guide for Beginners — From Zero to Testing Your APIs & Projects

> **Project Context:** Employee Management System (ASP.NET Web API)

---

## 📌 Table of Contents

### Part I — Fundamentals & Types
1. [What is Software Testing?](#1-what-is-software-testing)
2. [Why Do We Test?](#2-why-do-we-test)
3. [When Do We Test?](#3-when-do-we-test)
4. [Where Do We Test?](#4-where-do-we-test)
5. [Who Does Testing?](#5-who-does-testing)
6. [The Testing Pyramid](#6-the-testing-pyramid)
7. [Types of Testing — Full Breakdown](#7-types-of-testing--full-breakdown)
8. [Manual vs Automated Testing](#8-manual-vs-automated-testing)
9. [Black-Box vs White-Box vs Grey-Box](#9-black-box-vs-white-box-vs-grey-box)
10. [Key Terminology Glossary](#10-key-terminology-glossary)

### Part II — API Testing Deep Dive
11. [What is an API?](#11-what-is-an-api)
12. [What is API Testing?](#12-what-is-api-testing)
13. [HTTP Fundamentals for Testers](#13-http-fundamentals-for-testers)
14. [HTTP Status Codes](#14-http-status-codes)
15. [What to Test in an API](#15-what-to-test-in-an-api)
16. [API Testing with Postman](#16-api-testing-with-postman)
17. [API Testing with Swagger](#17-api-testing-with-swagger)
18. [Authentication & Authorization Testing](#18-authentication--authorization-testing)
19. [Automated API Testing in .NET](#19-automated-api-testing-in-net)
20. [Testing Patterns & Best Practices](#20-testing-patterns--best-practices)
21. [Common API Bugs to Watch For](#21-common-api-bugs-to-watch-for)

### Part III — Hands-On Implementation
22. [Setting Up the Test Project](#22-setting-up-the-test-project)
23. [Understanding Mocking](#23-understanding-mocking)
24. [Writing Unit Tests for EmployeeService](#24-writing-unit-tests-for-employeeservice)
25. [Writing Integration Tests for API Endpoints](#25-writing-integration-tests-for-api-endpoints)
26. [Testing Auth Flow](#26-testing-auth-flow)
27. [Code Coverage](#27-code-coverage)
28. [Test-Driven Development (TDD) Walkthrough](#28-test-driven-development-tdd-walkthrough)
29. [CI/CD — Automated Testing with GitHub Actions](#29-cicd--automated-testing-with-github-actions)
30. [Testing Tools Comparison](#30-testing-tools-comparison)
31. [Complete Test Checklist for EMS](#31-complete-test-checklist-for-ems)
32. [Learning Path](#32-learning-path)

---

## 1. What is Software Testing?

### Definition

> **Software Testing** is the process of checking whether your application works correctly, meets the requirements, and is free of bugs before it reaches the end users.

### Real-Life Analogy 🏠

Think of **building a house**:

| House Building Step | Software Equivalent |
|---|---|
| Checking if each brick is strong | **Unit Testing** — testing each small piece |
| Checking if walls + roof fit together | **Integration Testing** — testing pieces together |
| Living in the house for a day to check everything | **System Testing** — testing the whole app |
| The house owner inspects before moving in | **Acceptance Testing** — client approval |

### In Simple Words

Before you give your app to users, you want to make sure:
- ✅ Every button works
- ✅ Every API returns correct data
- ✅ Wrong inputs are handled gracefully
- ✅ The system doesn't crash under heavy use
- ✅ No one can hack into it

---

## 2. Why Do We Test?

### The Cost of NOT Testing

| When Bug is Found | Cost to Fix |
|---|---|
| During development | ₹1 (cheapest) |
| During testing | ₹10 |
| After deployment | ₹100 |
| After users find it | ₹1000+ (reputation damage) |

### Real-World Disaster Examples

1. **Knight Capital (2012)**: A software bug caused a trading firm to lose **$440 million in 45 minutes**. Proper testing would have caught it.
2. **Healthcare.gov (2013)**: The US healthcare website crashed on launch day because it wasn't load-tested for millions of users.

### Key Reasons to Test

| Reason | Explanation |
|---|---|
| **Find Bugs Early** | Catch problems before users do |
| **Save Money** | Fixing bugs early is 100x cheaper |
| **Build Confidence** | Know your code works before deploying |
| **Prevent Regressions** | New features don't break old ones |
| **Documentation** | Tests describe how code should behave |
| **Better Design** | Writing testable code forces cleaner architecture |

### From Your EMS Project Perspective

Imagine you add a new feature to the `SalaryService` and it accidentally breaks the `LeaveService`. Without tests, you won't know until a user complains: *"My leave balance is wrong!"*. With tests, the moment you break something, the test fails immediately.

---

## 3. When Do We Test?

Testing happens at **every stage** of development:

```
📋 Requirements → 🎨 Design → 💻 Coding → 🧪 Testing → 🚀 Deploy → 🔄 Maintain
      ↑               ↑            ↑            ↑            ↑           ↑
   Review          Review     Unit Tests    Full Testing  Smoke Tests  Monitoring
   requirements    design     while coding  phase         in prod      & alerts
```

### The "Shift Left" Principle

> Test **as early as possible**. The earlier you find a bug, the cheaper it is to fix.

| Phase | What We Test |
|---|---|
| **During Coding** | Write unit tests alongside your code (TDD) |
| **After Feature Complete** | Integration tests to check components work together |
| **Before Release** | Full system testing, performance testing |
| **After Deployment** | Smoke tests to verify nothing broke in production |
| **Continuously** | Automated tests run on every git push (CI/CD) |

---

## 4. Where Do We Test?

### Testing Environments

```
┌─────────────┐    ┌──────────────┐    ┌──────────────┐    ┌──────────────┐
│   DEV ENV    │───▶│   TEST ENV    │───▶│ STAGING ENV   │───▶│  PRODUCTION   │
│  (Your PC)   │    │  (QA Team)    │    │ (Pre-release) │    │  (Live App)   │
└─────────────┘    └──────────────┘    └──────────────┘    └──────────────┘
   Developer         QA Testers         Final check          Real users
   tests here        test here          before release       use this
```

| Environment | Purpose | Who Uses It |
|---|---|---|
| **Development (Local)** | Run unit tests, debug | Developers |
| **Testing / QA** | Run all automated tests | QA Team |
| **Staging** | Mirror of production, final validation | Dev + QA + Product |
| **Production** | Live application, smoke tests only | End Users |

---

## 5. Who Does Testing?

| Role | What They Test | Tools They Use |
|---|---|---|
| **Developer** | Unit tests, integration tests | xUnit, NUnit, Moq |
| **QA Engineer** | System tests, regression tests | Selenium, Postman |
| **DevOps Engineer** | CI/CD pipelines, smoke tests | GitHub Actions, Jenkins |
| **Security Tester** | Vulnerabilities, pen testing | OWASP ZAP, Burp Suite |
| **Product Owner** | Acceptance testing (UAT) | Manual testing |
| **End Users** | Beta testing | The actual application |

---

## 6. The Testing Pyramid

The **Testing Pyramid** tells you how many of each type of test to write:

```
                    ╱╲
                   ╱  ╲
                  ╱ UI ╲            ← Few (slow, expensive, brittle)
                 ╱ Tests╲
                ╱────────╲
               ╱Integration╲        ← Some (moderate speed & cost)
              ╱    Tests     ╲
             ╱────────────────╲
            ╱   Unit  Tests    ╲     ← Many (fast, cheap, reliable)
           ╱____________________╲
```

| Level | Quantity | Speed | Cost | Example |
|---|---|---|---|---|
| **Unit Tests** | 70-80% | ⚡ Very Fast | 💰 Cheap | Test if `GetEmployeeByIdAsync` throws when ID ≤ 0 |
| **Integration Tests** | 15-20% | 🐢 Moderate | 💰💰 Medium | Test Controller → Service → Database flow |
| **UI/E2E Tests** | 5-10% | 🐌 Slow | 💰💰💰 Expensive | Test full login → create employee → verify in list |

---

## 7. Types of Testing — Full Breakdown

### 7.1 Unit Testing

> **Unit Testing** tests the **smallest piece** of code (a function, a method) **in isolation** — without databases, APIs, or file systems.

**Analogy**: Testing a single car engine part on a workbench, not in the full car.

| Property | Description |
|---|---|
| **Scope** | Single method or function |
| **Speed** | Extremely fast (milliseconds) |
| **Dependencies** | All mocked/faked (no real DB) |
| **Who Writes** | Developers |
| **Framework** | xUnit, NUnit, MSTest (for .NET) |

**EMS Example:**

```csharp
// Test that invalid ID throws BadRequestException
[Fact]
public async Task GetEmployeeById_WithInvalidId_ThrowsBadRequestException()
{
    // ARRANGE — set up fake objects
    var mockRepo = new Mock<IEmployeeRepository>();
    var service = new EmployeeService(mockRepo.Object, ...);

    // ACT & ASSERT — call method, check it throws
    await Assert.ThrowsAsync<BadRequestException>(
        () => service.GetEmployeeByIdAsync(-1)
    );
}
```

**The AAA Pattern**: Every unit test follows **Arrange → Act → Assert**.

| ✅ Test This | ❌ Don't Test This |
|---|---|
| Business logic & validations | Framework code (ASP.NET itself) |
| Edge cases (null, empty, negative) | Simple getters/setters |
| Error handling (exceptions) | Third-party libraries |

---

### 7.2 Integration Testing

> **Integration Testing** checks if **multiple components work together** — e.g., Controller + Service + Database.

**Analogy**: You tested engine, brakes, and steering separately. Now put them in the car and drive.

**EMS Example:**

```csharp
public class EmployeeIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public EmployeeIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Use In-Memory DB instead of real SQL Server
                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetAllEmployees_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/employees");
        response.EnsureSuccessStatusCode();
    }
}
```

| Aspect | Unit Test | Integration Test |
|---|---|---|
| **Speed** | Milliseconds | Seconds |
| **Database** | Mocked (fake) | Real or In-Memory |
| **What it proves** | "This method works" | "These components work together" |

---

### 7.3 System Testing

> **System Testing** tests the **entire application end-to-end** as a complete system.

**Analogy**: You built the house. Now live in it for a week checking everything together.

```
Test: "Employee Lifecycle"
Step 1: Admin logs in                          → Auth works ✅
Step 2: Admin creates a department             → Department API works ✅
Step 3: Admin creates an employee              → Employee API works ✅
Step 4: Employee applies for leave             → Leave API works ✅
Step 5: Manager approves the leave             → Approval flow works ✅
Step 6: Generate salary with leave deductions  → Salary calculation works ✅
```

---

### 7.4 Acceptance Testing (UAT)

> **UAT** is when the **actual client/end-user** tests the software to verify it meets their business requirements.

**Analogy**: You ordered a custom cake. Before paying, you taste it to make sure it's what you wanted.

UAT answers: **"Did we build the RIGHT thing?"** (not just "did we build it right?")

---

### 7.5 Regression Testing

> **Regression Testing** means re-running existing tests after changes to ensure new code **didn't break old features**.

**Analogy**: You fix a leaking pipe in the kitchen. After fixing, you check **all other pipes** too.

```
Before Change:  50 tests passing ✅
Make Change:    Modify SalaryService.cs
Run All Tests:  49 passing ✅, 1 failing ❌  ← Regression detected!
```

---

### 7.6 Smoke Testing

> **Smoke Testing** is a quick check if the **most critical features** work after a new deployment.

**Analogy**: Plug in a new device — does it turn on? Does the screen light up?

```
Smoke Test Checklist (5 minutes):
✅ App starts without crashing?
✅ Can a user log in?
✅ Does /api/employees return data?
✅ Database connection works?
```

---

### 7.7 Sanity Testing

> **Sanity Testing** is a focused test on a **specific feature** that was recently changed.

| Aspect | Smoke Testing | Sanity Testing |
|---|---|---|
| **Scope** | Broad (all critical features) | Narrow (one specific area) |
| **When** | After every new build | After a specific bug fix |
| **Purpose** | "Is the build stable?" | "Does this fix work?" |

---

### 7.8 Performance Testing

> **Performance Testing** checks how the app behaves under **load, stress, and high traffic**.

| Sub-Type | What It Tests | Analogy |
|---|---|---|
| **Load Testing** | Normal expected traffic | 100 people in elevator rated for 100 |
| **Stress Testing** | Beyond max capacity | 200 people in that elevator |
| **Spike Testing** | Sudden traffic surge | 500 people rushing elevator at once |
| **Endurance Testing** | Performance over long time | Elevator running 24/7 for a month |

**Tools**: k6, Apache JMeter, Artillery (all free)

---

### 7.9 Security Testing

> **Security Testing** finds vulnerabilities attackers could exploit.

| Vulnerability | Description | EMS Example |
|---|---|---|
| **SQL Injection** | SQL code in inputs | `name='; DROP TABLE Employees;--` |
| **Broken Auth** | Bypassing login | Accessing `/api/employees` without JWT |
| **Broken Access Control** | Unauthorized access | Employee accessing admin-only APIs |
| **XSS** | Injecting scripts | `<script>alert('hacked')</script>` in name field |

---

## 8. Manual vs Automated Testing

| Aspect | Manual Testing | Automated Testing |
|---|---|---|
| **Done By** | Human tester | Code/scripts |
| **Speed** | Slow | Fast |
| **Cost (long-term)** | Expensive | Cheaper |
| **Best For** | Exploratory, UX testing | Regression, repetitive tests |

```
Use MANUAL when → Exploring new features, testing visual design
Use AUTOMATED when → Running same tests repeatedly, CI/CD, regression
```

---

## 9. Black-Box vs White-Box vs Grey-Box

| Approach | Knowledge of Code | Who Does It | Example |
|---|---|---|---|
| **Black-Box** | None | QA Testers | "Send request, check response" |
| **White-Box** | Full | Developers | "Test every if/else branch" |
| **Grey-Box** | Partial | Dev + QA | "Know DB schema, test edge cases" |

---

## 10. Key Terminology Glossary

| Term | Definition |
|---|---|
| **Test Case** | A specific scenario you test |
| **Test Suite** | A collection of related test cases |
| **Mock** | A fake object simulating real behavior |
| **Stub** | A simplified fake with pre-set responses |
| **Assertion** | A check that verifies expected outcome |
| **Code Coverage** | % of code executed by tests |
| **TDD** | Test-Driven Development — write tests FIRST |
| **CI/CD** | Tests run automatically on every git push |
| **Flaky Test** | Test that sometimes passes, sometimes fails |
| **SUT** | System Under Test — the thing you're testing |
| **Happy Path** | Normal expected flow (valid login) |
| **Sad Path** | Error/failure flow (wrong password) |
| **Edge Case** | Unusual input (null, 0, max int) |
| **Red-Green-Refactor** | TDD cycle: ❌ Fail → ✅ Pass → 🔄 Clean up |

---

---

# Part II — API Testing Deep Dive

---

## 11. What is an API?

### Definition

> **API (Application Programming Interface)** is a set of rules that allows one software to talk to another. It's like a **waiter in a restaurant** — you (the client) tell the waiter (API) what you want, and the waiter brings it from the kitchen (server/database).

### Real-Life Analogy 🍽️

```
YOU (Client/Frontend)
    │
    │  "I want employee #5's details"
    │  (HTTP GET /api/employees/5)
    ▼
🤵 WAITER (API)
    │
    │  Goes to the kitchen
    ▼
👨‍🍳 KITCHEN (Server + Database)
    │
    │  Prepares the data
    ▼
🤵 WAITER brings back the response
    │
    │  { "id": 5, "name": "Kartik", "dept": "Engineering" }
    ▼
YOU see the result
```

### Your EMS Project's APIs

Your project has **15 controllers** with many API endpoints:

| Controller | What It Does | Example Endpoint |
|---|---|---|
| `EmployeesController` | CRUD for employees | `GET /api/employees` |
| `AuthController` | Login, register, tokens | `POST /api/auth/login` |
| `LeaveController` | Leave management | `POST /api/leave/apply` |
| `SalaryController` | Salary operations | `GET /api/salary/{empId}` |
| `AttendanceController` | Check-in/check-out | `POST /api/attendance/checkin` |
| `DashboardController` | Statistics | `GET /api/dashboard/stats` |

---

## 12. What is API Testing?

### Definition

> **API Testing** is sending requests to your API endpoints and verifying the responses — without any UI. You directly test the "brain" of your application.

### Why Test APIs Directly?

| Reason | Explanation |
|---|---|
| **Faster than UI testing** | No browser needed, just HTTP requests |
| **Catches bugs earlier** | Test backend logic before building frontend |
| **More reliable** | Not affected by CSS changes or UI redesigns |
| **Covers more scenarios** | Easy to test edge cases, invalid data, auth |

### What Does API Testing Look Like?

```
Step 1: Send a REQUEST
   POST /api/employees
   Body: { "firstName": "Kartik", "email": "kartik@company.com", ... }

Step 2: Check the RESPONSE
   Status Code: 201 Created    ← ✅ Correct status?
   Body: { "status": true, "message": "Employee created successfully" }
   Headers: Content-Type: application/json  ← ✅ Correct format?

Step 3: VERIFY side effects
   GET /api/employees/1        ← ✅ Does the employee exist now?
```

---

## 13. HTTP Fundamentals for Testers

### HTTP Methods (Verbs)

Every API request uses an **HTTP method** that tells the server what action to perform:

| Method | Purpose | CRUD Operation | EMS Example |
|---|---|---|---|
| **GET** | Read/Retrieve data | **R**ead | `GET /api/employees` → Get all employees |
| **POST** | Create new data | **C**reate | `POST /api/employees` → Create new employee |
| **PUT** | Update existing data (full) | **U**pdate | `PUT /api/employees/5` → Update employee 5 |
| **PATCH** | Update partial data | **U**pdate | `PATCH /api/employees/5` → Update only name |
| **DELETE** | Remove data | **D**elete | `DELETE /api/employees/5` → Delete employee 5 |

### Anatomy of an HTTP Request

```
┌─────────────────────────────────────────────────────────────┐
│  POST /api/employees HTTP/1.1                  ← METHOD + URL
│                                                             │
│  HEADERS:                                                   │
│  ─────────                                                  │
│  Content-Type: application/json               ← Body format │
│  Authorization: Bearer eyJhbGciOi...          ← JWT Token   │
│  Accept: application/json                     ← Expected format│
│                                                             │
│  BODY:                                                      │
│  ──────                                                     │
│  {                                                          │
│    "firstName": "Kartik",                                   │
│    "lastName": "Zaveri",                                    │
│    "email": "kartik@company.com",                           │
│    "departmentId": 1,                                       │
│    "designationId": 2                                       │
│  }                                                          │
└─────────────────────────────────────────────────────────────┘
```

### Anatomy of an HTTP Response

```
┌─────────────────────────────────────────────────────────────┐
│  HTTP/1.1 201 Created                          ← STATUS CODE│
│                                                             │
│  HEADERS:                                                   │
│  ─────────                                                  │
│  Content-Type: application/json                             │
│  Location: /api/employees/6                                 │
│                                                             │
│  BODY:                                                      │
│  ──────                                                     │
│  {                                                          │
│    "status": true,                                          │
│    "statusCode": 201,                                       │
│    "message": "Employee created successfully"               │
│  }                                                          │
└─────────────────────────────────────────────────────────────┘
```

---

## 14. HTTP Status Codes

Status codes tell you **what happened** with your request. They are grouped into 5 categories:

### Category Overview

| Range | Category | Meaning | Emoji |
|---|---|---|---|
| **1xx** | Informational | "Hold on, processing..." | ⏳ |
| **2xx** | Success | "Everything worked!" | ✅ |
| **3xx** | Redirection | "Go somewhere else" | ↩️ |
| **4xx** | Client Error | "YOU made a mistake" | ❌ |
| **5xx** | Server Error | "WE (server) messed up" | 💥 |

### Most Important Status Codes for API Testing

| Code | Name | When It's Returned | EMS Example |
|---|---|---|---|
| **200** | OK | Request succeeded | `GET /api/employees/1` returns employee data |
| **201** | Created | New resource created | `POST /api/employees` creates new employee |
| **204** | No Content | Success but no body | `DELETE /api/employees/5` (deleted, nothing to return) |
| **400** | Bad Request | Invalid input data | Missing required field in employee creation |
| **401** | Unauthorized | No/invalid authentication | Calling API without JWT token |
| **403** | Forbidden | Authenticated but no permission | Employee trying to delete another employee |
| **404** | Not Found | Resource doesn't exist | `GET /api/employees/99999` (no such employee) |
| **409** | Conflict | Duplicate/conflicting data | Creating employee with email that already exists |
| **422** | Unprocessable Entity | Validation failed | Email format is invalid |
| **500** | Internal Server Error | Server crashed | Unhandled exception in your code |

### How to Remember: The Restaurant Analogy

```
200 OK          = "Here's your food, enjoy!" ✅
201 Created     = "Your food has been prepared!" 🍳
400 Bad Request = "Sorry, we don't serve that dish" 🚫
401 Unauthorized = "Do you have a reservation? Show your ID" 🪪
403 Forbidden    = "VIP section only, you can't enter" 🚷
404 Not Found    = "That dish is not on our menu" 📋
500 Server Error = "The kitchen is on fire!" 🔥
```

---

## 15. What to Test in an API

### The Complete API Testing Checklist

#### A. Functional Tests (Does it work?)

| What to Test | Example Test Case |
|---|---|
| **Happy Path** | Create employee with valid data → 201 Created |
| **Sad Path** | Create employee with missing name → 400 Bad Request |
| **Edge Cases** | Create employee with 255-char name → should work |
| **Boundary Values** | Page=0, Page=-1, Page=999999 |
| **Data Validation** | Invalid email format, future dates, negative salary |
| **Response Format** | JSON structure matches expected schema |
| **Response Data** | Returned data matches what was sent |

#### B. Authentication & Authorization Tests

| What to Test | Example |
|---|---|
| **No token** | Call API without Authorization header → 401 |
| **Invalid token** | Send garbage token → 401 |
| **Expired token** | Send expired JWT → 401 |
| **Wrong role** | Employee calls admin-only endpoint → 403 |
| **Correct role** | Admin calls admin endpoint → 200 |

#### C. Error Handling Tests

| What to Test | Example |
|---|---|
| **Invalid ID format** | `GET /api/employees/abc` → 400 |
| **Non-existent resource** | `GET /api/employees/99999` → 404 |
| **Duplicate data** | Create employee with existing email → 400/409 |
| **Empty body** | `POST /api/employees` with no body → 400 |
| **Server errors** | Error response has proper structure, no stack trace |

#### D. Performance Tests

| What to Test | Example |
|---|---|
| **Response time** | GET /api/employees responds in < 500ms |
| **Payload size** | Response is not unexpectedly large |
| **Pagination** | Large dataset returns paginated results |

---

## 16. API Testing with Postman

### What is Postman?

> **Postman** is a free tool that lets you send HTTP requests to your API and inspect the responses. It's the most popular tool for manual API testing.

### Setting Up Postman for Your EMS Project

#### Step 1: Download & Install

- Go to [postman.com](https://www.postman.com/downloads/)
- Download and install the desktop app

#### Step 2: Create a Collection

A **Collection** is a folder of related API requests:

```
📁 EMS API Tests
├── 📁 Auth
│   ├── POST Login
│   ├── POST Register
│   └── POST Refresh Token
├── 📁 Employees
│   ├── GET All Employees
│   ├── GET Employee by ID
│   ├── POST Create Employee
│   ├── PUT Update Employee
│   └── DELETE Employee
├── 📁 Leave
│   ├── POST Apply Leave
│   ├── PUT Approve Leave
│   └── GET Leave Balance
└── 📁 Salary
    ├── GET Employee Salary
    └── POST Generate Salary
```

#### Step 3: Set Up Environment Variables

```
Variable          Value
─────────────────────────────────────
base_url          https://localhost:5001
auth_token        (auto-filled after login)
employee_id       1
```

#### Step 4: Test Your Login Endpoint

```
Method:  POST
URL:     {{base_url}}/api/auth/login
Headers: Content-Type: application/json
Body:
{
    "email": "admin@company.com",
    "password": "Admin@123"
}
```

**Expected Response:**
```json
{
    "status": true,
    "statusCode": 200,
    "data": {
        "token": "eyJhbGciOiJIUzI1NiIs...",
        "refreshToken": "abc123...",
        "expiration": "2026-04-22T12:00:00Z"
    }
}
```

#### Step 5: Write Postman Test Scripts

In the **Tests** tab, you can write JavaScript to automatically verify responses:

```javascript
// Test 1: Check status code is 200
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

// Test 2: Check response has a token
pm.test("Response contains token", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData.data.token).to.be.a("string");
    pm.expect(jsonData.data.token.length).to.be.above(10);
});

// Test 3: Save token for use in other requests
pm.test("Save auth token", function () {
    var jsonData = pm.response.json();
    pm.environment.set("auth_token", jsonData.data.token);
});

// Test 4: Check response time is acceptable
pm.test("Response time is less than 500ms", function () {
    pm.expect(pm.response.responseTime).to.be.below(500);
});
```

#### Step 6: Test Employee CRUD

**Create Employee:**
```
Method:  POST
URL:     {{base_url}}/api/employees
Headers:
    Content-Type: application/json
    Authorization: Bearer {{auth_token}}
Body:
{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@company.com",
    "departmentId": 1,
    "designationId": 1
}
```

Test Script:
```javascript
pm.test("Employee created successfully", function () {
    pm.response.to.have.status(201);
    var json = pm.response.json();
    pm.expect(json.message).to.eql("Employee created successfully");
});
```

---

## 17. API Testing with Swagger

### What is Swagger?

> **Swagger (OpenAPI)** is a tool built into ASP.NET projects that auto-generates an interactive API documentation page where you can test endpoints directly in the browser.

### How to Use Swagger in Your EMS Project

1. Run your project: `dotnet run`
2. Open browser: `https://localhost:5001/swagger`
3. You'll see all your controllers and endpoints listed
4. Click any endpoint → Click "Try it out" → Fill in parameters → Click "Execute"

### Swagger vs Postman

| Feature | Swagger | Postman |
|---|---|---|
| **Setup Required** | None (built-in) | Download & install |
| **Test Scripts** | ❌ No | ✅ Yes |
| **Save Requests** | ❌ No | ✅ Yes |
| **Environment Variables** | ❌ No | ✅ Yes |
| **Best For** | Quick manual testing | Comprehensive testing |
| **Auto-generated Docs** | ✅ Yes | ❌ No |

> [!TIP]
> **Use Swagger** for quick, one-off testing during development. **Use Postman** for organized, repeatable, scripted testing.

---

## 18. Authentication & Authorization Testing

### Understanding the Difference

| Concept | Question It Answers | Analogy |
|---|---|---|
| **Authentication** | "WHO are you?" | Showing your ID at the door |
| **Authorization** | "WHAT can you do?" | Your ID says VIP, so you can enter VIP area |

### Testing JWT Authentication in Your EMS

#### Test 1: No Token (401 Unauthorized)

```
GET /api/employees
Headers: (no Authorization header)

Expected: 401 Unauthorized
```

#### Test 2: Invalid Token (401 Unauthorized)

```
GET /api/employees
Headers: Authorization: Bearer invalid.token.here

Expected: 401 Unauthorized
```

#### Test 3: Valid Token, No Permission (403 Forbidden)

```
# Login as a regular employee (no Employee.Delete permission)
DELETE /api/employees/5
Headers: Authorization: Bearer <employee_token>

Expected: 403 Forbidden
```

#### Test 4: Valid Token, Has Permission (200 OK)

```
# Login as admin (has Employee.Delete permission)
DELETE /api/employees/5
Headers: Authorization: Bearer <admin_token>

Expected: 200 OK
```

### Your EMS Permission System

Your project uses `[HasPermission("...")]` attribute. Test each permission:

| Endpoint | Required Permission | Test Without | Test With |
|---|---|---|---|
| `GET /api/employees` | Employee.Read | → 403 | → 200 |
| `POST /api/employees` | Employee.Create | → 403 | → 201 |
| `PUT /api/employees/{id}` | Employee.Update | → 403 | → 200 |
| `DELETE /api/employees/{id}` | Employee.Delete | → 403 | → 200 |

---

## 19. Automated API Testing in .NET

### Testing Framework: xUnit + WebApplicationFactory

#### What is `WebApplicationFactory`?

> It creates a **test server** that runs your entire ASP.NET API **in memory**. You can send real HTTP requests to it without starting the actual server.

#### Project Setup

```
EmpMS.sln
├── EmpMS/              (your API project)
├── Application/        (services layer)
├── Domain/             (entities layer)
├── Infrastructure/     (data access layer)
└── EmpMS.Tests/        (NEW - test project)  ← You create this
```

**Create the test project:**
```powershell
dotnet new xunit -n EmpMS.Tests
dotnet sln add EmpMS.Tests
dotnet add EmpMS.Tests reference EmpMS
dotnet add EmpMS.Tests package Microsoft.AspNetCore.Mvc.Testing
dotnet add EmpMS.Tests package Moq
dotnet add EmpMS.Tests package FluentAssertions
```

#### Example: Employee API Integration Tests

```csharp
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class EmployeeApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public EmployeeApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllEmployees_WithoutAuth_Returns401()
    {
        // ACT
        var response = await _client.GetAsync("/api/employees");

        // ASSERT
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateEmployee_WithValidData_Returns201()
    {
        // ARRANGE — login first to get token
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "admin@company.com",
            password = "Admin@123"
        });
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Data.Token);

        // ACT — create employee
        var response = await _client.PostAsJsonAsync("/api/employees", new
        {
            firstName = "Test",
            lastName = "User",
            email = "test.user@company.com",
            departmentId = 1,
            designationId = 1
        });

        // ASSERT
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
```

#### Running Tests

```powershell
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity detailed

# Run specific test class
dotnet test --filter "EmployeeApiTests"

# Run with code coverage
dotnet test --collect:"XPlat Code Coverage"
```

---

## 20. Testing Patterns & Best Practices

### Naming Convention for Tests

Use this format: `MethodName_Scenario_ExpectedResult`

```csharp
// ✅ Good names — clear and descriptive
GetEmployeeById_WithValidId_ReturnsEmployee()
GetEmployeeById_WithNegativeId_ThrowsBadRequestException()
CreateEmployee_WithDuplicateEmail_ThrowsBadRequestException()
Login_WithWrongPassword_Returns401()

// ❌ Bad names — vague and unclear
Test1()
EmployeeTest()
TestCreate()
```

### The FIRST Principles

| Letter | Principle | Meaning |
|---|---|---|
| **F** | Fast | Tests should run in milliseconds |
| **I** | Independent | Tests should not depend on each other |
| **R** | Repeatable | Same result every time you run |
| **S** | Self-validating | Pass or fail automatically (no manual check) |
| **T** | Timely | Write tests close to the time you write code |

### Test Data Best Practices

```csharp
// ❌ BAD — hardcoded data shared across tests
private static readonly string TestEmail = "test@company.com";

// ✅ GOOD — unique data per test
private string GetUniqueEmail() => $"test-{Guid.NewGuid()}@company.com";

// ✅ GOOD — use Builder pattern for complex objects
var employee = new EmployeeBuilder()
    .WithName("Kartik")
    .WithEmail(GetUniqueEmail())
    .WithDepartment(1)
    .Build();
```

### Arrange-Act-Assert (AAA) Pattern

```csharp
[Fact]
public async Task CreateEmployee_WithValidData_ReturnsSuccess()
{
    // ═══════════════════════════════════════
    // ARRANGE — Prepare everything needed
    // ═══════════════════════════════════════
    var mockRepo = new Mock<IEmployeeRepository>();
    mockRepo.Setup(r => r.EmailExistAsync(It.IsAny<string>()))
            .ReturnsAsync(false);  // email doesn't exist yet

    var service = new EmployeeService(mockRepo.Object, ...);

    var dto = new CreateEmployeeDto
    {
        FirstName = "Kartik",
        Email = "kartik@company.com"
    };

    // ═══════════════════════════════════════
    // ACT — Execute the method being tested
    // ═══════════════════════════════════════
    await service.CreateEmployeeAsync(dto);

    // ═══════════════════════════════════════
    // ASSERT — Verify the expected outcome
    // ═══════════════════════════════════════
    mockRepo.Verify(r => r.CreateAsync(It.IsAny<Employee>()), Times.Once);
}
```

---

## 21. Common API Bugs to Watch For

| Bug Category | Description | How to Catch |
|---|---|---|
| **Missing Validation** | API accepts invalid data | Send empty strings, nulls, negative numbers |
| **Wrong Status Code** | Returns 200 when it should return 404 | Check status codes for every scenario |
| **Data Leak** | Password hash returned in response | Inspect response JSON for sensitive fields |
| **Missing Auth** | Endpoint works without token | Call every endpoint without Authorization header |
| **N+1 Query** | API makes too many DB calls | Monitor response time with large datasets |
| **Race Condition** | Two simultaneous requests cause conflict | Send parallel requests to same resource |
| **Incorrect Pagination** | Page 2 shows same data as page 1 | Request multiple pages, verify different data |
| **Missing Error Message** | 400 response with no explanation | Check that error responses include helpful messages |

### Example Bug Hunt in Your EMS

```
Bug: "Employee with same email can be created twice"

Reproduce:
1. POST /api/employees  →  { email: "kartik@company.com" }  →  201 ✅
2. POST /api/employees  →  { email: "kartik@company.com" }  →  201 ❌ BUG!

Expected: Second request should return 400 with "Email already exists"

Fix: Your EmployeeService already checks this:
  if (await _employeeRepository.EmailExistAsync(dto.Email))
      throw new BadRequestException("An employee with this email already exists!");

Test to prevent regression:
[Fact]
public async Task CreateEmployee_DuplicateEmail_ThrowsBadRequest()
{
    // ... setup mock to return true for EmailExistAsync ...
    await Assert.ThrowsAsync<BadRequestException>(
        () => service.CreateEmployeeAsync(duplicateDto));
}
```

---

---

# Part III — Hands-On Implementation with Your EMS Project

---

---

## 22. Setting Up the Test Project

### Step-by-Step Setup

```powershell
# Navigate to your solution directory
cd d:\Projects\emp-management-system-asp.net-web-api\EmpMS

# 1. Create the test project
dotnet new xunit -n EmpMS.Tests

# 2. Add it to your solution
dotnet sln EmpMS.slnx add EmpMS.Tests/EmpMS.Tests.csproj

# 3. Add references to your existing projects
dotnet add EmpMS.Tests reference EmpMS/EmpMS.csproj
dotnet add EmpMS.Tests reference Application/Application.csproj
dotnet add EmpMS.Tests reference Domain/Domain.csproj
dotnet add EmpMS.Tests reference Infrastructure/Infrastructure.csproj

# 4. Install testing packages
dotnet add EmpMS.Tests package Moq                                    # For mocking
dotnet add EmpMS.Tests package FluentAssertions                       # For readable assertions
dotnet add EmpMS.Tests package Microsoft.AspNetCore.Mvc.Testing       # For integration tests
dotnet add EmpMS.Tests package Microsoft.EntityFrameworkCore.InMemory  # For test database
```

### Resulting Project Structure

```
EmpMS/
├── EmpMS.slnx
├── Domain/                 (Entities, Interfaces)
├── Application/            (Services, DTOs)
├── Infrastructure/         (Repositories, DbContext)
├── EmpMS/                  (API Controllers)
└── EmpMS.Tests/            ← NEW
    ├── EmpMS.Tests.csproj
    ├── UnitTests/
    │   ├── Services/
    │   │   ├── EmployeeServiceTests.cs
    │   │   ├── LeaveServiceTests.cs
    │   │   ├── SalaryServiceTests.cs
    │   │   └── AuthServiceTests.cs
    │   └── Validators/
    │       └── EmployeeDtoValidatorTests.cs
    ├── IntegrationTests/
    │   ├── Controllers/
    │   │   ├── EmployeeControllerTests.cs
    │   │   ├── AuthControllerTests.cs
    │   │   └── LeaveControllerTests.cs
    │   └── CustomWebApplicationFactory.cs
    └── Helpers/
        ├── TestDataBuilder.cs
        └── AuthHelper.cs
```

---

## 23. Understanding Mocking

### What is Mocking?

> **Mocking** creates **fake versions** of dependencies so you can test a class in isolation.

### Real-Life Analogy 🎭

Imagine you're testing a **cashier** at a store:
- You don't need a REAL warehouse to test if the cashier can process payments
- You use a **fake warehouse** that pretends to have items in stock
- This way, you're ONLY testing the cashier's ability, not the warehouse

### In Your EMS Project

When testing `EmployeeService`, you don't want to use a real database. You create **mocks**:

```csharp
// The REAL dependency chain:
EmployeeService → IEmployeeRepository → SQL Server Database

// During testing, we break the chain:
EmployeeService → Mock<IEmployeeRepository> → (no database needed!)
```

### How Moq Works

```csharp
using Moq;

// 1. CREATE a mock
var mockRepo = new Mock<IEmployeeRepository>();

// 2. SETUP behavior — "When GetByIdAsync(1) is called, return this fake employee"
mockRepo.Setup(repo => repo.GetByIdAsync(1))
        .ReturnsAsync(new Employee { Id = 1, FirstName = "Kartik" });

// 3. USE the mock — pass .Object to get the fake instance
var service = new EmployeeService(mockRepo.Object, ...);

// 4. VERIFY it was called — "Was GetByIdAsync called exactly once?"
mockRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
```

### Mock Setup Cheat Sheet

| Setup | What It Does |
|---|---|
| `.ReturnsAsync(value)` | Returns a specific value |
| `.ReturnsAsync((Type)null)` | Returns null |
| `.ThrowsAsync(new Exception())` | Throws an exception |
| `.ReturnsAsync(true)` | Returns true (for bool methods) |
| `.Callback(() => { ... })` | Runs custom code when called |
| `It.IsAny<int>()` | Matches any integer argument |
| `It.Is<int>(x => x > 0)` | Matches integers greater than 0 |

---

## 24. Writing Unit Tests for EmployeeService

### Complete Example: EmployeeServiceTests.cs

```csharp
using Application.DTOs.Employee;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EmpMS.Tests.UnitTests.Services
{
    public class EmployeeServiceTests
    {
        // ── Shared mocks (used by all tests) ──
        private readonly Mock<IEmployeeRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IWebHostEnvironment> _mockEnv;
        private readonly Mock<ILogger<EmployeeService>> _mockLogger;
        private readonly Mock<IUnitOfWork> _mockUoW;
        private readonly EmployeeService _service;

        public EmployeeServiceTests()
        {
            // Setup runs BEFORE EACH test
            _mockRepo = new Mock<IEmployeeRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockLogger = new Mock<ILogger<EmployeeService>>();
            _mockUoW = new Mock<IUnitOfWork>();

            _service = new EmployeeService(
                _mockRepo.Object,
                _mockMapper.Object,
                _mockEnv.Object,
                _mockLogger.Object,
                _mockUoW.Object
            );
        }

        // ══════════════════════════════════════
        // GetEmployeeByIdAsync Tests
        // ══════════════════════════════════════

        [Fact]
        public async Task GetEmployeeById_NegativeId_ThrowsBadRequest()
        {
            // ACT & ASSERT
            var exception = await Assert.ThrowsAsync<BadRequestException>(
                () => _service.GetEmployeeByIdAsync(-1)
            );
            Assert.Equal("Enter a valid employee ID!", exception.Message);
        }

        [Fact]
        public async Task GetEmployeeById_ZeroId_ThrowsBadRequest()
        {
            var exception = await Assert.ThrowsAsync<BadRequestException>(
                () => _service.GetEmployeeByIdAsync(0)
            );
            Assert.Equal("Enter a valid employee ID!", exception.Message);
        }

        [Fact]
        public async Task GetEmployeeById_NonExistentId_ThrowsNotFound()
        {
            // ARRANGE — repo returns null (employee not found)
            _mockRepo.Setup(r => r.GetByIdAsync(999))
                     .ReturnsAsync((Employee)null);

            // ACT & ASSERT
            await Assert.ThrowsAsync<NotFoundException>(
                () => _service.GetEmployeeByIdAsync(999)
            );
        }

        [Fact]
        public async Task GetEmployeeById_ValidId_ReturnsEmployeeDto()
        {
            // ARRANGE
            var fakeEmployee = new Employee { Id = 1, FirstName = "Kartik" };
            var expectedDto = new EmployeeResponseDto { Id = 1, FirstName = "Kartik" };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fakeEmployee);
            _mockMapper.Setup(m => m.Map<EmployeeResponseDto>(fakeEmployee))
                       .Returns(expectedDto);

            // ACT
            var result = await _service.GetEmployeeByIdAsync(1);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Kartik", result.FirstName);
        }

        // ══════════════════════════════════════
        // CreateEmployeeAsync Tests
        // ══════════════════════════════════════

        [Fact]
        public async Task CreateEmployee_DuplicateEmail_ThrowsBadRequest()
        {
            // ARRANGE — email already exists
            _mockRepo.Setup(r => r.EmailExistAsync("existing@company.com"))
                     .ReturnsAsync(true);

            var dto = new CreateEmployeeDto { Email = "existing@company.com" };

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<BadRequestException>(
                () => _service.CreateEmployeeAsync(dto)
            );
            Assert.Contains("already exists", ex.Message);
        }

        [Fact]
        public async Task CreateEmployee_ValidData_CallsRepoAndSaves()
        {
            // ARRANGE
            _mockRepo.Setup(r => r.EmailExistAsync(It.IsAny<string>()))
                     .ReturnsAsync(false);
            _mockMapper.Setup(m => m.Map<Employee>(It.IsAny<CreateEmployeeDto>()))
                       .Returns(new Employee());

            var dto = new CreateEmployeeDto
            {
                FirstName = "Kartik",
                Email = "kartik@company.com"
            };

            // ACT
            await _service.CreateEmployeeAsync(dto);

            // ASSERT — verify repo methods were called
            _mockRepo.Verify(r => r.CreateAsync(It.IsAny<Employee>()), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        // ══════════════════════════════════════
        // SoftDeleteEmployeeAsync Tests
        // ══════════════════════════════════════

        [Fact]
        public async Task SoftDelete_InvalidId_ThrowsBadRequest()
        {
            await Assert.ThrowsAsync<BadRequestException>(
                () => _service.SoftDeleteEmployeeAsync(0)
            );
        }

        [Fact]
        public async Task SoftDelete_NonExistent_ThrowsNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(999))
                     .ReturnsAsync((Employee)null);

            await Assert.ThrowsAsync<NotFoundException>(
                () => _service.SoftDeleteEmployeeAsync(999)
            );
        }

        [Fact]
        public async Task SoftDelete_ValidEmployee_DeletesAndSaves()
        {
            // ARRANGE
            var employee = new Employee { Id = 5, FirstName = "Test" };
            _mockRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(employee);

            // ACT
            await _service.SoftDeleteEmployeeAsync(5);

            // ASSERT
            _mockRepo.Verify(r => r.SoftDeleteAsync(employee), Times.Once);
            _mockUoW.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        // ══════════════════════════════════════
        // Parameterized Tests (multiple inputs)
        // ══════════════════════════════════════

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(-100)]
        [InlineData(int.MinValue)]
        public async Task GetEmployeeById_InvalidIds_AllThrowBadRequest(int invalidId)
        {
            await Assert.ThrowsAsync<BadRequestException>(
                () => _service.GetEmployeeByIdAsync(invalidId)
            );
        }
    }
}
```

### Understanding [Fact] vs [Theory]

```csharp
// [Fact] — A single test with fixed data
[Fact]
public void Add_TwoNumbers_ReturnsSum()
{
    Assert.Equal(5, Calculator.Add(2, 3));
}

// [Theory] — Same test with MULTIPLE sets of data
[Theory]
[InlineData(2, 3, 5)]
[InlineData(0, 0, 0)]
[InlineData(-1, 1, 0)]
[InlineData(100, 200, 300)]
public void Add_VariousNumbers_ReturnsCorrectSum(int a, int b, int expected)
{
    Assert.Equal(expected, Calculator.Add(a, b));
}
```

---

## 25. Writing Integration Tests for API Endpoints

### Custom WebApplicationFactory

```csharp
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the real database registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            // Add in-memory database for testing
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("TestDatabase"));

            // Seed test data
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
            SeedTestData(db);
        });
    }

    private void SeedTestData(AppDbContext db)
    {
        db.Departments.Add(new Department { Id = 1, Name = "Engineering" });
        db.Employees.Add(new Employee
        {
            Id = 1, FirstName = "Test", LastName = "Admin",
            Email = "admin@test.com", DepartmentId = 1
        });
        db.SaveChanges();
    }
}
```

### Employee Controller Integration Tests

```csharp
public class EmployeeControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public EmployeeControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_WithoutAuth_Returns401()
    {
        var response = await _client.GetAsync("/api/employees");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_WithAuth_Returns200()
    {
        // Login and set token...
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/employees");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("Items", json);
    }

    [Fact]
    public async Task GetById_NonExistent_Returns404()
    {
        // With auth...
        var response = await _client.GetAsync("/api/employees/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
```

---

## 26. Testing Auth Flow

```csharp
public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "admin@test.com",
            password = "Admin@123"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<APIResponse<LoginDto>>();
        Assert.NotNull(result.Data.Token);
        Assert.NotEmpty(result.Data.Token);
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "admin@test.com",
            password = "WrongPassword"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_NonExistentUser_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "nobody@test.com",
            password = "anything"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
```

---

## 27. Code Coverage

### What is Code Coverage?

> **Code Coverage** measures what % of your code is executed by tests.

```
┌──────────────────────────────────────────┐
│  EmployeeService.cs — 85% Coverage       │
│                                          │
│  ✅ Line 60: if (id <= 0)     COVERED    │
│  ✅ Line 62: GetByIdAsync     COVERED    │
│  ✅ Line 63: if (null)        COVERED    │
│  ❌ Line 88: email check      NOT COVERED│
│  ❌ Line 91: mapper.Map       NOT COVERED│
└──────────────────────────────────────────┘
```

### Running Code Coverage

```powershell
# Generate coverage report
dotnet test --collect:"XPlat Code Coverage"

# Install report tool
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate HTML report
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport"

# Open the report
start coveragereport/index.html
```

### Coverage Goals

| Coverage Level | Quality |
|---|---|
| **0-30%** | 🔴 Poor — major risk |
| **30-60%** | 🟡 Acceptable — basic coverage |
| **60-80%** | 🟢 Good — most scenarios covered |
| **80-95%** | 🟢 Excellent — well-tested |
| **100%** | ⚠️ Not always necessary (diminishing returns) |

> [!IMPORTANT]
> **Don't chase 100% coverage**. Focus on testing **critical business logic** like salary calculations, leave validations, and authentication. Testing simple getters/setters has low value.

---

## 28. Test-Driven Development (TDD) Walkthrough

### What is TDD?

> **TDD (Test-Driven Development)** means writing the **test FIRST**, then writing the code to make it pass.

### The Red-Green-Refactor Cycle

```
    ┌──────────┐
    │  1. RED   │  Write a failing test
    │   ❌     │
    └────┬─────┘
         │
    ┌────▼─────┐
    │ 2. GREEN │  Write minimal code to pass
    │   ✅     │
    └────┬─────┘
         │
    ┌────▼──────┐
    │3. REFACTOR│  Clean up the code
    │   🔄     │
    └────┬──────┘
         │
         └──────▶ Repeat
```

### TDD Example: Adding "Employee Count by Department" Feature

**Step 1: RED — Write the failing test first**

```csharp
[Fact]
public async Task GetEmployeeCountByDept_ValidDeptId_ReturnsCount()
{
    // ARRANGE
    _mockRepo.Setup(r => r.CountByDepartmentAsync(1)).ReturnsAsync(15);

    // ACT
    var count = await _service.GetEmployeeCountByDepartmentAsync(1);

    // ASSERT
    Assert.Equal(15, count);
}
```

This test FAILS ❌ because `GetEmployeeCountByDepartmentAsync` doesn't exist yet.

**Step 2: GREEN — Write minimal code to pass**

```csharp
// In IEmployeeService.cs
Task<int> GetEmployeeCountByDepartmentAsync(int departmentId);

// In EmployeeService.cs
public async Task<int> GetEmployeeCountByDepartmentAsync(int departmentId)
{
    return await _employeeRepository.CountByDepartmentAsync(departmentId);
}
```

Now the test PASSES ✅.

**Step 3: REFACTOR — Add validation and error handling**

```csharp
public async Task<int> GetEmployeeCountByDepartmentAsync(int departmentId)
{
    if (departmentId <= 0)
        throw new BadRequestException("Invalid department ID!");

    return await _employeeRepository.CountByDepartmentAsync(departmentId);
}
```

Add more tests for the new validation:

```csharp
[Fact]
public async Task GetEmployeeCountByDept_InvalidId_ThrowsBadRequest()
{
    await Assert.ThrowsAsync<BadRequestException>(
        () => _service.GetEmployeeCountByDepartmentAsync(-1)
    );
}
```

---

## 29. CI/CD — Automated Testing with GitHub Actions

### What is CI/CD?

> **CI/CD** automatically runs your tests every time you push code to GitHub. If any test fails, it blocks the deployment.

### GitHub Actions Workflow

Create file: `.github/workflows/test.yml`

```yaml
name: Run Tests

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  test:
    runs-on: ubuntu-latest

    steps:
    - name: Checkout code
      uses: actions/checkout@v4

    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '9.0.x'

    - name: Restore dependencies
      run: dotnet restore EmpMS/EmpMS.slnx

    - name: Build
      run: dotnet build EmpMS/EmpMS.slnx --no-restore

    - name: Run Tests
      run: dotnet test EmpMS/EmpMS.slnx --no-build --verbosity normal
```

### What Happens

```
Developer pushes code
        │
        ▼
GitHub Actions triggers
        │
        ▼
   dotnet build  →  Build fails?  →  ❌ STOP, notify developer
        │
        ▼
   dotnet test   →  Test fails?   →  ❌ STOP, notify developer
        │
        ▼
   All passed ✅  →  Ready to deploy / merge PR
```

---

## 30. Testing Tools Comparison

### .NET Testing Frameworks

| Feature | xUnit | NUnit | MSTest |
|---|---|---|---|
| **Popularity** | ⭐⭐⭐ Most popular | ⭐⭐ Popular | ⭐ Microsoft default |
| **Test Method** | `[Fact]` | `[Test]` | `[TestMethod]` |
| **Parameterized** | `[Theory]` | `[TestCase]` | `[DataRow]` |
| **Setup** | Constructor | `[SetUp]` | `[TestInitialize]` |
| **Recommended** | ✅ Yes | ✅ Yes | 🔶 Okay |

### Mocking Libraries

| Library | Usage | Example |
|---|---|---|
| **Moq** | Most popular, easy syntax | `new Mock<IRepo>()` |
| **NSubstitute** | Simpler syntax | `Substitute.For<IRepo>()` |
| **FakeItEasy** | Fluent syntax | `A.Fake<IRepo>()` |

### API Testing Tools

| Tool | Type | Best For |
|---|---|---|
| **Postman** | GUI | Manual API testing, team collaboration |
| **Swagger** | Built-in | Quick testing during development |
| **curl** | CLI | Quick command-line tests |
| **Insomnia** | GUI | Alternative to Postman |
| **REST Client (VS Code)** | Extension | Testing from code editor |

---

## 31. Complete Test Checklist for EMS

### Employee Module

| # | Test Case | Type | Expected Result |
|---|---|---|---|
| 1 | GET /api/employees (no auth) | Integration | 401 Unauthorized |
| 2 | GET /api/employees (valid auth) | Integration | 200 + paginated list |
| 3 | GET /api/employees/1 (exists) | Integration | 200 + employee data |
| 4 | GET /api/employees/999 (not exist) | Integration | 404 Not Found |
| 5 | POST create with valid data | Integration | 201 Created |
| 6 | POST create with duplicate email | Unit | Throws BadRequestException |
| 7 | PUT update with valid data | Unit | Calls repo.Update + UoW.Save |
| 8 | DELETE employee (admin) | Integration | 200 OK (soft delete) |
| 9 | DELETE employee (non-admin) | Integration | 403 Forbidden |
| 10 | GET /api/employees/search?name=K | Integration | 200 + filtered results |

### Auth Module

| # | Test Case | Type | Expected Result |
|---|---|---|---|
| 1 | Login with valid credentials | Integration | 200 + JWT token |
| 2 | Login with wrong password | Integration | 401 Unauthorized |
| 3 | Login with non-existent email | Integration | 401 Unauthorized |
| 4 | Access endpoint with expired token | Integration | 401 Unauthorized |
| 5 | Refresh token with valid refresh | Integration | 200 + new token |

### Leave Module

| # | Test Case | Type | Expected Result |
|---|---|---|---|
| 1 | Apply leave with valid data | Unit | Creates leave record |
| 2 | Apply leave with past date | Unit | Throws BadRequestException |
| 3 | Apply leave exceeding balance | Unit | Throws BadRequestException |
| 4 | Approve leave (manager) | Unit | Updates status to Approved |
| 5 | Approve leave (non-manager) | Integration | 403 Forbidden |

### Salary Module

| # | Test Case | Type | Expected Result |
|---|---|---|---|
| 1 | Generate salary for valid employee | Unit | Correct calculation |
| 2 | Generate salary for non-existent | Unit | Throws NotFoundException |
| 3 | Salary with overtime | Unit | Overtime amount included |
| 4 | Salary with deductions | Unit | Deductions applied correctly |

## 32. Learning Path

Follow this order to learn testing step by step:

```powershell
# Quick Start — set up right now:
cd d:\Projects\emp-management-system-asp.net-web-api\EmpMS
dotnet new xunit -n EmpMS.Tests
dotnet sln EmpMS.slnx add EmpMS.Tests/EmpMS.Tests.csproj
dotnet add EmpMS.Tests reference Application/Application.csproj
dotnet add EmpMS.Tests reference Domain/Domain.csproj
dotnet add EmpMS.Tests package Moq
dotnet test
```

```
Week 1: Read Part I  → Understand concepts & terminology
Week 2: Read Part II → Learn API testing with Postman
Week 3: Read Part III → Set up test project, write first unit test
Week 4: Write 10 unit tests for EmployeeService
Week 5: Write 5 integration tests for Employee API
Week 6: Add tests for Auth, Leave, Salary modules
Week 7: Set up CI/CD with GitHub Actions
Week 8: Practice TDD on a new feature
```

> [!TIP]
> **Start small**. Write ONE test today. Then one more tomorrow. Building the habit is more important than writing 100 tests on day one.

---

> **📚 End of Guide** — You now have everything you need to start testing your projects and APIs. Good luck! 🚀

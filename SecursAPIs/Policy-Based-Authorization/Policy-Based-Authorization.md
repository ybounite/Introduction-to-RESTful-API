## Policy-Based Authorization & Advanced Rules

**Centralize Ownership Logic and Build Scalable Security**

## 🗝️ Introduction

So far, you implemented ownership checks like this:

- Read the authenticated user ID from JWT
- Compare it with the `{id}` in the route
- Allow access if the user is the owner or an admin

This approach is **correct**, but it does **not scale**.

As your API grows, repeating ownership logic inside controllers becomes:

- Hard to maintain
- Easy to forget
- Easy to implement inconsistently
- Difficult to audit

This is where **policy-based authorization** becomes essential.

##

## Policy-Based Authorization

## 🎯 Lesson Goal

By the end of this lesson, you will learn how to:

- 🔹 Centralize ownership logic
- 🔹 Use policies instead of inline checks
- 🔹 Build reusable, scalable authorization rules
- 🔹 Structure your project correctly for real-world APIs

##

## 🔹 Why Policies Exist (The Real Problem They Solve)

Inline ownership checks inside controllers are:

- Correct, but duplicated
- Scattered across methods
- Hard to evolve when rules change

A **policy** moves security rules to **one central place**, so you can:

- Reuse the same rule in multiple endpoints
- Change the rule once and affect the entire API
- Keep controllers clean and focused on business logic

##

## 🔹 What Is Policy-Based Authorization?

A **policy** is a named security rule evaluated by ASP.NET Core’s authorization system.

Instead of writing this logic inside endpoints:

```csharp
if (!isAdmin && authenticatedStudentId != id)
    return Forbid();
```

You define the rule **once** and apply it through the authorization system:

```csharp
AuthorizeAsync(User, id, "StudentOwnerOrAdmin")
```

Authorization becomes **centralized, consistent, and scalable**.

### How the evaluation actually works:

1. Controller receives the request
2. Controller knows the resource ID (`id`)
3. Controller asks the authorization system:

```csharp
AuthorizeAsync(User, id, "StudentOwnerOrAdmin")
```

4. ASP.NET Core then:
   - Finds the policy
   - Finds the requirement
   - Finds the handler
   - Passes the `id` to the handler
   - Makes the final authorization decision

📌 **Important:**
This is **resource-based authorization**, which **cannot** be expressed correctly using attributes alone.

This is exactly how ASP.NET Core is designed to handle ownership checks.

##

## 🔹 Project Structure (Very Important)

Before writing any code, we must place it in the correct location.

Authorization logic is **security infrastructure**, not business logic.

### ✅ Recommended Project Structure

```text
StudentApi
│
├── Authorization
│   ├── StudentOwnerOrAdminRequirement.cs
│   └── StudentOwnerOrAdminHandler.cs
│
├── Controllers
│   ├── StudentsController.cs
│   └── AuthController.cs
│
├── Models
├── DataSimulation
├── Program.cs
```

📌 Everything related to authorization rules lives in the **Authorization** folder.

##

## 🔹 Step 1: Define the Ownership Requirement

📁 **Location:** `Authorization/StudentOwnerOrAdminRequirement.cs`

```csharp
using Microsoft.AspNetCore.Authorization;

// This class represents the authorization rule itself.
// It does NOT contain logic.
// It simply defines the requirement:
// "Owner OR Admin can access the student resource."
public class StudentOwnerOrAdminRequirement : IAuthorizationRequirement
{
}
```

📌 **Important concept:**

- This class is intentionally empty
- It represents **what rule exists**, not **how it is enforced**

##

## 🔹 Step 2: Create the Authorization Handler

📁 **Location:** `Authorization/StudentOwnerOrAdminHandler.cs`

```csharp
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

// This authorization handler enforces the ownership rule for student resources.
// It checks whether the current user is either:
// - An Admin (full access), OR
// - The owner of the student record being requested
public class StudentOwnerOrAdminHandler
    : AuthorizationHandler<StudentOwnerOrAdminRequirement, int>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        StudentOwnerOrAdminRequirement requirement,
        int studentId)
    {
        // Admin override
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Ownership check
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (int.TryParse(userId, out int authenticatedStudentId) &&
            authenticatedStudentId == studentId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
```

📌 **Important detail:**

- This is a **resource-based handler**
- The resource is the `studentId` being requested

##

## 🔹 Step 3: Register the Handler in `Program.cs`

```csharp
builder.Services.AddSingleton<IAuthorizationHandler, StudentOwnerOrAdminHandler>();
```

📌 This tells ASP.NET Core:

> When this requirement is evaluated, use this handler.

##

## 🔹 Step 4: Register the Policy in `Program.cs`

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("StudentOwnerOrAdmin", policy =>
        policy.Requirements.Add(new StudentOwnerOrAdminRequirement()));
});
```

Your API now has a **reusable, named policy**.

##

## 🔹 Step 5: Use the Policy in the Student Endpoint

📁 **Location:** `StudentsController`

```csharp
[HttpGet("{id}", Name = "GetStudentById")]
public async Task<ActionResult<Student>> GetStudentById(
    int id,
    [FromServices] IAuthorizationService authorizationService)
{
    if (id < 1)
        return BadRequest("Invalid student id.");

    var student = StudentDataSimulation.StudentsList
        .FirstOrDefault(s => s.Id == id);

    if (student == null)
        return NotFound("Student not found.");

    var authResult = await authorizationService.AuthorizeAsync(
        User,
        id,
        "StudentOwnerOrAdmin");

    if (!authResult.Succeeded)
        return Forbid(); // 403

    return Ok(student);
}
```

📌 **Why not use** `[Authorize(Policy = "...")]` **attribute?**

Because:

- Attributes **cannot pass resource data** (`id`)
- Ownership is **dynamic**
- Resource-based authorization **requires runtime evaluation**

This is the **correct** and **recommended** approach.

##

## 🔹 Why This Design Scales?

With this structure:

- Security rules live in one place
- Controllers stay clean
- Ownership logic is reusable
- Rules are easy to audit and change

If tomorrow the rule becomes:

- Admin **OR** Manager
- Student **OR** Guardian
- Student only during active enrollment

You change **one handler**, not 10 controllers.

### **🔹 Why Not to Make Just a Helper Method?**

A common question is:

> “Why not write a centralized helper method and call it from controllers?”

While helper methods may seem simpler, they create serious long-term problems.

Helper methods:

- ❌ Are invisible to the authorization system
- ❌ Cannot be audited or composed
- ❌ Hide security intent
- ❌ Scale poorly as rules evolve

Policy-based authorization:

- ✅ Makes security intent explicit
- ✅ Centralizes rules in one place
- ✅ Allows auditing and composition
- ✅ Integrates with the framework’s security pipeline

Even if a policy is forgotten on an endpoint, **the policy still exists, is auditable, and is structurally correct**.

With helper methods, there is no concept of “missing protection” — only hidden mistakes.

📌 **Policies don’t prevent forgetting. They prevent chaos.**

> **Security is not about writing code that works.**
> **It’s about writing systems that fail safely and predictably.**

Policy-based authorization is not about convenience.

It is about:

- Structure
- Visibility
- Consistency
- Professional-grade security design

## **Why We Built a Policy Even for One Endpoint**

This is intentional.

Policies are created based on correctness and future growth, not quantity.

Today, the policy protects:

- One endpoint

Tomorrow, it may protect:

- Multiple endpoints
- New features
- Additional resources

Because the rule is centralized, you can reuse it safely without rewriting logic.

##

## **🧠 How to Think Going Forward**

Whenever you design a new endpoint, ask:

- Does this access depend on who owns the data?
- Can roles alone express the rule?
- Does this require comparing user identity with resource data?

If the answer is yes, use an ownership policy.

If not, use roles or allow anonymous access.

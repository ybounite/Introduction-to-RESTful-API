## Role-Based Authorization — What Can Students and Admins Do?

## 🗝️ Introduction

Up to this point, your API has achieved something critical:

✅ Secure communication (HTTPS + CORS)
✅ Authentication using JWT
✅ The API knows **who** is calling it

But knowing _who_ is calling is **not enough**.

If every authenticated user can access every endpoint, then your system is still unsafe.

This lesson introduces **clear security boundaries** using roles.

##

## 🎯 Lesson Goal

By the end of this lesson, you will understand:

- 🔹 Why authentication alone is dangerous
- 🔹 What role-based authorization really means
- 🔹 How to define clear access boundaries
- 🔹 How students and admins are treated differently
- 🔹 Why **403 Forbidden** is a security feature
- 🔹 How this prepares you for ownership & policies

##

## 🔐 Authentication vs 🛂 Authorization (Quick Reminder)

### Authentication answers:

**Who are you?**

**Example:**

- “I am youssef”
- “Here is my valid JWT”

### Authorization answers:

**What are you allowed to do?**

Example:

- “Can you delete students?”
- “Can you see all records?”
- “Can you access someone else’s data?”

📌 Authentication always comes first.
📌 Authorization always comes second.

##

## 🔹 The Security Boundary Table (Very Important)

From now on, **this table defines the rules of your API**.
Every lesson that follows will respect this boundary.

## ![](https://uploads.teachablecdn.com/attachments/294e29cc471249ffad2800f2821a2e16.png)

📌 This table is **not optional**.
📌 This is your **security contract**.

##

## 🔹 What This Table Really Means

Let’s interpret it correctly.

### 🔓 Public Endpoints

These endpoints:

- Do NOT require authentication
- Do NOT require a JWT
- Are safe to expose publicly

Examples:

- Viewing passed students
- Viewing average grades

These endpoints **do not reveal sensitive data**.

##

### 🪪 Student (Owner) or Admin

This is the first **advanced security rule**.

Example:

```text
GET /api/Students/{id}
```

Access is allowed if:

- The user is an **Admin**
- OR the user is a **Student requesting their own record**

📌 A student must NOT access another student’s data.

This introduces the concept of **ownership**, which will be handled later using policies.

##

### 👑 Admin-Only Endpoints

These are **dangerous operations**:

- Viewing all students
- Creating students
- Updating students
- Deleting students

Only admins are allowed.

Students are explicitly blocked — even if authenticated.

##

## 🔹 Why This Boundary Is Critical

Without this table:

- ❌ Any logged-in student could delete data
- ❌ Any logged-in student could modify records
- ❌ Any logged-in student could view all students

This is how **real systems get breached**, even with JWT.

##

## 🔹 How ASP.NET Core Enforces These Rules

ASP.NET Core uses **declarative authorization**.

Examples:

```csharp
[Authorize]
```

Requires authentication only.

```csharp
[Authorize(Roles = "Admin")]
```

Requires:

- A valid JWT
- Role = Admin

📌 Controllers do NOT read JWT manually.
📌 Controllers do NOT parse claims manually.
📌 The framework enforces rules consistently.

##

## 🔹 Understanding 401 vs 403 (Critical)

### ❌ 401 Unauthorized

- No token
- Invalid token
- Authentication failed

Meaning:

> “I don’t know who you are.”

##

### ❌ 403 Forbidden

- Token is valid
- User is authenticated
- User is NOT allowed

Meaning:

> “I know who you are — but you are not allowed.”

📌 403 is **not an error**.
📌 403 is **a security success**.

##

## 🧠 Your Current Security Level

You are now moving from:

**Authenticated API (JWT)**
⬇️
**Authorized API (Roles)**

This means:

- Identity exists
- Permissions now matter
- Not all users are equal anymore

##

## 🔗 How This Fits Into the Security Roadmap

You are building security in layers:

1. Secure transport (HTTPS + CORS)
2. Authentication (JWT — identity)
3. Authorization (Roles — permissions) ← **you are here**
4. Ownership & policy-based rules
5. Production hardening

📌 Skipping a layer breaks security.
📌 You are building it in the correct order.

##

## 🛠️ Final Summary of Interconnections

- 🔹 Authentication tells **who you are**
- 🔹 Roles tell **what you can do**
- 🔹 The boundary table defines **what must be protected**
- 🔹 Authorization enforces those rules automatically
- 🔹 403 Forbidden protects your system

##

## 🏁 Conclusion

You now clearly understand:

- ✅ Why authentication alone is dangerous
- ✅ How roles protect sensitive operations
- ✅ How to read and enforce access boundaries
- ✅ Why admins and students must be separated
- ✅ How this prepares you for ownership rules

📌 Your API is no longer just authenticated.
📌 It is becoming **professionally authorized**.

##

## ▶️ Next Lesson

🛠️ **Implementing Role-Based Authorization in the Student API**

You will:

- Apply `[Authorize(Roles = "...")]`
- Enforce admin-only endpoints
- Prepare for ownership rules
- Test 403 Forbidden in action
- Respect the boundary table fully

**This is where permissions become real security 🔐**

## Step 1: Identify Public Endpoints
Some endpoints **do not require authentication**.

### Why?

- They expose **aggregated or non-sensitive data**
- They do not reveal individual student identities
- They are safe to consume publicly

### Public Endpoints

- GET /api/Students/Passed
- GET /api/Students/AverageGrade
Because you have `[Authorize]` at controller level, these endpoints must override it with `[AllowAnonymous]` (boundary table says: **Public**).

**✅ GET /api/Students/Passed → Public**

**Update your endpoint like this:**

```
[AllowAnonymous]
[HttpGet("Passed", Name = "GetPassedStudents")]
public ActionResult<IEnumerable<Student>> GetPassedStudents()
{
		...
}
```

**✅ GET /api/Students/AverageGrade → Public**

**Update your endpoint like this:**

```
[AllowAnonymous]
[HttpGet("AverageGrade", Name = "GetAverageGrade")]
public ActionResult<double> GetAverageGrade()
{
		...
}
```

📌 Being explicit prevents future mistakes when global authorization rules are added.

## 

## 🔹 Step 2: Secure the Controller by Default
All **non-public** endpoints should require authentication.

### Best Practice
Protect the controller itself: we already did that before.

```
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
}
```
📌 This ensures:

- Anonymous users are blocked by default
- Security is centralized
- No endpoint is accidentally left open

## 

## 🔹 Step 3: Restrict Admin-Only Endpoints
Based on the boundary table, the following endpoints are **Admin-only**:

- GET /api/Students/All
- POST /api/Students
- PUT /api/Students/{id}
- DELETE /api/Students/{id}

### **Implementation:**
**✅ GET /api/Students/All → Admin**

```
[Authorize(Roles = "Admin")]
[HttpGet("All", Name ="GetAllStudents")]
public ActionResult<IEnumerable<Student>> GetAllStudents()
{
		...
}
```

**✅ POST /api/Students → Admin**

```
[Authorize(Roles = "Admin")]
[HttpPost(Name = "AddStudent")]
public ActionResult<Student> AddStudent(Student newStudent)
{
		...
}
```

**✅ PUT /api/Students/{id} → Admin**

```
[Authorize(Roles = "Admin")]
[HttpPut("{id}", Name = "UpdateStudent")]
public ActionResult<Student> UpdateStudent(int id, Student updatedStudent)
{
		...
}
```

**✅ DELETE /api/Students/{id} → Admin**

```
[Authorize(Roles = "Admin")]
[HttpDelete("{id}", Name = "DeleteStudent")]
public ActionResult DeleteStudent(int id)
{
		...
}
```

📌 ASP.NET Core will automatically:

- Extract the role from the JWT
- Compare it to the required role
- Return **403 Forbidden** if the role does not match

## 

## 🔹 Step 4: Understanding 401 vs 403 (Critical)
When authorization is active, two errors matter:

### ❌ 401 Unauthorized

- No token
- Invalid token
- Authentication failed

### ❌ 403 Forbidden

- Token is valid
- User is authenticated
- User **does not have the required role**
📌 **403 is a security success**, not a bug.

## 

## 🔹 Step 5: Endpoints That Need Ownership (Not Yet)
The endpoint below **cannot be secured by role alone**:

- GET /api/Students/{id}
Why?

- A student should see **only their own record**
- An admin should see **any student**
📌 Role-based authorization alone is **not enough here**.

We intentionally **do not implement this now**.

This endpoint will be handled in the **Ownership & Policy-Based Authorization** lesson.

## 

## 🧠 Why We Do It in This Order
Security is built in layers:

1. Secure transport (HTTPS + CORS)
2. Authentication (JWT)
3. Authorization (Roles) ← **you are here**
4. Ownership rules
5. Policies & production hardening
Skipping steps leads to fragile systems.

## 

## 🧬 Characteristics of Role-Based Authorization

- Declarative (attributes, not if-statements)
- Centralized
- Enforced automatically
- Based on identity claims
- Easy to audit and reason about

## 

## 🔗 Interconnection

- Login → issues JWT
- JWT → carries role claim
- Middleware → validates identity
- Authorization → checks role
- Controller → allows or blocks action

## 

## 🛠️ Final Summary of Interconnections

- Authentication without authorization is dangerous
- Roles convert identity into controlled access
- Admin-only endpoints protect system integrity
- Public endpoints must be intentionally public
- Ownership rules require the next layer

## 

## 🏁 Conclusion
You have now:

- Implemented real role-based authorization
- Enforced Admin-only operations
- Protected sensitive endpoints
- Followed a clear security boundary
- Prepared the API for ownership rules
Your API has officially moved from:

**Authenticated API → Authorized API**

## 

## ▶️ Next Lesson
**We will move to : **

🟦 Module 6 — Ownership Rules (Policies)

🧭 **Ownership-Based Authorization — Is This Your Data?**

In the next lesson, you will learn:

- Why roles are not enough
- How to restrict students to their own records
- How to prevent horizontal privilege escalation
- How to implement ownership checks correctly
This is where **real-world security** begins.

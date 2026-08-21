# Security Audit — Attacking the Existing Student API

## Security Maturity Levels

Open -> Foundation (HTTPS + CORS) -> Authenticated -> Authorized -> Production

### 🗝️ Introduction

Many developers think security is just one step:

> “I added JWT, so my API is secure.”

That belief creates weak systems.

Real API security grows through maturity levels. Each level exists because it solves a specific security problem.

In this lesson, you will learn the correct security maturity progression for our Student API.

🎯 Outcome:
The student understands why every next module exists and why security must be built in layers.

### 🔹 What Are Security Maturity Levels?

Security maturity levels describe how secure an API is, based on:

- What threats it can handle
- What attacks it can resist
- What security gaps still remain

📌 Security is not one feature.
Security is a step-by-step evolution.

### 🔹 The Correct Maturity Path for This Course

For the Student API, the correct progression is:

> Open -> Foundation (HTTPS + CORS) -> Authenticated -> Authorized -> Production

Each level answers a new question.

### 🔹 Level 0️⃣ — Open API (No Security)

❓ Security Question

❌ None - the API is open.

🔴 Characteristics

- No HTTPS enforcement
- No CORS policy
- No authentication
- No authorization
- Anyone can call anything

🔴 Example

```http
DELETE /api/Students/5   ✅ works without login
```

📌 This level is easy to build and easy to break.

### 🔹 Level 1️⃣ — Foundation (HTTPS + CORS)

❓ Security Question

🌐 Is communication safe, and is browser access controlled?

🟡 What We Add?

- HTTPS (TLS)
  - Encrypts traffic in transit
  - Protects tokens and credentials from being sniffed
  - Prevents man-in-the-middle attacks (when configured correctly)
- CORS policy
  - Controls which browser origins can call your API
  - Limits browser-based abuse from untrusted websites
  - Defines allowed methods and headers

📌 Important:

- HTTPS is transport security
- CORS is browser access control
- Neither of them proves who the user is (that comes next)

### 🔹 Level 2️⃣ — Authenticated API (JWT) — Who Are You?

❓ Security Question

🪪 Who is calling the API?

🟡 What We Add?

- Registration
- Password hashing
- Login
- JWT issuance
- JWT authentication middleware

🟡 Result

- Anonymous users are blocked
- The API knows the caller identity

Example:

```http
DELETE /api/Students/5   ❌ 401 Unauthorized (no token)
```

📌 Problem remaining:
Authenticated users may still be overpowered (any student can do the admin role!).

### 🔹 Level 3️⃣ — Authorized API (Roles) — What Can You Do?

❓ Security Question

🛂 What is this user allowed to do?

🟡 What We Add?

- Role-based authorization (Student, Admin)
- [Authorize]
- [Authorize(Roles="Admin")] for write endpoints

🟡 Result

- Students are restricted
- Admin operations are protected

Example:

```http
Student -> DELETE /api/Students/5   ❌ 403 Forbidden
Admin   -> DELETE /api/Students/5   ✅ Allowed
```

📌 Problem remaining:
Roles alone cannot prevent accessing other users' data. (any student can see other student information)

### 🔹 Level 4️⃣ — Production-Ready API (Survives Reality)

❓ Security Questions

- Is this your data? (ownership)
- Can sessions be controlled? (refresh/logout)
- Can the API resist abuse? (rate limiting)
- Can we detect and trace events? (logging/auditing)

🟢 What We Add?

- Ownership policies (policy-based authorization)
- Refresh tokens + logout + rotation
- Rate limiting (especially login/refresh)
- Logging & auditing

🟢 Result

- Prevents horizontal privilege escalation
- Supports safe long sessions
- Resists brute-force and abuse
- Enables visibility and accountability

📌 This is where your API becomes professional and real-world ready.

### 🧬 Characteristics

- HTTPS + CORS are mandatory foundations
- Authentication identifies users
- Authorization controls actions
- Production security adds resilience, ownership, and visibility
- Skipping levels creates security holes

### 🔗 Interconnection

- HTTPS protects transport -> safe credentials/tokens
- CORS limits browser origins -> reduced attack surface
- JWT provides identity -> [Authorize] can work
- Roles control authority -> policies enforce ownership
- Refresh tokens manage sessions -> rate limiting stops abuse
- Logging/auditing provide visibility -> incidents can be traced

### 🛠️ Summary of Interconnections

- Foundation enables safe communication -> identity can be trusted.
- Identity enables authorization -> permissions can be enforced.
- Production adds ownership + resilience + visibility -> real security.

### 🏁 Conclusion

Now the maturity levels are correct and complete:

- Open
- Foundation (HTTPS + CORS)
- Authenticated
- Authorized
- Production

🎯 Outcome achieved:
The student understands exactly why each next module exists, and why skipping foundational layers causes real vulnerabilities.

### 🗝️ Introduction

In the previous lessons, you confirmed two critical facts:

✅ Your **Student API works correctly**

❌ Your **Student API is completely open**

In this lesson, we will **intentionally attack our own API**.

**⚠️ This is not hacking:**
This is a **security audit** — something **every professional backend developer must know how to do**.

You cannot secure something **until you clearly see how it breaks**.

### Hacking vs Security Auditing

![](https://uploads.teachablecdn.com/attachments/87f9ee2c65d64139a6fd341cec63c492.png)

**🔴 Hacking (Illegal / Malicious)**

- Goal: **Cause harm or steal data**
- Permission: ❌ **No permission**
- Intent: ❌ **Malicious**
- Outcome:
  - Data theft
  - System damage
  - Service disruption

📌 Uses **exploitation for personal gain**

**🟢 Security Auditing / Ethical Testing**

- Goal: **Find risks before attackers do**
- Permission: ✅ **Explicit permission**
- Intent: ✅ **Defensive & protective**
- Outcome:
  - Risk reports
  - Security improvements
  - Stronger systems

📌 Uses **attacker mindset for protection**

### What Is a Security Audit?

A security audit means:

- Testing the system as an attacker
- Trying to misuse valid endpoints
- Observing what the system allows
- Identifying **security risks**, not bugs

📌 Important:
The API is doing **exactly what we told it to do** — the problem is **what we did NOT tell it to do**.

### What “The API is doing exactly what we told it to do” Means

Imagine you wrote this API rule:

> “If someone calls `GET /students`, return all students.”

The API does that **perfectly**.

It does **not** ask:

- Who is calling?
- Should they see all students?
- Are they allowed?

Because **you never told it to ask those questions**.

So from the API’s point of view:

✅ It is correct

❌ It is not safe

### What “the problem is what we did NOT tell it to do” Means

You **did NOT tell the API** things like:

- “Only logged-in users can call this”
- “Only admins can see all students”
- “A student can see only their own record”
- “Block too many requests”
- “Reject unexpected input”

Since those rules **don’t exist**, the API allows everything.

📌 That is not a bug — that is **missing security instructions**.

### Think Like an Attacker (Security Audit Mindset)

An attacker does NOT think:

> “How do I crash the API?”

They think:

> “What is the API allowed to do?”

So they try things like:

- Call endpoints without logging in
- Access IDs that are not theirs
- Send requests very fast
- Use valid endpoints in unintended ways

And the API replies:

> “Sure, you didn’t tell me not to.”

### Very Important Difference

**❌ Bug**

- Code crashes
- Wrong output
- Exception thrown

**🔐 Security Risk**

- API works
- Returns data
- Allows something dangerous

📌 Security risks are **successful behavior**, not failures.

### Real Example (Very Simple)

```http
GET /students/1
GET /students/2
GET /students/3
```

The API returns all of them.

**Why?**

- You told it: “Return student by ID”
- You did NOT tell it: “Check ownership”

So the API is:

✅ Correct

❌ Insecure

### One Sentence Summary (Memorize This)

> **Security problems are caused by missing rules, not broken code.**

Or in your words:

> **The API is doing exactly what we told it to do — the real problem is what we did NOT tell it to do.**

### Attack #1: Unauthorized Delete (Critical)

**Instructions**

Using Swagger or Postman:

- Call `DELETE /api/Students/{id}`
- Use any valid student ID
- Do NOT log in
- Do NOT send any token

**What Just Happened?**

- The student was deleted
- No identity was checked
- No permission was required

❌ Invalid for production

**Why This Is Dangerous?**

Imagine this in a real system:

- University student records
- Grades
- Personal data

Anyone could:

- Delete students
- Destroy academic history
- Corrupt reports

### Attack #2: Unauthorized Update

**Instructions**

Call: `PUT /api/Students/{id}`

**Modify:**

- Name
- Age
- Grade

**Again:**

- No login
- No token

**Result**

- Student data changes successfully

❌ Invalid for production

### Attack #3: Mass Data Exposure (Privacy Risk)

**Instructions**

Call: `GET /api/Students/All`

Anyone can retrieve:

- All students
- All grades
- All data

Even if deletion is blocked later, **data exposure is also a security risk**.

### Characteristics (What We Discovered)

- The API trusts everyone
- There is no identity
- There is no permission system
- Write endpoints are extremely dangerous
- Read endpoints may leak sensitive data

### Interconnection (Audit → Security Design)

- Attacks → Security requirements
- Open endpoints → Access rules
- CRUD logic → Authorization logic

**Summary of Interconnections**

- Seeing the attack → understanding the risk
- Understanding the risk → justifying security
- Security is a response to danger, not decoration

### Conclusion

You have now **proven** that your API is unsafe.

Not by theory — but by **real attacks on your own code**.

This lesson gives us **permission and motivation** to add security.

📌 In the next lesson, we will **define security boundaries**:

- Which endpoints stay public
- Which require login
- Which require admin permissions

That is the **foundation of all API security**.

## Defining Security Boundaries for Student Endpoints

### 🗝️ Introduction

In the previous lesson, you **proved** that your Student API is unsafe by performing real attacks.

Now comes a **very important professional step**:

> 🔐 **Deciding who is allowed to access what**

This step is called **defining security boundaries**. Without clear boundaries, **no authentication or JWT will help you**.

![](https://uploads.teachablecdn.com/attachments/9e7733930c6447d997a28d5b183066e0.png)

### 🔹 What Are Security Boundaries?

Security boundaries answer three critical questions:

- Who can access this endpoint?
- Should this endpoint be public or protected?
- What happens if the wrong person accesses it?

📌 This is **design work**, not coding yet.

### 🔹 Step 1: List All Existing Endpoints (From Your API)

Your current endpoints are:

- `GET /api/Students/All`
- `GET /api/Students/Passed`
- `GET /api/Students/AverageGrade`
- `GET /api/Students/{id}`
- `POST /api/Students`
- `PUT /api/Students/{id}`
- `DELETE /api/Students/{id}`

### 🔹 Step 2: Categorize Endpoints by Risk

Let’s classify them **by danger level**:

#### 🔴 High Risk (Must Be Protected)

- `POST /api/Students`
- `PUT /api/Students/{id}`
- `DELETE /api/Students/{id}`

**Why?**

- Modify data
- Can destroy records
- Can corrupt grades

#### 🟡 Medium Risk (Needs Careful Rules)

- `GET /api/Students/{id}`
- `GET /api/Students/All`

**Why?**

- Exposes personal data
- Can leak grades and identities

#### 🟢 Low Risk (Can Be Public)

- `GET /api/Students/Passed`
- `GET /api/Students/AverageGrade`

**Why?**

- Aggregated data
- No personal identity exposure

### 🔹 Step 3: Define Who Should Access Each Group

Now we define **clear access rules**.

#### 🔐 Admin Only

- Create student
- Update student
- Delete student

**Endpoints:**

- `POST /api/Students`
- `PUT /api/Students/{id}`
- `DELETE /api/Students/{id}`

#### 🟡 Authenticated Student (With Rules)

- View own student record

**Endpoint:**

- `GET /api/Students/{id}`

📌 Rule:
A student must **only access his own ID**.

#### 🟢 Public (No Login Required)

- View passed students
- View average grade

**Endpoints:**

- `GET /api/Students/Passed`
- `GET /api/Students/AverageGrade`

### 🔹 Step 4: Final Security Boundary Table

![](https://uploads.teachablecdn.com/attachments/e15f4800917c489394985f34260df968.png)

### 🧬 Characteristics

- Security is planned before coding
- Not all endpoints are equal
- Read endpoints can still be dangerous
- Clear rules simplify implementation

### 🔗 Interconnection

- Attacks → Risk analysis
- Risk analysis → Access rules
- Access rules → JWT, Roles, Policies

**Summary of Interconnections**

- Attacks expose weaknesses → boundaries define protection
- Boundaries define rules → rules guide implementation
- Good security starts with clear decisions

### 🏁 Conclusion

You have now completed the **design phase of security**.

You clearly know:

✅ Which endpoints are dangerous
✅ Which endpoints can remain public
✅ Who should access what

📌 From the next lesson onward, we will **start implementing security**.

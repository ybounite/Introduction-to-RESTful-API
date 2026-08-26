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

## Common API Security Myths (That Break Real Systems)

### ![](https://uploads.teachablecdn.com/attachments/f593ce25c1a74e4bb3dd84e7e2faf569.png)

### 🗝️ Introduction

Many APIs fail **not because developers are bad**, but because they believe **dangerous myths** about security.

These myths:

- Sound logical
- Feel safe
- Are repeated everywhere

But in real production systems, they lead to:

- ❌ Data leaks
- ❌ Account takeover
- ❌ Broken trust

In this lesson, we will **destroy the most common API security myths** — using logic and real-world thinking.

🎯 Outcome:
You stop relying on false confidence and start **thinking like a security-aware backend engineer**.

### ❌ Myth 1 — “JWT Means My API Is Secure”

**🔴 Why People Believe This?**

- JWT feels advanced
- Tokens look complex
- Many tutorials stop at JWT

**❌ Why This Is False?**

JWT only answers:

> 🪪 Who are you?

It does **not** answer:

- ❌ What can you do?
- ❌ Is this your data?
- ❌ Can this token be abused?

📌 JWT without authorization is **identity without control**.

**💥 Real Example**

A company adds JWT authentication:

```http
Authorization: Bearer eyJhbGciOi...
```

Now **every endpoint accepts any valid token**.

An attacker:

- Registers a normal user account
- Gets a valid JWT
- Calls:

```http
DELETE /api/users/123
```

📌 Result

- Request succeeds
- Admin-only action executed
- Data deleted

**🔍 What Went Wrong?**

- JWT verified identity
- No authorization checks
- No role or permission enforcement

📌 JWT authenticated the user — it did not protect the action.

### ❌ Myth 2 — “If the User Is Logged In, They Are Trusted”

**🔴 Why People Believe This?**

- Login feels like a gate
- “They have an account, so it’s fine”

**❌ Why This Is False?**

Being logged in does **not** mean:

- ❌ The user is honest
- ❌ The user should access all data
- ❌ The user won’t try random IDs

📌 Most attacks come from **authenticated users**, not anonymous ones.

**💥 Real Example**

A university portal allows logged-in students to view grades:

```http
GET /api/grades/{studentId}
```

A student:

- Logs in normally
- Changes the ID in the request:

```http
GET /api/grades/987
GET /api/grades/988
```

📌 Result

- Other students’ grades exposed

**🔍 What Went Wrong?**

- Login check existed
- Ownership check did NOT

📌 Authenticated ≠ authorized

### ❌ Myth 3 — “Roles Are Enough for Authorization”

**🔴 Why People Believe This?**

- Roles feel powerful (Admin / User)
- Easy to implement

**❌ Why This Is False?**

**Roles answer:**

> 🛂 What type of user are you?

They do **not** answer:

- ❌ Do you own this resource?

📌 Two students have the same role — but different data.

Without ownership rules:

- 🔥 Horizontal privilege escalation happens.

**💥 Real Example**

Both students have the role `Student`.

```json
{
  "userId": 45,
  "role": "Student"
}
```

The API allows:

```http
PUT /api/Students/{id}
```

Any student can update any student record.

📌 Result

- Students modify other students’ profiles
- Grades changed
- Personal data corrupted

**🔍 What Went Wrong?**

- Role check passed
- Ownership rule missing

📌 Roles define type — not ownership

### ❌ Myth 4 — “Nobody Will Guess IDs”

**🔴 Why People Believe This?**

- IDs look random
- “Who would try that?”

**❌ Why This Is False?**

Attackers always try:

- 🔹 Incrementing IDs
- 🔹 Copying requests
- 🔹 Modifying URLs

Example:

```http
GET /api/Students/5
GET /api/Students/6
GET /api/Students/7
```

📌 If the API allows it, it **will be abused**.

**💥 Real Example**

An e-commerce API:

```http
GET /api/orders/1001
```

Attacker tries:

```http
GET /api/orders/1002
GET /api/orders/1003
```

📌 Result

- Full order history leaked
- Names, addresses, phone numbers exposed

**🔍 What Went Wrong?**

- IDs were sequential
- No ownership validation

📌 Attackers don’t guess — they enumerate

### ❌ Myth 5 — “HTTPS Is Enough”

**🔴 Why People Believe This?**

- HTTPS sounds like “security”
- Browsers show a lock icon

**❌ Why This Is False?**

**HTTPS only protects:**

- 🔹 Data during transport

It does **not** protect:

- ❌ Who can access endpoints
- ❌ What actions are allowed
- ❌ Abuse or brute-force

📌 HTTPS is a **foundation**, not a solution.

**💥 Real Example**

API uses HTTPS everywhere 🔒

But:

- No authentication
- No authorization
- No rate limiting

Attacker:

- Securely deletes records over HTTPS
- Securely scrapes all data
- Securely brute-forces endpoints

📌 Result

- Encrypted attacks
- Clean, silent data loss

📌 HTTPS protected the attacker too

### ❌ Myth 6 — “Rate Limiting Is Optional”

**Rate limiting** is a security mechanism that **restricts how many requests a client can make to an API within a specific time window**, to prevent abuse, brute-force attacks, and system overload.

📌 Purpose: protect availability, performance, and security — not functionality.

**💡 Example of Rate Limiting**

- An API allows **100 requests per minute** per user.

If a client sends:

- 1–100 requests → ✅ allowed
- 101+ requests → ❌ blocked (429 Too Many Requests)

📌 This prevents brute-force attacks and abuse.

**🔴 Why People Believe This?**

- “Who would attack my API?”
- “This is just a small project”

**❌ Why This Is False?**

Attackers do not care if:

- ❌ Your project is small
- ❌ Your API is new

Without rate limiting:

- 🔥 Login brute-force
- 🔥 Refresh token abuse
- 🔥 Resource exhaustion

📌 Public APIs are attacked **by default**.

**💥 Real Example**

Login endpoint:

```http
POST /api/auth/login
```

No rate limiting.

**Attacker:**

- Tries 100,000 passwords/hour
- Eventually succeeds

📌 Result

- Account takeover
- No alerts
- No throttling

📌 APIs are attacked automatically — not personally

### ❌ Myth 7 — “Logging Is Only for Debugging”

**🔴 Why People Believe This?**

- Logs feel technical
- Focus is on features

**❌ Why This Is False?**

**Without logs:**

- ❌ You don’t know attacks happened
- ❌ You can’t investigate incidents
- ❌ You can’t prove what happened

📌 Security without visibility is **blind security**.

**💥 Real Example**

A data breach happens.

Questions asked:

- Who accessed the data?
- When?
- From where?
- How many times?

📌 Answer

> “We don’t know.”

📌 Result

- No forensic analysis
- No proof
- No compliance
- No trust

📌 An unlogged system is an invisible system

## 🧬 Characteristics (Reality Check)

- Security is not a single feature
- Attackers are curious and persistent
- Most vulnerabilities are logical, not technical
- Confidence without design is dangerous

### 🔗 Interconnection

- JWT without roles → overpowered users
- Roles without ownership → data leaks
- No rate limiting → brute-force attacks
- No logging → invisible breaches

**Summary of Interconnections:**

- Every myth removes one security layer
- Removing layers creates attack paths
- Secure systems exist because myths are rejected

### 🏁 Conclusion

If you remember **one thing** from this lesson, remember this:

> 🔐 Security fails because of **false assumptions**, not missing libraries.

From now on:

- You will question “simple” security advice
- You will design before implementing
- You will recognize weak APIs immediately

🎯 Outcome achieved:
You now know **what NOT to trust** — which is the first step toward real security.

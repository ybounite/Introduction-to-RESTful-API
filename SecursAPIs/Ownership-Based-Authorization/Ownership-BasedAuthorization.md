(The file `/home/ybounite/Introduction-to-RESTful-API/SecursAPIs/Ownership-Based-Authorization/Ownership-BasedAuthorization.md` exists, but is empty)

## Ownership-Based Authorization — Is This Your Data?

### 🗝️ Introduction

So far, your API has reached an important milestone:

- ✅ It knows **who** is calling it (Authentication)
- ✅ It knows **what role** the user has (Authorization)

But there is still a **critical real-world security problem** that roles alone cannot solve.

**Right now, a logged-in student can do this:**

- Call `GET /api/Students/1`
- Call `GET /api/Students/2`
- Call `GET /api/Students/999`

And the API has **no idea** whether this data belongs to them or not.

**This lesson fixes that problem.**

---

## 🎯 Lesson Goal

By the end of this lesson you will understand:

- Why **roles are not enough**
- What **ownership-based authorization** really means
- What **horizontal privilege escalation** is
- Why this is one of the most common real-world vulnerabilities
- How ASP.NET Core is designed to support ownership checks
- Where ownership fits in the security roadmap

This lesson focuses on **thinking correctly before coding**.

---

## 🔹 Why Roles Alone Are Not Enough

Roles answer this question:

🛂 **What can this user do in general?**

Example:

- `Admin` → can manage students
- `Student` → limited access

But roles do **not** answer:

🧭 **Is this specific data owned by this user?**

A student with role `Student` should NOT automatically be allowed to:

- Read other students’ profiles
- View other students’ grades
- Modify records that are not theirs

Yet many systems stop at roles — and that is dangerous.

---

## 🔹 The Real-World Problem: Horizontal Privilege Escalation

This type of vulnerability is called **Horizontal Privilege Escalation**.

It means:

- The attacker does **not** become admin
- The attacker stays in the same role
- But accesses data belonging to **other users**

Example:

- Student with `Id = 3` logs in
- Changes the URL from `/api/Students/3` to `/api/Students/4`
- API returns another student’s data

📌 This is one of the **most common API security failures**.

---

## 🔹 Real-Life Analogy

Think of a university portal:

- All students are `Student`
- But each student can only see **their own grades**

The rule is not just:

> “You are a student”

The rule is:

> “You are **this** student”

That difference is ownership.

---

## 🔹 What Is Ownership-Based Authorization?

Ownership-based authorization means:

- The API checks **who the data belongs to**
- The API compares the **authenticated user identity** with the **resource owner**
- Access is granted only if the user owns the data **OR** the user has a higher privilege (Admin)

Ownership is **contextual**, not global.

---

## 🔹 Why This Is Harder Than Roles

Roles are static:

- Stored in JWT
- Checked with attributes

Ownership is dynamic:

- Depends on request data (route, body)
- Depends on database values
- Must be evaluated at runtime

This is why ownership checks are often skipped — and why systems get breached.

---

## 🔹 How Ownership Will Work in Your Student API

In your API:

- Each student record has an `Id`
- Each JWT contains the authenticated student’s `Id`

The API must enforce this rule:

> A student can only access a record if `JWT.StudentId == Route.StudentId`

Admins are exempt from this rule.

This logic **cannot** be expressed with roles alone.

---

## 🔹 Ownership vs Role — Clear Comparison

Ownership connects **identity** to **data** while roles control **what actions** a user may perform.

---

## 🔹 Where Ownership Fits in the Security Roadmap

Your security layers now look like this:

1. HTTPS + CORS — secure communication
2. Authentication — identity
3. Authorization (Roles) — permissions
4. **Ownership-Based Authorization** — data boundaries ← **you are here**
5. Policies & advanced rules
6. Production hardening

Skipping this step leaves a **huge security hole**.

---

## 🧬 Characteristics of Ownership-Based Authorization

- Requires authentication
- Depends on identity claims
- Depends on resource data
- Prevents horizontal privilege escalation
- Essential for real-world APIs

---

## 🔗 Interconnection

- JWT → provides user identity
- Route data → identifies requested resource
- Ownership logic → compares both
- Authorization → allows or blocks access

Ownership connects **identity** to **data**.

---

## 🛠️ Final Summary of Interconnections

- Roles protect actions
- Ownership protects data
- Authentication without ownership is incomplete
- Most real-world breaches happen here
- This is where APIs become truly secure

---

## 🏁 Conclusion

You now understand:

- ✅ Why roles are not enough
- ✅ What ownership-based authorization means
- ✅ Why this problem is so dangerous
- ✅ Where real-world API security actually begins

Your API is about to move from:

> “Authorized”

to

> **“Correctly authorized.”**

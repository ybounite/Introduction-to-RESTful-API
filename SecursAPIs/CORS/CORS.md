**Who Is Allowed to Call Your API (and Why Browsers Care)**

### 🗝️ Introduction
Now that your API uses **HTTPS**, data is encrypted while traveling.

But here is a surprising truth:

🔐 **HTTPS protects data**
 🚧 **CORS controls access from browsers**

These are **different problems**.

![](https://uploads.teachablecdn.com/attachments/c92d87628fc148c8a078631dd488dcd4.png)

**An API can be:**

- Perfectly encrypted
- Completely open to abuse

🎯 **Outcome**
 By the end of this lesson, you will understand:
 - ✅ What CORS really is?
 - ✅ Why it exists?
 - ✅ When it applies (and when it doesn’t)?
 - ✅ How browsers enforce it?
 - ✅ Why backend developers must care?

###

### 🔹 What Is CORS? (Simple Definition)
**CORS** stands for **Cross-Origin Resource Sharing**.

**In simple words:**

> **CORS is a browser security rule that decides which websites are allowed to call your API.**
📌 CORS is enforced by **browsers**, not by servers.

###

### 🔹 What Is an “Origin”?
![](https://uploads.teachablecdn.com/attachments/4e7c05e924c142858619821ffdc4df80.png)

An **origin** is defined by **three things together**:

🔹 Protocol (http / https)
🔹 Domain (example.com)
🔹 Port (if any)

Example origins:

- `https://localhost:3000`
- `https://studentportal.com`
- `https://studentportal.com:8080`
📌 If **any part changes**, it is a **different origin**.

##

### 🔹 The Problem CORS Solves
Imagine this situation:

- You are logged in to `bank.com`
- You visit a malicious website: `evil-site.com`
- That website secretly calls:

```text
GET https://bank.com/api/account
```
**Without protection:**
 - ❌ Your browser would send the request
 - ❌ Cookies/tokens might be included
 - ❌ Private data could be read

📌 This attack is called **Cross-Site Request Abuse**.

###

### 🔹 Why Browsers Enforce CORS?
Browsers protect **users**, not servers.

So browsers ask:

> “Is this website allowed to read data from that API?”
If not:
 - ❌ Browser blocks the response
 - ❌ JavaScript cannot access the data

📌 The API may still respond — but the browser hides it.

##

### 🔹 Very Important Clarification (Critical)
🚨 **CORS is NOT an API security feature by itself**

CORS:
 - ✅ Restricts browser-based JavaScript
 - ❌ Does NOT stop Postman
 - ❌ Does NOT stop curl
 - ❌ Does NOT stop backend-to-backend calls

📌 CORS is about **browser access**, not total security.

###
**🧠 What does “CORS does NOT stop curl” mean?**

**🔹 What is curl?**

**curl** is a **command-line tool** used to send HTTP requests **directly** to an API.

📌 Think of it as:

> “A browser **without** browser rules.”
Example:

```bash
curl https://api.example.com/students
```
This request:

- ❌ Is NOT coming from a browser
- ❌ Has NO JavaScript environment
- ❌ Has NO CORS enforcement

**🔍 Why CORS does NOT stop curl?**

**🚧 CORS is enforced by browsers only**

CORS is a **browser safety rule**, not a server firewall.

So:

![](https://uploads.teachablecdn.com/attachments/7dc7372198204f9f9f354bd867b07b51.png)

![](https://uploads.teachablecdn.com/attachments/fe47ff73a4e04bca8b33824267558e8c.png)

## 🔹 When Does CORS Apply?
CORS applies when **ALL** of these are true:

- ✅ Request comes from a browser

- ✅ JavaScript is making the request

- ✅ Origin is different

**Examples where CORS applies:**

- React app → API
- Angular app → API
- Vue app → API
**Examples where CORS does NOT apply:**

- Postman
- curl
- Mobile apps
- Backend services

###

### 🔹 Same-Origin vs Cross-Origin
**🟢 Same-Origin**

```text
Frontend: https://app.com
API:      https://app.com
```
✅ Allowed automatically

###
**🔴 Cross-Origin**

```text
Frontend: https://app.com
API:      https://api.com
```
🚧 Browser asks for permission → CORS

##

## 🔹 How CORS Works (High-Level Flow)
![](https://uploads.teachablecdn.com/attachments/c8fad4a5441b448cb076085fcc10a3c5.png)

1️⃣ Browser sends request
2️⃣ API responds with CORS headers
3️⃣ Browser checks origin
4️⃣ Browser decides:

- ✅ Allow JavaScript access
- ❌ Block JavaScript access
📌 The server **declares rules**
📌 The browser **enforces them**

##

## 🔹 Common CORS Headers (Conceptual)

- `Access-Control-Allow-Origin`
- `Access-Control-Allow-Methods`
- `Access-Control-Allow-Headers`
📌 These headers tell the browser **what is allowed**.

##

## 🔹 Mental Model (Very Important)
![](https://uploads.teachablecdn.com/attachments/c13fcc5604194b29baa68d03426c68d3.png)

### 🏢 CORS = Building Security Desk

- Anyone can **walk near** the building (API is public)
- But only approved visitors can **enter certain rooms**
- The security guard checks **where you came from**
📌 The guard does NOT stop thieves outside —
 it controls **who is allowed inside from the lobby** (browser).

##

## ❌ Common Beginner Myths
- ❌ “CORS secures my API”
- ❌ “If CORS blocks it, it’s safe”
- ❌ “Postman not blocked = CORS broken”

📌 All false.

##

## 🔹 What CORS Is NOT
- ❌ Authentication
- ❌ Authorization
- ❌ Rate limiting
- ❌ Attack prevention

📌 CORS is **access control for browsers only**.

##

### 🔹 Why Backend Developers Must Understand CORS?
Because:
 - 🔹 Frontend apps depend on it
 - 🔹 Wrong configuration breaks apps
 - 🔹 Overly open CORS exposes users
 - 🔹 Misunderstanding leads to false security

📌 CORS mistakes are extremely common.

##

## 🔗 Interconnection
🔗 HTTPS → encrypts transport
🔗 CORS → controls browser access
🔗 Authentication → identifies users
🔗 Authorization → controls actions

📌 Each layer solves a **different problem**.

##
**🛠️ Summary of Interconnections**

🔹 HTTPS protects data in transit
🔹 CORS protects users from malicious websites
🔹 Authentication proves identity
🔹 Authorization enforces rules

🔹 Security is layered, not single-feature.

##

### 🏁 Conclusion
CORS is not magic.
 It is not optional.
 It is not security by itself.

🚧 **CORS is a browser safety mechanism.**

✅ It protects users
✅ It controls who can read API responses
 ❌ It does not secure your API alone

📌 From now on, remember this rule:

> **If the request does not come from a browser, CORS does not apply.**


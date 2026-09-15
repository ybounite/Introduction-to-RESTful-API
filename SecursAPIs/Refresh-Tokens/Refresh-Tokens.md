# Token Expiration & Refresh Tokens

## 🗝️ Introduction

So far in this course, your Student API does something very important:

- It authenticates users using JWT.
- It enforces identity and permissions.
- It protects endpoints correctly.

At this stage, many developers think:

> “Authentication is done.”

But in real systems, authentication is not a one-time event.
It is a lifecycle.

![](https://uploads.teachablecdn.com/attachments/5119f10c60bc4409bd971a2ab10b52aa.png)

This lesson explains why tokens must expire, what problem refresh tokens solve, and how logout works in stateless APIs before writing any code.

## 🔍 Why This Lesson Exists

JWT authentication works, but without expiration and a refresh strategy, it becomes dangerous.

### The core problems

- JWTs are self-contained.
- Once issued, they are valid until they expire.
- The server does not track sessions.

This leads to two risks:

- Long-lived tokens are dangerous.
- Short-lived tokens hurt usability.

This lesson exists to solve that tension.

## ❓ Why JWTs Must Expire

Every JWT has an expiration time (`exp` claim).

This is not optional.

### Why expiration is mandatory

- Tokens can be stolen from a browser, log file, memory, or proxy.
- Tokens can be leaked accidentally.
- Tokens cannot be revoked easily in stateless systems.

If a token never expires:

- Anyone holding it has permanent access.
- Logout becomes meaningless.
- Stolen tokens can be reused forever.

📌 Expiration limits damage.
It does not prevent theft, but it limits how long theft is useful.

## ⚠️ The Problem with Long-Lived Tokens

Imagine issuing a JWT that lasts:

- 30 days
- 90 days
- Or never expires

What happens if:

- A student logs in on a shared computer?
- A token is copied from browser storage?
- A token appears in logs or error messages?

The attacker does not need:

- A password
- Login access
- Brute force

They already have access.

📌 Long-lived tokens turn one mistake into long-term compromise.

## 😣 The Problem with Very Short-Lived Tokens

Now imagine the opposite:

- The token expires every 5 minutes.

Security is improved, but:

- Users are logged out frequently.
- Applications break when tokens expire.
- Clients must log in constantly.

This creates a usability problem.

📌 This is the core tradeoff:

- Security vs usability

## 🧩 The Concept of Refresh Tokens

Refresh tokens exist to solve this exact problem.

![](https://uploads.teachablecdn.com/attachments/1563a2a7f44f4b56859a9814a085eb30.png)

### The idea is simple

- Use short-lived access tokens.
- Use longer-lived refresh tokens.
- Never use refresh tokens to access APIs directly.

### Roles of each token

- **Access token**
  - Short lifespan
  - Sent with every API request
  - If stolen, the damage is limited
- **Refresh token**
  - Long lifespan
  - Used only to request a new access token
  - Stored more securely
  - Rotated and controlled

📌 Access token = permission to act.
📌 Refresh token = ability to renew permission.

Think of it like this:

- 🔑 Access token -> “I may enter this room”
- 🧾 Refresh token -> “I may request a new key”

### 🧠 What “accessing APIs directly” means

Accessing an API directly means calling business endpoints like:

- `GET /students`
- `POST /orders`
- `PUT /profile`
- `DELETE /courses/5`

These endpoints do real work and return real data.

Normally, you access them like this:

```http
Authorization: Bearer <ACCESS_TOKEN>
```

### ❌ What you must never do

You must never send a refresh token like this:

```http
Authorization: Bearer <REFRESH_TOKEN>
```

to:

- `/students`
- `/orders`
- `/payments`
- `/anything-business-related`

That is what using a refresh token to access APIs directly means, and it is wrong by design.

### ✅ What refresh tokens are allowed to do

A refresh token has one job only:

🔁 Request a new access token

Example:

```http
POST /auth/refresh
Body:
{
	"refreshToken": "xyz..."
}
```

That is it.

No data access.
No business logic.
No permissions.

### 🧩 Why this rule exists

#### 🔥 Refresh tokens are powerful

They live much longer than access tokens.

If you allow them to access APIs:

- An attacker gets long-term access.
- Token expiration becomes meaningless.
- Your system becomes impossible to contain.

That would destroy the whole security model.

### 🛡️ The security separation

![](https://uploads.teachablecdn.com/attachments/d2d4c093c33b4abea1602b6a83097add.png)

Think of it like this:

- 🔑 Access token -> “I may enter this room”
- 🧾 Refresh token -> “I may request a new key”

You never open doors with the receipt.

### 📌 Why access tokens can be used everywhere

- Short lifespan
- Limited damage if stolen
- Designed for frequent exposure
- Safe to send on every request

That is why middleware accepts only access tokens.

### 📌 Why refresh tokens must be protected

- Long lifespan
- Rarely used
- Stored more securely
- Rotated and revoked

They are not meant to travel around your API.

### 🧠 One-sentence mental model

📌 Access token = permission to act.
📌 Refresh token = permission to renew permission.

Or even simpler:

> Access tokens do work.
> Refresh tokens only ask for new tools.

## 🔁 What Is Refresh Token Rotation?

![](https://uploads.teachablecdn.com/attachments/ff89603a6810441395e810cbc6be7a34.png)

Refresh token rotation means:

- Every time a refresh token is used,
- It is replaced with a new one,
- The old one becomes invalid.

Why this matters:

- If a refresh token is stolen and reused,
- The system can detect reuse,
- The session can be invalidated.

📌 Rotation turns silent theft into a detectable event.

## 🚪 How Logout Works in Stateless Systems

![](https://uploads.teachablecdn.com/attachments/d31d4ab0f8084d2da44b04e764f5eb78.png)

In traditional session-based systems:

- Logout = delete the session from the server.

In JWT-based systems:

- The server does not store sessions.
- Tokens are already issued.

So logout means:

- Invalidate the refresh token.
- Let the access token expire naturally.

📌 Logout is not about deleting tokens.
📌 It is about cutting the renewal path.

## 🔁 How a Refresh Token Renews an Access Token

Here is the exact mechanics of how refresh tokens work in real systems: no magic, just a controlled exchange.

![](https://uploads.teachablecdn.com/attachments/b6064d8a7cc3489ba13299b55637153f.png)

### 🧠 The one-sentence idea

When the access token expires, the client presents a refresh token to a special endpoint, and the server issues a brand-new access token, and usually a new refresh token.

### 1️⃣ Access Token Expires

The client makes an API call with an expired access token.

- Server checks `exp`
- Token is expired
- Server responds with `401 Unauthorized`

This is expected and healthy.

### 2️⃣ Client Calls the Refresh Endpoint

Instead of asking the user to log in again, the client sends the refresh token to a single, dedicated endpoint:

```http
POST /auth/refresh
```

Payload, or cookie depending on your design:

```json
{
  "refreshToken": "abc123..."
}
```

🚫 This endpoint does not access business data.

### 3️⃣ Server Validates the Refresh Token

The server performs strict checks:

- Is the refresh token known and stored?
- Is it not expired?
- Is it not revoked?
- Has it not been used before? (rotation check)

If any check fails, deny the request and force re-login.

### 4️⃣ Server Issues New Tokens

If validation succeeds:

- ✅ Create a new access token with a short lifespan.
- 🔁 Create a new refresh token for rotation.
- ❌ Invalidate the old refresh token.

Response:

```json
{
  "accessToken": "new.jwt.access",
  "refreshToken": "new.refresh.token"
}
```

📌 This is silent re-authentication.

### 5️⃣ Client Stores New Tokens

The client replaces the old tokens:

- Access token -> used for API calls
- Refresh token -> stored securely for the next renewal

The user notices nothing, no login screen.

### 🔐 Why This Is Secure

- Access tokens expire quickly, so damage is limited.
- Refresh tokens are rarely sent, so exposure is reduced.
- Rotation detects token theft.
- Logout revokes the refresh token, so renewal stops.

### 📌 Two Rules to Remember

📌 Access tokens do work.
📌 Refresh tokens renew work permission.

Or simply:

> Access token opens doors.
> Refresh token asks for a new key.

## 🏁 Conclusion

Your API already knows:

- Who the user is.
- What they can do.

Now it starts learning:

- How long trust should last
- How to recover safely
- How to limit damage when things go wrong

This is the first step into real production security thinking.

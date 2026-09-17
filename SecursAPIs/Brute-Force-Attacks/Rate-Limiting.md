# Rate Limiting - Protecting Login and Refresh Endpoints from Abuse

#### Concept Lesson

## Introduction

Your Student API is now secure by design:

- Authentication
- JWT
- Roles
- Ownership rules
- Refresh tokens

But security is not only about correctness.

Even a perfectly designed authentication system can be destroyed by abuse.

What if an attacker sends 1,000 login requests per minute?

This lesson introduces rate limiting, a critical security layer that protects your API from:

- Brute-force attacks
- Credential stuffing
- Resource exhaustion

Without rate limiting, your API can be technically correct and practically unusable.

## What Is Rate Limiting?

![What is rate limiting](https://uploads.teachablecdn.com/attachments/4304e492525c4733af7de2ff949b08e9.png)

Rate limiting means restricting how many requests a client can make within a time window.

Example:

- Max 5 login attempts per minute per IP

If the limit is exceeded:

- Requests are blocked automatically

The request never reaches your controller logic.

## Why Authentication Endpoints Are High-Risk

![Authentication endpoints are high-risk](https://uploads.teachablecdn.com/attachments/97ae0d7e12f54baa9ad22c18b1862b51.png)

The most attacked endpoints in any real system are:

- `POST /api/Auth/Login`
- `POST /api/Auth/Refresh`

Why?

- They accept credentials or secrets
- They are publicly exposed
- They can be brute-forced
- They are cheap to attack

We never leave these endpoints unprotected.

## What Rate Limiting Protects Against

![What rate limiting protects against](https://uploads.teachablecdn.com/attachments/99e9316118dd4098b2a896aea04df6b4.png)

### Brute Force

Guessing passwords repeatedly.

Rate limiting:

- Slows attackers dramatically
- Makes guessing impractical

### Credential Stuffing

Trying leaked email and password combinations.

Rate limiting:

- Blocks automated scripts
- Limits attempts per source

### Denial of Service

Overloading authentication endpoints.

Rate limiting:

- Preserves CPU and memory
- Keeps the API responsive

## What Rate Limiting Does Not Do

Rate limiting does not:

- Identify users
- Replace authentication
- Fix weak passwords

It is a protective shield, not an identity mechanism.

## How Rate Limiting Works

![How rate limiting works](https://uploads.teachablecdn.com/attachments/e65762a6133c4822951609cfcca6baff.png)

A typical rate limit rule includes:

- Max requests: 5
- Time window: 1 minute
- Scope: per IP or per client

If the limit is exceeded:

- Request is blocked
- API returns `429 Too Many Requests`

This happens before controller logic executes.

![Rate limiting response](https://uploads.teachablecdn.com/attachments/983dd0a636c242c3afbe7759c439de02.png)

## Step 1: Decide Which Endpoints to Protect

In your Student API, protect high-risk endpoints:

- `POST /api/Auth/Login`
- `POST /api/Auth/Refresh`

Normal Student endpoints do not need aggressive limits.

## Step 2: Decide Rate Limit Strategy

Common strategies:

- Per IP address
- Per user after authentication
- Per endpoint

For login and refresh, per IP is the best starting point.

Why?

- User is not authenticated yet
- IP is the only reliable identifier

## Step 3: Apply Rate Limiting Middleware

Rate limiting is usually applied:

- Globally, with rules per endpoint
- Selectively on specific routes

The middleware checks:

- Client identity, such as IP
- Request count
- Time window

## Step 4: Client Experience When Limited

When a client exceeds the limit:

- Request blocked
- Status code: `429 Too Many Requests`
- Optional message: "Too many login attempts. Please try again later."

Do not reveal internal limits.

## Characteristics

- Protects authentication endpoints
- Reduces brute-force risk
- Improves system stability
- Works before controller logic
- Essential for public APIs

## Interconnection

Login verifies identity.

Refresh maintains session continuity.

Rate limiting prevents abuse.

Each layer solves a different problem.

## Summary of Interconnections

- Authentication proves identity, while rate limiting prevents abuse
- Secure credentials still need request limits
- Defense in depth is mandatory

Security is layered, not singular.

## Conclusion

You have now understood:

- Why authentication endpoints are special
- Why rate limiting is mandatory
- How abuse happens even with correct security
- Where rate limiting fits in the system

Your authentication system is now designed for reality, not just correctness.

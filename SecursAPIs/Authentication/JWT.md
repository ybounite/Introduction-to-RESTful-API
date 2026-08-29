# JWT Explained (Student Identity Analogy)

## The Digital ID Card of Your Student API

### Introduction

You already know:

- What authentication is
- Why your Student API needs it
- That authentication answers, “Who are you?”

Now we answer the next beginner question:

> What is a JWT, really?

> [!IMPORTANT]
> This lesson is 100% conceptual.
>
> - No configuration
> - No middleware
> - No code yet
>
> If you do not understand JWT conceptually, the code will feel like magic.

## What Is a JWT?

JWT stands for:

### JSON Web Token

In simple words:

> A JWT is a digital ID card that the API gives to a student after login.

The student carries this ID card and shows it with every request.

## Real-Life Analogy

Think of entering a university:

1. You show your ID once at login
2. The university gives you a badge
3. You show the badge at every door

> [!TIP]
> The badge:
>
> - Identifies who you are
> - Cannot be easily forged
> - Can expire
>
> That badge is the JWT.

## Why APIs Use JWT?

JWT is popular because it is:

- Stateless — the server does not store sessions
- Self-contained — identity information is inside the token
- Fast — no database lookup per request
- Widely supported

This makes it perfect for REST APIs like your Student API.

## What Information Does a JWT Carry?

A JWT usually contains:

- Student ID
- Student role (`Student` / `Admin`)
- Token expiration time

> [!WARNING]
> A JWT does not contain:
>
> - Passwords
> - Sensitive personal data

## JWT Structure — Header, Payload & Signature Explained

### Introduction

Now we answer the next important question:

> What is inside a JWT, and why does each part matter?

By the end of this lesson, you will:

- Understand every part of a JWT
- Know what information belongs where
- Stop seeing JWT as a “black box”

> [!IMPORTANT]
>
> - No middleware
> - No configuration
> - No coding yet
>
> Understanding comes before implementation.

### The JWT Format (One Line)

A JWT always looks like this:

![JWT structure](https://uploads.teachablecdn.com/attachments/87a32295d6b140aaac67bc289346744e.png)

```text
Header.Payload.Signature
```

Three parts, separated by dots.

Each part has a specific job.

### Part 1: Header (How the Token Is Built)

#### What Is the Header?

The header describes how the token is created.

It usually contains:

- the token type (`JWT`)
- the signing algorithm, such as `HS256`

Example (conceptual):

```json
{
  "typ": "JWT",
  "alg": "HS256"
}
```

> [!NOTE]
> The header does not contain user data.

### Part 2: Payload (Who the Student Is)

#### What Is the Payload?

The payload contains the claims.

Claims are pieces of information about the user.

In your Student API, typical claims will be:

- Student ID
- Role (`Student` / `Admin`)
- Token expiration time

Example (conceptual):

```json
{
  "studentId": 5,
  "role": "Admin",
  "exp": 1700000000
}
```

> [!IMPORTANT]
> This is the identity card part of the JWT.

#### Important Rule About the Payload

The payload is:

- not encrypted
- only encoded

That means:

- anyone can read it
- but no one can safely change it without breaking the signature

> [!WARNING]
> Never store passwords or sensitive personal data in the payload.

### Part 3: Signature (Why the Token Can Be Trusted)

#### What Is the Signature?

The signature proves that:

- the token was created by your API
- the payload was not modified

It is created using:

- the header
- the payload
- a secret key known only to the server

> [!IMPORTANT]
> If anything changes, the signature validation fails and the API rejects the request.

#### Why the Signature Is the Most Important Part

Without the signature:

- anyone could change `role` from `"Student"` to `"Admin"`
- anyone could impersonate another student

> [!WARNING]
> The signature prevents forgery.

### JWT Trust Flow (Very Important)

![JWT trust flow](https://uploads.teachablecdn.com/attachments/ced8284c127a450ab98b3adc0c9f907f.png)

When a request arrives with a JWT:

1. The API reads the token
2. The API checks the signature
3. The API checks expiration
4. The API extracts the claims
5. The API trusts the identity

> [!NOTE]
> If any step fails, the request is rejected.

## What Makes JWT Trustworthy?

The signature is the key.

- The server signs the token
- The client cannot change it
- The API can verify it

If someone modifies the token:

- the signature becomes invalid
- the API rejects the request

## How JWT Will Be Used in Our Student API

The flow will be:

1. Student logs in
2. API returns a JWT
3. Client sends JWT with requests
4. API validates JWT
5. API knows:
   - Who the student is
   - What role they have

> [!EXAMPLE]
> This applies to endpoints such as:
>
> - `POST /api/Students`
> - `GET /api/Students/{id}`
> - `DELETE /api/Students/{id}`

## What JWT Does Not Do

JWT does not:

- decide permissions
- protect against abuse
- replace HTTPS

JWT only answers:

> Who is this request coming from?

## Characteristics

- Stateless authentication
- Token-based
- Self-contained identity
- Requires HTTPS
- Foundation for roles and policies

## Interconnection

- HTTPS → protects the token
- JWT → carries identity
- Roles and policies → use JWT data

## Summary of Interconnections

- Secure transport → safe token usage
- JWT provides identity → authorization uses it
- Authentication precedes authorization

## Conclusion

You now understand:

- What a JWT really is
- Why it exists
- How it fits into your Student API
- The structure of a JWT
- The role of each part in building trust

> [!NOTE]
> In the next lesson, we will move from theory to practice and explain how to generate a JWT for a student after login.

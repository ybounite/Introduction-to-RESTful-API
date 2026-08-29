# Authentication Fundamentals

## Why Authentication Is Needed Now?

### Introduction

So far, we have achieved two important things:

- Data travels securely with HTTPS
- Browser access is controlled with CORS

But the API still has one huge blind spot:

> It does not know who is making the request.

In this lesson, we will not write JWT code yet.

Instead, we will build the mental model of authentication.

This lesson answers one question only:

> How does an API identify the caller?

---

### What Is Authentication?

![Authentication concept](https://uploads.teachablecdn.com/attachments/2ebd6f04547e4610a03dae0a70d08bba.png)

Authentication answers this question:

> Who are you?

It does not answer:

- What can you do?
- Are you allowed to do this?

Those are authorization questions, and they come later.

---

### Authentication in Real Life (Analogy)

Think about entering a university:

- Gate security checks your ID
- They do not decide which office you can enter
- They only confirm your identity

> Authentication = identity verification

---

### Why Your Student API Needs Authentication

Look at your existing endpoints:

- `POST /api/Students`
- `PUT /api/Students/{id}`
- `DELETE /api/Students/{id}`

Right now:

- Anyone can call them
- The API trusts every request

Authentication allows the API to say:

> I know who you are — now I can decide what you’re allowed to do.

---

### Common Authentication Approaches (High-Level)

![Authentication approaches](https://uploads.teachablecdn.com/attachments/962a60254cad4d5192c6993420c874ba.png)

APIs usually use one of these:

- Username + Password -> Token
- API Keys
- OAuth (Google, Microsoft, etc.)

In this course, we will use:

> Username + Password -> JWT Token

Because it is:

- Beginner-friendly
- Stateless
- Widely used
- Perfect for REST APIs

---

### Stateless Authentication (Important Concept)

Your Student API is stateless, which means:

- The server does not remember users between requests
- Every request must prove identity again

JWT solves this by:

- Carrying identity info inside the token
- Sending that token with every request

![JWT concept](https://uploads.teachablecdn.com/attachments/313292c7d947433eb8ec0fc8ac8492a8.png)

---

### What Authentication Will Look Like in Our API

We will add new endpoints without changing the existing Student endpoints:

- `POST /api/Auth/Register`
- `POST /api/Auth/Login`

![Auth flow](https://uploads.teachablecdn.com/attachments/a33721ab4ad9414a846592f47d705978.png)

Flow:

1. Student logs in
2. API verifies credentials
3. API returns a token
4. Client sends the token with requests

---

### What Authentication Does NOT Do

Authentication does NOT:

- Decide if you are an admin
- Decide which student you can access
- Protect business rules

> That comes in authorization lessons.

---

### Characteristics

- Authentication identifies users
- It does not grant permissions
- It is stateless by design
- It is required before authorization
- It is the foundation for all advanced security

---

## Interconnection

- HTTPS -> safe credential transfer
- CORS -> safe browser access
- Authentication -> identity
- Authorization -> permissions (next lessons)

### Summary of Interconnections

- Secure transport -> safe login
- Identity -> permission decisions
- Authentication always comes before authorization

---

## Conclusion

You now clearly understand:

- What authentication is
- Why your Student API needs it
- Why we do not jump directly into JWT code

> In the next lesson, we will finally open the JWT box and answer: What is a JWT and how does it represent a student identity?

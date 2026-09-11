## What Is Middleware?

**Middleware** is:

A piece of code that stands between an incoming **Request** from the client and the **Controller / Endpoint** in the API.

In other words, every request that enters the API passes through a series of Middleware components in order. Then it reaches the Controller, and the response travels back through the same Middleware pipeline in reverse order.

## The Actual Path of a Request

A request does not go directly to the Controller.

The correct flow is:

**Request → Middleware 1 → Middleware 2 → Middleware 3 → Controller → Middleware 3 → Middleware 2 → Middleware 1 → Response**

The Controller is part of the pipeline, but Middleware acts as the **guard and organizer of the entire request pipeline**.

## What Does Middleware Do?

Middleware is responsible for general and shared tasks such as:

- 🔹 JWT validation
- 🔹 Checking whether the user is authenticated (**Authentication**)
- 🔹 Checking permissions (**Authorization**)
- 🔹 HTTPS redirection
- 🔹 CORS
- 🔹 Logging
- 🔹 Rate limiting
- 🔹 Exception handling

## An Important Point

The **Controller does not validate the token by itself**.

The Middleware handles authentication and authorization before the request reaches the Controller.

For example, when we write:

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

these are **Middleware components**, not Controllers.

This means that for every incoming request, before it reaches an Endpoint, ASP.NET Core can check:

- 🔹 Is there a token?
- 🔹 Is the token valid?
- 🔹 Has the token expired?
- 🔹 Is the user's role allowed?

If authentication or authorization fails, the request can be rejected **before the Controller action executes at all**.

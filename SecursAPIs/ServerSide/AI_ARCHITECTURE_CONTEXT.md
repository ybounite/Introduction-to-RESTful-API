StudentApi Architecture Refactoring — Deep Understanding Mode

I am working on an ASP.NET Core Web API project called StudentApi.

The solution has three projects:

StudentApi
StudentApiBusinessLayer
StudentDataAccessLayer


My goal is to refactor the backend into a clean, professional architecture one feature/endpoint at a time, while understanding why the architecture is designed this way.

Very important mentoring instruction

Act as a senior ASP.NET Core backend developer and mentor.

I do NOT want you to simply give me code to copy and paste.

For every architectural change, I want you to teach me the reasoning first.

Before showing code, explain:

What problem are we solving?
Why does this responsibility belong in this layer?
What is the dependency direction?
What is the responsibility of each class/interface?
How does the request flow through the application?
Why are we choosing this design instead of another design?
What would be wrong with putting this logic in another layer?
Only then show the code.

Do not move to the next step until I understand and confirm the current step.

Architecture

The desired dependency direction is:

StudentApi
    ↓
StudentApiBusinessLayer
    ↓
StudentDataAccessLayer
    ↓
SQL Server


Very important:

StudentDataAccessLayer
        X
StudentApiBusinessLayer


The Data Access Layer must never reference the Business Layer.

The Business Layer may depend on Data Access abstractions such as repositories.

The API layer should depend on Business Layer abstractions such as services.

Current Student architecture

The Student CRUD endpoints have already been refactored.

Current architecture:

StudentsController
        ↓
IStudentService
        ↓
StudentService
        ↓
IStudentRepository
        ↓
StudentRepository
        ↓
SQL Server


The following endpoints are already working:

GET    /api/students/All
GET    /api/students/Passed
GET    /api/students/AverageGrade
GET    /api/students/{studentId}
POST   /api/students/AddStudent
DELETE /api/students/{studentId}
PUT    /api/students/Update/{studentId}


Do not unnecessarily change these working endpoints.

Authentication architecture

The authentication system already contains:

AuthController
AuthService
IAuthService
JwtService
JwtSettings


Current login flow is approximately:

AuthController
      ↓
IAuthService
      ↓
AuthService
      ↓
existing authentication/data-access logic
      ↓
JwtService
      ↓
JWT Access Token


Login already works.

JWT generation already works.

Do not unnecessarily rewrite the working JWT generation code.

New goal: Refresh Tokens

We are now learning and implementing:

Access Token Expiration
+
Refresh Tokens


The goal is eventually to have:

Client
   ↓
Login
   ↓
AuthService
   ↓
JwtService
   ↓
Short-lived Access Token
+
Longer-lived Refresh Token


When the access token expires:

Client
   ↓
Refresh endpoint
   ↓
AuthService
   ↓
RefreshTokenRepository
   ↓
SQL Server
   ↓
validate refresh token
   ↓
JwtService
   ↓
new Access Token

Current RefreshToken model

I currently have:

StudentDataAccessLayer/Models/RefreshToken.cs


with:

namespace StudentDataAccessLayer.Models;

public class RefreshToken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string TokenHash { get; set; } = string.Empty;

    public DateTime RefreshTokenExpiresAt { get; set; }

    public DateTime? RefreshTokenRevokeAt { get; set; }
}


Before changing this model, explain what each property represents and whether the naming/design is appropriate.

In particular, explain:

Id
UserId
TokenHash
RefreshTokenExpiresAt
RefreshTokenRevokeAt

Also explain why we may store a hash of the refresh token rather than the raw refresh token.

Current repository interface

I currently have:

StudentDataAccessLayer/Interfaces/IRefreshTokenRepository.cs


with:

using StudentDataAccessLayer.Models;

namespace StudentDataAccessLayer.Interfaces;

public interface IRefreshTokenRepository
{
    Task CreateAsync(RefreshToken refreshToken);

    //Task<RefreshToken?> GetByTokenHashAsync(string TokenHash);
    //Task RevokeAsync(int refreshTokenId, DateTime revokedAt);
}


Do not immediately add more methods.

First explain the purpose of the repository abstraction.

Explain:

IRefreshTokenRepository
        ↓
RefreshTokenRepository
        ↓
SQL Server


Explain why AuthService should depend on IRefreshTokenRepository instead of directly using SqlConnection.

Current repository implementation

I currently have:

StudentDataAccessLayer/Repositories/RefreshTokenRepository.cs


with approximately:

using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudentDataAccessLayer.Interfaces;
using StudentDataAccessLayer.Models;

namespace StudentDataAccessLayer.RefreshTokenRepository;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly string _connectionString = string.Empty;

    public RefreshTokenRepository(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "DefaultConnection was not found."
            );
    }

    public async Task CreateAsync(RefreshToken refreshToken)
    {
        await using var connection =
            new SqlConnection(_connectionString);

        await using var command = new SqlCommand(
            @"INSERT INTO RefreshToken(
                UserId,
                TokenHash,
                RefreshTokenExpiresAt,
                RefreshTokenRevokedAt
              )
              VALUES(
                @UserId,
                @TokenHash,
                @RefreshTokenExpiresAt,
                @RefreshTokenRevokedAt
              )",
            connection
        );

        command.Parameters.AddWithValue(
            "@UserId",
            refreshToken.UserId);

        command.Parameters.AddWithValue(
            "@TokenHash",
            refreshToken.TokenHash);

        command.Parameters.AddWithValue(
            "@RefreshTokenExpiresAt",
            refreshToken.RefreshTokenExpiresAt);

        command.Parameters.AddWithValue(
            "@RefreshTokenRevokeAt",
            (object?)refreshToken.RefreshTokenRevokeAt
                ?? DBNull.Value);

        await connection.OpenAsync();

        await command.ExecuteNonQueryAsync();
    }
}


There may currently be naming inconsistencies between:

RefreshTokenRevokeAt


and:

RefreshTokenRevokedAt


Do not silently fix these.

First explain the problem and ask me to verify the actual SQL table/column names.

Also notice that the namespace currently appears to be:

namespace StudentDataAccessLayer.RefreshTokenRepository;


while the class is physically located under:

Repositories/RefreshTokenRepository.cs


Explain whether the namespace should instead be:

namespace StudentDataAccessLayer.Repositories;


and why namespace/folder organization matters for maintainability.

Important architecture rule

We are using Dependency Injection.

Do not introduce static initialization patterns such as:

RefreshTokenRepository.Initialize(...)


Repositories should receive required infrastructure through constructors.

For example:

ASP.NET Core DI
      ↓
RefreshTokenRepository
      ↓
IConfiguration
      ↓
Connection String


Explain this rather than just implementing it.

SQL Server

The project already uses SQL Server.

Do not introduce Entity Framework Core unless there is a strong architectural reason.

The existing application uses ADO.NET and stored procedures/raw SQL.

Continue using the existing approach unless we explicitly decide otherwise.

Do not unnecessarily replace working SQL with a new ORM.

Refresh-token architecture we want to learn

Eventually we want to understand these responsibilities:

Controller

Responsible for:

HTTP request
HTTP response
model validation
status codes


It should NOT:

generate refresh tokens
hash tokens
query SQL Server
generate JWTs

Business Layer / AuthService

Responsible for authentication business rules:

generate refresh token
hash refresh token
decide refresh-token expiration
validate refresh token
check expiration
check revocation
generate new access token
rotate/revoke refresh tokens


It should use:

IRefreshTokenRepository
JwtService


It should NOT directly use:

SqlConnection
SqlCommand

JwtService

Responsible for JWT creation.

It should generate the short-lived access token.

It should NOT become responsible for database persistence of refresh tokens.

RefreshTokenRepository

Responsible for database operations:

INSERT refresh token
SELECT refresh token
UPDATE/revoke refresh token


It should not decide authentication business rules.

It should not generate JWTs.

It should not hash tokens unless we explicitly decide that hashing belongs there for a specific reason.

Learning approach

For every step, use this order:

1. Concept

Explain the concept in simple terms.

2. Architecture

Show the flow:

Controller
    ↓
Service
    ↓
Repository
    ↓
Database

3. Responsibility

Explain what each layer does and does not do.

4. Design decision

Explain why we're choosing this design.

5. Code

Only after the explanation, provide the code.

6. Build/test

Tell me exactly what to build/test.

7. Stop

Wait for my confirmation before moving to the next step.

Current exact stopping point

Do NOT start implementing the refresh endpoint yet.

Do NOT modify AuthService yet.

Do NOT create more repository methods yet.

Do NOT create a refresh controller endpoint yet.

First, help me understand the architecture of the code I already created:

IRefreshTokenRepository
RefreshToken
RefreshTokenRepository


Explain:

Why do we have an interface?
Why do we have a model?
Why do we have a repository?
Why does the repository use IConfiguration?
Why does AuthService eventually depend on the interface?
Why shouldn't AuthService directly use SqlConnection?
Why should SQL/database code stay inside the Data Access Layer?
Why should refresh-token business rules stay inside the Business Layer?


Then review my current implementation for architectural problems.

Do not rewrite everything immediately.

Identify the problems first.

Then we will fix one problem at a time.

The goal is not just to make the code work.

The goal is for me to understand why the architecture works and be able to design the next feature myself.
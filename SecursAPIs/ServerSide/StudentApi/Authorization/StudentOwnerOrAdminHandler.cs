using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using StudentApi.Authorization.Requirements;

namespace StudentApi.Authorization.Handlers;

// This authorization handler enforces the ownership rule for student resources.
// It checks whether the current user is either:
// - An Admin (full access), OR
// - The owner of the student record being requested
public class StudentOwnershipHandler 
	: AuthorizationHandler<StudentOwnerOrAdminRequirement>
{
	//- context : contains authenticated user and authorization state
	// - requirement : represents the ownership rule (Owner OR Admin)
	// - studentId : resource bieng protected (route parameter)
	protected override Task HandleRequirementAsync(
			AuthorizationHandlerContext context,
			StudentOwnerOrAdminRequirement requirement)
			//int studentId)
	{
		// Firdt rule Admin override
		// If the authenticated user has Admin role.
		// they are allowed to access any student record.
		if (context.User.IsInRole("Admin"))
		{
			//Mark the repuirement as satisfied
			context.Succeed(requirement);
			return Task.CompletedTask;
		}

		// Second rule : Owership check
		// Extract the authenticated user's ID from JWT claims
		// This value was added to the token during login 
		// and valideted by the JWT middleware.
		var userId = 
			context.User.FindFirstValue(ClaimTypes.NameIdentifier);
		// Compare the authenticated user's ID with the requested student ID.
		// IF they match, the user ownrs the ressource.
		if (int.TryParse(userId, out int authencatedSudentId) &&
			context.Resource is HttpContext httpContext)
		{
			var studentIdValue = 
				httpContext.Request.RouteValues["studentId"];
				if (int.TryParse(studentIdValue?.ToString(), out var requestedStudentId)
						&& authencatedSudentId == requestedStudentId)
				{
					// Ownership confirmed, authorization succeds
					context.Succeed(requirement);
				}
		}
		// If neither admin nor owner conditions are met,
		// the requirement is not satisfied and access will be denied.
		// ASP.NET Core will authomatically return 403 Forbidden. 
		return Task.CompletedTask;
	}
}
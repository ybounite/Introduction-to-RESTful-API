using Microsoft.AspNetCore.Authorization;

namespace StudentApi.Authorization.Requirements;
//This class represents the authorization rule itself
// It does Not contain login.
// "Owner OR Admin can access the student resource."
public class StudentOwnerOrAdminRequirement : IAuthorizationRequirement
{
}
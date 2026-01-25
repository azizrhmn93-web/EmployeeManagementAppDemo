using Microsoft.AspNetCore.Authorization;

namespace EmployeeManagement.Security
{
    public class NotSelfUpdateRequirement : IAuthorizationRequirement
    {
    }
}

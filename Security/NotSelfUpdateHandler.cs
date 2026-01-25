using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EmployeeManagement.Security
{
    public class NotSelfUpdateHandler : AuthorizationHandler<NotSelfUpdateRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NotSelfUpdateHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, NotSelfUpdateRequirement requirement)
        {
            // 1. Always allow Super Admin
            if (context.User.IsInRole("Super Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // 2. Check Admin permissions
            bool isAdminWithClaim = context.User.IsInRole("Admin") &&
                                    context.User.HasClaim(c => c.Type == "Update User Claims" && c.Value == "true");

            if (isAdminWithClaim)
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var currentUserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Extract "id" from the route (e.g., /api/users/{id})
                var targetUserId = httpContext?.Request.Query["userId"].ToString();

                // Success if the Admin is NOT editing themselves
                if (currentUserId != null && currentUserId != targetUserId)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}

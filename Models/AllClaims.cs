using System.Security.Claims;

namespace EmployeeManagement.Models
{
    public static class AllClaims
    {
        public static List<Claim> Claims = new List<Claim>
        {
            new Claim("Create Role", "Create Role"),
            new Claim("Edit Role", "Edit Role"),
            new Claim("Delete Role", "Delete Role"),
            new Claim("Create User", "Create User"),
            new Claim("Update User Claims", "Update User Claims")
        };
    }
}

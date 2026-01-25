using EmployeeManagement.Models;
using EmployeeManagement.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    [Authorize(Policy = "EditRolePolicy")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<AppUser> userManager;
        private readonly ILogger<AdministrationController> logger;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, ILogger<AdministrationController> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
        }
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoleAsync(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                IdentityResult result = await roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administration");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = userManager.Users;
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            // var role = roleManager.FindByIdAsync(id).Result;
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }
            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            var usersInRole = await userManager.GetUsersInRoleAsync(role.Name);
            model.Users = usersInRole.Select(u => u.UserName).ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRoleAsync(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }

        /*[HttpGet]
        public async Task<IActionResult> DeleteRoleAsync(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }
            DeleteRoleViewModel model = new DeleteRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };
            return View(model);
        }*/

        [HttpPost]
        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }
 
            else
            {
                try
                {
                    var result = await roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                catch (DbUpdateException ex)
                {
                    logger.LogError("Update Database Error" + ex);
                    TempData["Error"] = "Cannot delete the role " + role.Name + " because it is currently assigned to users. " +
                        "Remove all users from this role first and then delete the role";
                }
                return RedirectToAction("ListRoles");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            // Implementation for editing users in a role
            ViewBag.roleId = roleId;
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            ViewBag.roleName = role.Name;
            var model = new List<UserRoleViewModel>();
            var usersInRole = await userManager.GetUsersInRoleAsync(role.Name);
            var users = userManager.Users;

            foreach (var user in users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                if (usersInRole.Any(u => u.Id == user.Id))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            
            foreach (var userRole in model)
            {
                var user = await userManager.FindByIdAsync(userRole.UserId);
                IdentityResult result = null;
                if (userRole.IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!userRole.IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                if (result.Succeeded)
                {
                    if (model.Any())
                        continue;
                    else
                        return View("EditRole", new {Id = roleId});
                }

            }
            return RedirectToAction("EditRole", new { Id = roleId });
        }

        [HttpGet]
        [Authorize(Policy = "UpdateUserClaimsPolicy")]
        public async Task<IActionResult> EditUserRoles(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.Message = $"User with {userId} cannot be found";
                return View("NotFound");
            }

            var model = new List<EditUserRolesViewModel>();
            var roles = roleManager.Roles.ToList();

            foreach (var role in roles)
            {
                model.Add(new EditUserRolesViewModel()
                {
                    userName = user.UserName,
                    userId = user.Id,
                    roleId = role.Id,
                    roleName = role.Name,
                    isSelected = await userManager.IsInRoleAsync(user, role.Name)
                });
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "UpdateUserClaimsPolicy")]

        public async Task<IActionResult> EditUserRoles(List<EditUserRolesViewModel> model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.Message = $"User with {userId} cannot be found";
                return View("NotFound");
            }

            var currentRoles = await userManager.GetRolesAsync(user);
            var selectedRoles = model.Where(ur => ur.isSelected).Select(role => role.roleName).ToList();

            var rolesToRemove = currentRoles.Except(selectedRoles);
            var rolesToAdd = selectedRoles.Except(currentRoles);

            if(rolesToRemove.Any())
            {
                var removeRolesResult = await userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if(!removeRolesResult.Succeeded)
                {
                    ModelState.AddModelError("", "Failed to remove unselected roles");
                    return View(model);
                }
            }

            if(rolesToAdd.Any())
            {
                var rolesToAddResult = await userManager.AddToRolesAsync(user, rolesToAdd);
                if (!rolesToAddResult.Succeeded)
                {
                    ModelState.AddModelError("", "Failed to remove unselected roles");
                    return View(model);
                }
            }
            return RedirectToAction("ListUsers");
        }

        [HttpGet]
        public async  Task<IActionResult> EditUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.Message = $"User with {userId} cannot be found";
                return View("NotFound");
            }

            var userRoles = await userManager.GetRolesAsync(user);
            var userclaims = await userManager.GetClaimsAsync(user);

            var model = new EditUserViewModel
            {
                Id = userId,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                UserRoles = userRoles,
                Claims = userclaims.Where(v => v.Value == "true").Select(c => c.Type).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.Message = $"User with {model.Id} cannot be found";
                return View("NotFound");
            }

            if (ModelState.IsValid)
            {
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.EmailConfirmed = model.EmailConfirmed;
                user.FullName = model.FullName;

                IdentityResult result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                    return RedirectToAction("ListUsers");

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest();

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = $"User with id = {userId} cannot be found"; return View("NotFound");
            }

            var currentUserId =  userManager.GetUserId(User);
            if (currentUserId == userId)
            {
                TempData["Error"] = "You cannot delete your own account";
                return RedirectToAction("EditUser", new { userId });
            }

            // Prevent deleting the last admin
            if (await userManager.IsInRoleAsync(user, "Admin"))
            {
                var admins = await userManager.GetUsersInRoleAsync("Admin");
                if (admins.Count <= 1)
                {
                    TempData["Error"] = "Cannot delete the last remaining admin account.";
                    return RedirectToAction("EditUser", new { userId });
                }
            }

            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["ToastMessage"] = "User deleted";
                return RedirectToAction("ListUsers");
            }

            TempData["Error"] = "Unable to delete user.";
            return RedirectToAction("EditUser", new { userId });
        }


        [HttpGet]
        [Authorize(Policy = "UpdateUserClaimsPolicy")]
        public async Task<IActionResult> EditUserClaims(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest();

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.Message = $"User with {userId} cannot be found";
                return View("NotFound");
            }

            //existing claims for the model
            var currentClaims = await userManager.GetClaimsAsync(user);

            EditUserClaimsViewModel model = new EditUserClaimsViewModel()
            {
                UserId = user.Id,
                UserName = user.UserName
            };
            foreach (Claim claim in AllClaims.Claims)
            {
                UserClaimViewModel userClaimViewModel = new UserClaimViewModel()
                {
                    ClaimType = claim.Type
                };
                if(currentClaims.Any(c => c.Type == claim.Type && c.Value == "true"))
                    userClaimViewModel.IsSelected = true;

                model.Claims.Add(userClaimViewModel);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize (Policy = "UpdateUserClaimsPolicy")]
        public async Task<IActionResult> UpdateUserClaims(EditUserClaimsViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ViewBag.Message = $"User with {model.UserId} cannot be found";
                return View("NotFound");
            }

            var currentClaims = await userManager.GetClaimsAsync(user);

            var removeClaims = await userManager.RemoveClaimsAsync(user, currentClaims);
            if (!removeClaims.Succeeded)
            {
                ModelState.AddModelError("", "Failed to remove unselected claims");
                return View(model);
            }

            var ClaimsToAdd = model.Claims
                .Select(c => new Claim(c.ClaimType, c.IsSelected ? "true": "false"));

            var addClaims = await userManager.AddClaimsAsync(user, ClaimsToAdd);

            if (!addClaims.Succeeded)
            {
                ModelState.AddModelError("", "Failed to add unselected claims");
                return View(model);
            }
            return RedirectToAction("EditUser", new { userId = model.UserId});
        }

        
    }

}

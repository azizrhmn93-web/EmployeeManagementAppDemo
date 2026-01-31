using EmployeeManagement.Models;
using EmployeeManagement.Services;
using EmployeeManagement.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly ILogger<AccountController> logger;
        private readonly IWebHostEnvironment hostEnvironment;
        private readonly IEmailSender emailSender;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            IConfiguration configuration, ILogger<AccountController> logger, IWebHostEnvironment hostEnvironment,
            IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.logger = logger;
            this.hostEnvironment = hostEnvironment;
            this.emailSender = emailSender;
        }
        [HttpGet]
        public IActionResult RegisterUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUserAsync(RegisterUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Code to register the user goes here
                var user = new AppUser
                {
                    UserName = model.Username ?? model.Email,
                    Email = model.Email,
                    FullName = model.FullName
                };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                        new { userId = user.Id, token = token }, Request.Scheme);

                    var emailSubject = "Confirm Your Email";
                    var emailBody = $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.";
                    await emailSender.SendEmailAsync(user.Email!, emailSubject, emailBody);

                    logger.Log(logLevel: LogLevel.Warning, $"Email Confirmation Link: {confirmationLink}");

                    if (signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Administration");
                    }
                    return View("VerifyEmail");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }
            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                TempData["ToastMessage"] = "User Id and Token are required";
                return RedirectToAction("Index", "Home");
            }
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }
            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View("ConfirmEmail");
            }
            else
            {
                return BadRequest("Error confirming your email.");
            }
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> ValidateEmail(string Email)
        {
            var allowedDomains = configuration
                .GetSection("AllowedEmailDomains")
                .Get<string[]>();

            var domain = Email.Split('@').Last();

            if (!allowedDomains!.Contains(domain, StringComparer.OrdinalIgnoreCase))
            {
                return Json("Email domain is not allowed.");
            }

            var user = await userManager.FindByEmailAsync(Email);
            if (user != null)
            {
                return Json($"Email {Email} is already in use.");
            }

            return Json(true);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Get the list of external login providers for rendering view
            model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Input);

                // Check if the input is an email address
                if (user == null && new EmailAddressAttribute().IsValid(model.Input))
                {
                    user = await userManager.FindByEmailAsync(model.Input);
                }

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                    return View(model);
                }

                if (!user.EmailConfirmed && (await userManager.CheckPasswordAsync(user, model.Password)))
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                        new { userId = user.Id, token = token }, Request.Scheme);

                    var emailSubject = "Confirm Your Email";
                    var emailBody = EmailTemplates.GetHtmlTemplate(
                        "Confirm Email",
                        "We received a request to confirm your email. If you didn't make this request, you can safely ignore this email.",
                        "Confrim Email",
                        confirmationLink!);
                    await emailSender.SendEmailAsync(user.Email!, emailSubject, emailBody);

                    //send a copy to log file too
                    logger.Log(LogLevel.Warning, $"Email Confirmation Link: {confirmationLink}");

                    // Resend the verification email
                    return RedirectToAction("VerifyEmail", new { email = model.Input });
                }

                // Fix: user.UserName is guaranteed non-null here because user is not null
                var result = await signInManager.PasswordSignInAsync(user.UserName!, model.Password, model.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    TempData["ToastMessage"] = "Successfully Logged In";

                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                if(result.IsLockedOut)
                {
                    return View("AccountLocked");
                }
                var triesLeft = 5 - await userManager.GetAccessFailedCountAsync(user);
                ModelState.AddModelError(string.Empty, $"Invalid Login Attempt, you have {triesLeft} tries left");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallBack", "Account", new { ReturnUrl = returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalLoginCallBack(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= Url.Content("~/");
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            // If the external provider returned an error, surface it to the user.
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login", model);
            }

            // Retrieve the external login information.
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                // Could be caused by expired roundtrip or missing cookies. Prompt the user to try again.
                ModelState.AddModelError(string.Empty, "Error loading external login information.");
                return View("Login", model);
            }

            // If the external login is already linked to a local user, sign them in.
            var signInResult = await signInManager.ExternalLoginSignInAsync(
                info.LoginProvider,
                info.ProviderKey,
                isPersistent: false,
                bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                // External login already linked -> user signed in successfully.
                return LocalRedirect(returnUrl);
            }

            // Extract email from the external provider's claims. We require it to identify/create a local user.
            var email = info.Principal?.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                // Without an email claim we cannot create/link an account reliably.
                ModelState.AddModelError(string.Empty, "Email claim not received from: " + info.LoginProvider);
                return View("Login", model);
            }


            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // No local account: create one using info from the external provider.
                // Mark EmailConfirmed = true because we assume the external provider verified the email.
                // Note: This policy can be changed depending on application requirements.
                user = new AppUser
                {
                    Email = email,
                    UserName = email,
                    FullName = info.Principal?.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    foreach (var error in createResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View("Login", model);
                }
            }
            else if (!user.EmailConfirmed)
            {
                //if unconfirmed account exist delete it to prevent security vulnerabilty
                //A malicious user might have created this account using the real owner's email addrss we can't simply confirm it
                //and link it to local account, because the malicious user can access it, because he has the password
                await userManager.DeleteAsync(user);

                user = new AppUser()
                {
                    Email = email,
                    UserName = email,
                    FullName = info.Principal?.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user);
            }

            // Link the external login to the local user. This prevents duplicate accounts on future logins.
            var addLoginResult = await userManager.AddLoginAsync(user, info);
            if (addLoginResult.Succeeded)
            {
                // After linking, perform the external sign-in flow to establish the local authentication cookie.
                // perform the external sign-in because if signinasync were used it would prevent first user from the 2 factor authentication.
                await signInManager.ExternalLoginSignInAsync(
                    info.LoginProvider,
                    info.ProviderKey,
                    isPersistent: false,
                    bypassTwoFactor: true);

                return LocalRedirect(returnUrl);
            }

            // If linking failed, surface an error. Common causes: the external login is already associated
            // with another local account or provider returned conflicting information.
            ModelState.AddModelError(string.Empty, "Failed to link external login.");
            return View("Login", model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && await userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    var resetLink = Url.Action("ResetPassword", "Account", new { email = model.Email, token = token }, Request.Scheme);

                   
                    var emailSubject = "Reset Your Password";
                    var emailBody = EmailTemplates.GetHtmlTemplate(
                        "Reset Password",
                        "We received a request to reset your password. If you didn't make this request, you can safely ignore this email.",
                        "Reset Password",
                        resetLink!);
                    await emailSender.SendEmailAsync(user.Email!, emailSubject, emailBody);
                    
                    //send a copy to log file too
                    logger.Log(LogLevel.Warning, $"Reset Password Link: {resetLink}");
                }
                return View("ForgotPasswordConfirmation");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            var model = new ResetPasswordViewModel
            {
                Email = email,
                Token = token
            };
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError("", "Invalid password reset token");
                return View(model);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var passwordChangeResult = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (passwordChangeResult.Succeeded)
                    {
                        if (await userManager.IsLockedOutAsync(user))
                            await userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow);
                        return View("PasswordUpdated");
                    }
                    foreach (var error in passwordChangeResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
                return View("PasswordUpdated");
            }
            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }


        // GET: /Account/Profile
        [HttpGet]
        public async Task<IActionResult> DisplayProfile()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var model = new ProfileViewModel()
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Username = user.UserName,
                Id = user.Id,
                ExistingPhotoPath = user.ProfilePicture
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            var model = new ProfileViewModel()
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Username = user.UserName,
                Id = user.Id,
                ExistingPhotoPath = user.ProfilePicture
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound("User not found.");
                }
                user.UserName = model.Username;
                user.Email = model.Email;
                user.FullName = model.FullName;
                user.PhoneNumber = model.PhoneNumber;
                user.PhoneNumber = model.PhoneNumber;
                if (model.Photo != null && model.Photo.Length > 0)
                {
                    // Save the uploaded photo to wwwroot/images/users
                    var uploadsFolder = Path.Combine(hostEnvironment.WebRootPath, "images/users");
                    Directory.CreateDirectory(uploadsFolder); // Ensure the directory exists
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Photo.CopyToAsync(fileStream);
                    }
                    model.ExistingPhotoPath = "/images/users/" + uniqueFileName;
                }
                user.ProfilePicture = model.ExistingPhotoPath;
                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["ToastMessage"] = "Profile updated successfully.";
                    return RedirectToAction("DisplayProfile");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View("EditProfile", model);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                ViewBag.Message = $"User is not available";
                return View("NotFound");
            }

            var hasPassword = await userManager.HasPasswordAsync(user);

            if (!hasPassword)
            {
                return RedirectToAction("AddPassword");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                IdentityResult result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    await signInManager.RefreshSignInAsync(user);
                    TempData["ToastMessage"] = "Password Changed Successfully";
                    return RedirectToAction("EditProfile", new { userId = user.Id});
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }
            return View();
        }

        [HttpGet]
        public IActionResult VerifyEmail(string email)
        {
            return View("VerifyEmail", email);
        }

        [HttpPost]
        public async Task<IActionResult> ResendEmailVerification(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login");
            }
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return View("VerifyEmail");
            }

            if(user.EmailConfirmed)
            {
                TempData["ToastMessage"] = "Email is already confirmed. Please login.";
                return RedirectToAction("Login");
            }

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action("ConfirmEmail", "Account",
                new { userId = user.Id, token = token }, Request.Scheme);

            var emailSubject = "Confirm Your Email";
            var emailBody = $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.";
            await emailSender.SendEmailAsync(user.Email!, emailSubject, emailBody);

            //send a copy of the token to the log file as well
            logger.Log(LogLevel.Warning, $"Email Confirmation Link {confirmationLink}");
            
            TempData["ToastMessage"] = "Verification email sent. Please check your email.";

            return View("VerifyEmail", email);
        }

        [HttpGet]
        public async Task<IActionResult> AddPassword()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                ViewBag.Message = $"User is not available";
                return View("NotFound");
            }
            var hasPassword = await userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                return RedirectToAction("ChangePassword");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPassword(AddPasswordViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    ViewBag.Message = $"User is not available";
                    return View("NotFound");
                }

                var result = await userManager.AddPasswordAsync(user, model.NewPassword);
                if (result.Succeeded)
                {
                    await signInManager.RefreshSignInAsync(user);
                    TempData["ToastMessage"] = "You have successfully add a password to your account";
                    return RedirectToAction("DisplayProfile");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }
    }
}

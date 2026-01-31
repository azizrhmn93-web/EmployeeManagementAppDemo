using EmployeeManagement.Models;
using EmployeeManagement.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Host.UseSerilog((context, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration);
        });


        builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireDigit = true;
            options.Password.RequiredUniqueChars = 1;

            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

            options.Tokens.EmailConfirmationTokenProvider = "EmailTokenProvider";
        }).AddEntityFrameworkStores<AppDBContext>()
        .AddDefaultTokenProviders()
        .AddTokenProvider<EmailConfirmationTokenProvider<AppUser>>("EmailTokenProvider");

        // Configure all tokens lifespan
        builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
               options.TokenLifespan = TimeSpan.FromMinutes(30));

        // Set custom token lifespan for email confirmation token
        builder.Services.Configure<EmailConfirmationTokenProviderOption>(options =>
               options.TokenLifespan = TimeSpan.FromHours(4));

        builder.Services.AddDbContextPool<AppDBContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("EmployeeDbConnection")));
        builder.Services.AddControllersWithViews(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                             .RequireAuthenticatedUser()
                             .Build();
            options.Filters.Add(new AuthorizeFilter(policy));
            options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
        }).AddXmlDataContractSerializerFormatters();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSingleton<IAuthorizationHandler, NotSelfUpdateHandler>();
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("DeleteRolePolicy", policy =>
                policy.RequireClaim("Delete Role", "true"));

            options.AddPolicy("EditRolePolicy", policy =>
                policy.RequireRole("Admin", "Super Admin"));

            options.AddPolicy("UpdateUserClaimsPolicy", policy =>
                policy.AddRequirements(new NotSelfUpdateRequirement()));
        });

        builder.Services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? string.Empty;
                options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? string.Empty;
            });
        builder.Services.AddScoped<IEmployeeDepository, DbEmployeeDepository>();
        builder.Services.AddTransient<IEmailSender, EmailSender>();
        builder.Services.AddDataProtection();
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseStatusCodePagesWithReExecute("/Error/{0}");
        }
        app.UseStaticFiles();
        app.UseHttpsRedirection();
       
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapDefaultControllerRoute();
        //app.MapRazorPages();
        //app.MapControllerRoute(
        //    name: "custom",
        //    pattern: "{controller=Home}/{action=details}/{id:int:min(1):max(3)?}"
        //    );
        //   app.MapControllers();
        app.MapGet("/map", () => "Hello World form MapGet");

        app.Run();
    }
}
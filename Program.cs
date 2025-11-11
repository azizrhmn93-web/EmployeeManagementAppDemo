using EmployeeManagement.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddDbContextPool<AppDBContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("EmployeeDbConnection")));
        builder.Services.AddControllersWithViews().AddXmlDataContractSerializerFormatters();
        builder.Services.AddScoped<IEmployeeDepository, DbEmployeeDepository>();
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/error");
            app.Map("/error", (HttpContext context) => { return Results.Problem("Page not found exception!"); });
        }
        app.UseStaticFiles();
        app.UseHttpsRedirection();

        
        app.UseRouting();
        //  app.UseMvcWithDefaultRoute();
         app.MapDefaultControllerRoute();
        //app.MapControllerRoute(
        //    name: "custom",
        //    pattern: "{controller=Home}/{action=details}/{id:int:min(1):max(3)?}"
       //    );
     //   app.MapControllers();





        app.MapGet("/map", () => "Hello World form MapGet");

        app.Run();
    }
}
using Basics.AuthorizationRequirements;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandler>();

// Configure the cookies that identifies a user
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", config =>
    {
        config.Cookie.Name = "Starr.Cookie";
        config.LoginPath = "/Home/Authenticate";
    });


// Configure the Roles, Claims, Policies that will be assigned to a user.
// A user must possess all the features below in order to be allowed/authenticated to use certains services within the app.
builder.Services.AddAuthorization(config =>
{

    // DEFAULT METHOD OF SETTING UP REQUIRED AUTHORIZATIONS

    //var defaultAuthBuilder = new AuthorizationPolicyBuilder();
    //var defaultAuthPolicy = defaultAuthBuilder
    //    .RequireAuthenticatedUser()
    //    .RequireClaim(ClaimTypes.DateOfBirth)
    //    .Build();

    //config.DefaultPolicy = defaultAuthPolicy;


    // You can configure Roles to be required when setting up Authorization
    config.AddPolicy("Admin", policyBuilder =>
    {
        policyBuilder.RequireRole("Admin");
        //policyBuilder.RequireClaim(ClaimTypes.Role, "Admin"); // A different method of configuring Roles
    });


    // The below RequireCustomClaim() is a custom authorization method.

    config.AddPolicy("ClaimType.DoB", builder =>
    {
        builder.RequireCustomClaim(ClaimTypes.DateOfBirth);
    });

    // The below RequireClaim() is the default method from the Authorization package

    //config.AddPolicy("ClaimTypes.DoB", builder =>
    //{
    //    builder.RequireClaim(ClaimTypes.DateOfBirth);
    //});

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();

// Asks 'WHO ARE YOU???'
app.UseAuthentication();

// Asks 'ARE YOU ALLOWED???'
app.UseAuthorization();

//app.MapDefaultControllerRoute();

app.UseEndpoints(endPoints =>
{
    endPoints.MapDefaultControllerRoute();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

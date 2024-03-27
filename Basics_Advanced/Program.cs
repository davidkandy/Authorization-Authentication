using Basics.AuthorizationRequirements;
using Basics_Advanced.Controllers;
using Basics_Advanced.CustomPolicyProvider;
using Basics_Advanced.Transformation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);


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

    config.AddPolicy("Claim.DoB", builder =>
    {
        builder.RequireCustomClaim(ClaimTypes.DateOfBirth);
    });

    // The below RequireClaim() is the default method from the Authorization package

    //config.AddPolicy("ClaimTypes.DoB", builder =>
    //{
    //    builder.RequireClaim(ClaimTypes.DateOfBirth);
    //});

});


builder.Services.AddControllersWithViews(config =>
{
    var builder = new AuthorizationPolicyBuilder();
    var defaultAuthPolicy = builder
        .RequireAuthenticatedUser()
        .Build();

    // The AuthorizeFilter() will Configure the policies the [Authorize] attribute will filter for
    //config.Filters.Add(new AuthorizeFilter(defaultAuthPolicy));
})
    .AddRazorRuntimeCompilation();


// Adds the functionality to configure AUTHORIZATION for Razor pages
builder.Services.AddRazorPages(config =>
{
    config.Conventions.AuthorizePage("/Razor/Secured");
    config.Conventions.AuthorizePage("/Razor/Policy", "Admin");
    config.Conventions.AuthorizeFolder("/RazorSecured");
    config.Conventions.AllowAnonymousToPage("/RazorSecured/Anon");
}); 


builder.Services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, SecurityLevelHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CookieJarAuthorizationHandler>();
builder.Services.AddScoped<IClaimsTransformation, ClaimsTransformation>();


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
    endPoints.MapRazorPages();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

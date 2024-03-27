using IdentityExample.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;

var builder = WebApplication.CreateBuilder(args);

var _config = builder.Configuration;

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(config => config.UseInMemoryDatabase("Memory"));


// Add Identity registers the services
builder.Services.AddIdentity<IdentityUser, IdentityRole>(config =>
{
    config.Password.RequiredLength = 4;
    config.Password.RequireDigit = false;
    config.Password.RequireUppercase = false;
    config.Password.RequireNonAlphanumeric = false;
    config.SignIn.RequireConfirmedEmail = true; // set this to true when you want to set up the verify email functionality
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders(); // Adds the functionality to create and send out tokens (eg email verification etc.)

// This is the functionality for setting up cookie configuration when using IdentityDbContext
builder.Services.ConfigureApplicationCookie(config =>
{
    config.LoginPath = "/Home/Login";
    config.Cookie.Name = "Identity.Cookie";
});

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();



var mailKitOptions = _config.GetSection("Email").Get<MailKitOptions>();

builder.Services.AddMailKit(config =>
{
    config.UseMailKit(mailKitOptions);

    //OR

    //config.UseMailKit(new MailKitOptions()
    //{
    //    SenderName = "David",
    //    SenderEmail = "david@testing.com",
    //    Server = "127.0.0.1",
    //    Port = 25
    //});
});

//builder.Services.AddAuthentication("CookieAuth")
//    .AddCookie("CookieAuth", config =>
//    {
//        config.Cookie.Name = "Starr.Cookie";
//        config.LoginPath = "/Home/Authenticate";
//    });

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

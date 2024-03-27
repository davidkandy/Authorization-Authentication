var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.AddAuthentication(config =>
{
    // We check the cookie to confirm if we are authenticated
    config.DefaultScheme = "ClientCookie";

    // When we sign in, we deal out a cookie
    config.DefaultSignInScheme = "ClientCookie";

    // use this to check if we are allowed to do something
    config.DefaultChallengeScheme = "OurServer";
})
    .AddCookie("ClientCookie")
    .AddOAuth("OurServer", config =>
    {
        config.ClientId = "client_id";
        config.ClientSecret = "client_secret";
        config.CallbackPath = "/oauth/callback";
        config.AuthorizationEndpoint = "https://localhost:7069/oauth/authorize";
        config.TokenEndpoint = "https://localhost:7069/oauth/token";
    });

var app = builder.Build();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endPoints =>
{
    endPoints.MapDefaultControllerRoute();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

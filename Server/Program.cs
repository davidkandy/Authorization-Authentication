using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Server;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure the cookies that identifies a user
builder.Services.AddAuthentication("OAuth")
    .AddJwtBearer("OAuth", config =>
    {
        var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
        var key = new SymmetricSecurityKey(secretBytes);

        // This is how you configure your Server to validate the Audience, Issuer and Key.
        // without this, your program doesn't know anything about the JWT token you create anywhere in the app
        config.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidAudience = Constants.Audience,
            ValidIssuer = Constants.Issuer,
            IssuerSigningKey = key,
        };

        // This is how you can use a token within the URL
        // Example - {accessToken : "jdjdjdj.jsojeinjisdjoalnkm.kjsdnkjnkjds"}
        // You can use the accessToken to authenticate a URL 
        // Example - https://localhost.7005/home/secret?accessToken=jdjdjdj.jsojeinjisdjoalnkm.kjsdnkjnkjds
        // Then that URL will be authenticated as long as the accessToken value is accurate.
        config.Events = new JwtBearerEvents()
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Query.ContainsKey("access_token"))
                {
                    context.Token = context.Request.Query["access_token"];
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

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

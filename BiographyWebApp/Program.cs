using BiographyWebApp.Abstractions;
using BiographyWebApp.Database.DbContexts;
using BiographyWebApp.Database.Repositories;
using BiographyWebApp.Services;
using BiographyWebApp.Services.Filters;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login";
        options.Cookie.Name = "user";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.AccessDeniedPath = "/";
    });

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    );
builder.Services.AddScoped<IAppDbContext>(provider => 
    provider.GetService<AppDbContext>()!);

builder.Services.AddScoped<IAppRepository, AppRepository>();

builder.Services.AddSingleton<EmailSenderService>();

builder.Services.AddScoped<AuthService>();

builder.Services.AddSingleton<CustomExceptionFilterAttribute>();

builder.Services.AddMvc()
.AddMvcOptions(
    options =>
    {
        options.Filters.AddService<CustomExceptionFilterAttribute>();
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

app.UseAuthentication();
app.UseAuthorization();

app.UseCookiePolicy();  

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

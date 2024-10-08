using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tutorial.Identity.Data;
using Tutorial.Identity.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("IdentityCore") ?? throw new InvalidOperationException("Connection string 'IdentityCore' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)
 );

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredUniqueChars = 4;
})
.AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddAuthentication()
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "";
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "";
    // You can set other options as needed.
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EditRolePolicy", policy => policy.RequireClaim("EditRole"));
    options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("DeleteRole"));
});

// Configure the Application Cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    // If the LoginPath isn't set, ASP.NET Core defaults the path to /Accounts/Login.
    options.LoginPath = "/Accounts/Login"; // Set your login path here
    options.AccessDeniedPath = "/Accounts/AccessDenied";
});
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

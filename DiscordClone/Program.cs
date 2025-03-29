using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DiscordClone.Data;
using DiscordClone.Models;
using SignalRChat.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.Parse("8.0.31")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// PASUL 2: 
// Se adauga manageriera de roluri la aplicatie 
// P3 -> ApplicationDbContext

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddSignalR();

var app = builder.Build();

// PASUL 5: Aici facem efectiv rolurile si conturile respective in momentul in care se deschide aplicatia
// Se vor crea o singura data
// P6 -> Se introduce User-ul in fiecare tabel care-l necesita

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapHub<ChatHub>("/chatHub");

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Groups}/{action=Index}/{id?}");
app.MapRazorPages();

app.MapControllerRoute(
    name: "login",
    pattern: "/login/",
    defaults: new { controller = "Account", action = "Login" });

app.Run();

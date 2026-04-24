using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using SharavaniTours.Data;
using SharavaniTours.Models;
using SharavaniTours.Services;
using static SharavaniTours.Helpers.Seeder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("LiveConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
	options.SignIn.RequireConfirmedAccount = false; //  set false for now
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultUI()
.AddDefaultTokenProviders();


builder.Services.AddScoped<BillingService>();
builder.Services.AddScoped<DatabaseResetService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();	

var app = builder.Build();

RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;

	await DbSeeder.SeedRoles(services);   //  FIRST
	await DbSeeder.SeedAdmin(services);   //  SECOND
	await DbSeeder.SeedVehicleTypes(services); // Third
	await DbSeeder.SeedClients(services); // Fourth
	
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
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
	name: "areas",
	pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
);

app.MapDefaultControllerRoute();
app.MapRazorPages();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}")
//    .WithStaticAssets();

//app.MapRazorPages()
//   .WithStaticAssets();

app.Run();

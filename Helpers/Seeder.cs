using Microsoft.AspNetCore.Identity;
using SharavaniTours.Data;
using SharavaniTours.Models;

namespace SharavaniTours.Helpers
{
	public static class Seeder
	{

		public static class DbSeeder
		{
			public static async Task SeedRoles(IServiceProvider serviceProvider)
			{
				var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

				string[] roles = { "Admin", "Driver" };

				foreach (var role in roles)
				{
					if (!await roleManager.RoleExistsAsync(role))
						await roleManager.CreateAsync(new IdentityRole(role));
				}
			}

			public static async Task SeedAdmin(IServiceProvider serviceProvider)
			{
				var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

				string email = "admin@demo.com";
				string password = "Admin@123";

				if (await userManager.FindByEmailAsync(email) == null)
				{
					var user = new ApplicationUser
					{
						UserName = email,
						Email = email,
						FullName = "Admin"
					};

					await userManager.CreateAsync(user, password);
					await userManager.AddToRoleAsync(user, "Admin");
				}
			}

			public static async Task SeedVehicleTypes(IServiceProvider serviceProvider)
			{
				var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

				// ✅ Ensure DB is ready
				await context.Database.EnsureCreatedAsync();

				var defaultTypes = new List<string>
				{
					"DZire",
					"Ertiga"
				};

				foreach (var type in defaultTypes)
				{
					if (!context.VehicleTypes.Any(x => x.Name == type))
					{
						context.VehicleTypes.Add(new VehicleType
						{
							Name = type
						});
					}
				}

				await context.SaveChangesAsync();
			}

			public static async Task SeedClients(IServiceProvider serviceProvider)
			{
				var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

				// ✅ Check if client already exists (by GST - best unique field)
				var gst = "27AAACG1376N1ZC";

				var existingClient = context.Clients
					.FirstOrDefault(x => x.GST == gst);

				if (existingClient == null)
				{
					context.Clients.Add(new Client
					{
						Name = "Kansai Nerolac Paints Ltd",
						GST = gst,
						Address = "28th Floor, A - Wing, Marathon Futurex, NM Joshi Marg, Lower Parel, Mumbai, Maharashtra 400013",
						CreatedAt = DateTime.Now,
						IsDeleted = false
					});

					await context.SaveChangesAsync();
				}
			}

		}
	}
}

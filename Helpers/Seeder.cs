using Microsoft.AspNetCore.Identity;
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


		}
	}
}

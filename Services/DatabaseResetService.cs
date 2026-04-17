using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharavaniTours.Data;
using SharavaniTours.Models;

namespace SharavaniTours.Services
{
	public class DatabaseResetService
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IServiceProvider _serviceProvider;

		public DatabaseResetService(
			ApplicationDbContext context,
			UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager,
			IServiceProvider serviceProvider)
		{
			_context = context;
			_userManager = userManager;
			_roleManager = roleManager;
			_serviceProvider = serviceProvider;
		}

		public async Task ResetDatabaseAsync()
		{
			// 🔥 DELETE DATABASE
			await _context.Database.EnsureDeletedAsync();

			// 🔥 CREATE DATABASE
			await _context.Database.EnsureCreatedAsync();

			// 🔥 RUN SEEDER AGAIN
			await SeedData();
		}

		private async Task SeedData()
		{
			// ===== ROLES =====
			if (!await _roleManager.RoleExistsAsync("Admin"))
				await _roleManager.CreateAsync(new IdentityRole("Admin"));

			if (!await _roleManager.RoleExistsAsync("Driver"))
				await _roleManager.CreateAsync(new IdentityRole("Driver"));

			// ===== ADMIN USER =====
			var adminEmail = "admin@sharavani.com";

			var admin = await _userManager.FindByEmailAsync(adminEmail);

			if (admin == null)
			{
				admin = new ApplicationUser
				{
					UserName = adminEmail,
					Email = adminEmail,
					FullName = "Admin"
				};

				await _userManager.CreateAsync(admin, "Admin@123");
				await _userManager.AddToRoleAsync(admin, "Admin");
			}

			// 👉 OPTIONAL: Seed basic data
			if (!_context.RateCards.Any())
			{
				_context.RateCards.Add(new RateCard
				{
					Name = "Local",
					BaseKM = 80,
					BaseHours = 8,
					BasePrice = 2000,
					ExtraKMRate = 13,
					ExtraHourRate = 85
				});

				await _context.SaveChangesAsync();
			}
		}
	}
}

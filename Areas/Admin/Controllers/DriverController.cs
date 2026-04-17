using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharavaniTours.Models;

namespace SharavaniTours.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class DriverController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;

		public DriverController(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}

		// =======================
		// LIST
		// =======================
		public IActionResult Index()
		{
			var drivers = _userManager.Users.ToList();
			return View(drivers);
		}

		// =======================
		// DETAILS
		// =======================
		public async Task<IActionResult> Details(string id)
		{
			if (id == null)
				return NotFound();

			var user = await _userManager.FindByIdAsync(id);

			if (user == null)
				return NotFound();

			return View(user);
		}

		// =======================
		// CREATE GET
		// =======================
		public IActionResult Create()
		{
			return View();
		}

		// =======================
		// CREATE POST
		// =======================
		[HttpPost]
		public async Task<IActionResult> Create(string fullName, string email, string password)
		{
			if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
			{
				ModelState.AddModelError("", "All fields are required");
				return View();
			}

			var user = new ApplicationUser
			{
				FullName = fullName,
				UserName = email,
				Email = email
			};

			var result = await _userManager.CreateAsync(user, password);

			if (result.Succeeded)
			{
				await _userManager.AddToRoleAsync(user, "Driver");
				return RedirectToAction("Index");
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error.Description);
			}

			return View();
		}

		// =======================
		// EDIT GET
		// =======================
		public async Task<IActionResult> Edit(string id)
		{
			if (id == null)
				return NotFound();

			var user = await _userManager.FindByIdAsync(id);

			if (user == null)
				return NotFound();

			return View(user);
		}

		// =======================
		// EDIT POST
		// =======================
		[HttpPost]
		public async Task<IActionResult> Edit(ApplicationUser model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var user = await _userManager.FindByIdAsync(model.Id);

			if (user == null)
				return NotFound();

			// Update fields
			user.FullName = model.FullName;
			user.Email = model.Email;
			user.UserName = model.Email;

			var result = await _userManager.UpdateAsync(user);

			if (result.Succeeded)
			{
				return RedirectToAction("Index");
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error.Description);
			}

			return View(model);
		}

		// =======================
		// DELETE GET (CONFIRM)
		// =======================
		public async Task<IActionResult> Delete(string id)
		{
			if (id == null)
				return NotFound();

			var user = await _userManager.FindByIdAsync(id);

			if (user == null)
				return NotFound();

			return View(user);
		}

		// =======================
		// DELETE POST
		// =======================
		[HttpPost, ActionName("Delete")]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			var user = await _userManager.FindByIdAsync(id);

			if (user == null)
				return NotFound();

			// ⚠️ Hard delete (Identity default)
			var result = await _userManager.DeleteAsync(user);

			if (result.Succeeded)
			{
				return RedirectToAction("Index");
			}

			ModelState.AddModelError("", "Error deleting user");
			return View(user);
		}
	}
}
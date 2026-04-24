using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharavaniTours.Services;

namespace SharavaniTours.Controllers
{
	[Authorize(Roles = "Admin")]
	public class SystemController : Controller
	{
		private readonly DatabaseResetService _resetService;
		private readonly IWebHostEnvironment _env;


		public SystemController(DatabaseResetService resetService, IWebHostEnvironment env)
		{
			_resetService = resetService;
			_env = env;
		}

		public IActionResult Reset()
		{
			if (!_env.IsDevelopment())
				return Unauthorized();

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ResetConfirmed()
		{
			await _resetService.ResetDatabaseAsync();

			return RedirectToAction("Index", "Home");
		}
	}
}

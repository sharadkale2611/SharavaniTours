using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SharavaniTours.Data;
using SharavaniTours.Models;

namespace SharavaniTours.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class ClientUserController : Controller
	{
		private readonly ApplicationDbContext _context;

		public ClientUserController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public JsonResult GetByClientId(int clientId)
		{
			var users = _context.ClientUsers
				.Where(x => x.ClientId == clientId)
				.Select(x => new
				{
					id = x.Id,
					name = x.FullName + " (" + x.MobileNo + ")"
				})
				.ToList();

			return Json(users);
		}

		public IActionResult Index()
		{
			var users = _context.ClientUsers
				.Include(x => x.Client)
				.ToList();

			return View(users);
		}

		public IActionResult Create()
		{
			ViewBag.Clients = _context.Clients.ToList();
			return View();
		}

		[HttpPost]
		public IActionResult Create(ClientUser model)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.Clients = _context.Clients.ToList();
				return View(model);
			}

			_context.ClientUsers.Add(model);
			_context.SaveChanges();

			return RedirectToAction("Index");
		}

		public IActionResult Edit(int id)
		{
			var user = _context.ClientUsers.Find(id);

			if (user == null)
				return NotFound();

			ViewBag.Clients = new SelectList(_context.Clients, "Id", "Name", user.ClientId);

			return View(user);
		}

		[HttpPost]
		public IActionResult Edit(ClientUser model)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.Clients = _context.Clients.ToList();
				return View(model);
			}

			var existingUser = _context.ClientUsers.FirstOrDefault(x => x.Id == model.Id);

			if (existingUser == null)
				return NotFound();

			// Update fields
			existingUser.FullName = model.FullName;
			existingUser.MobileNo = model.MobileNo;
			existingUser.ClientId = model.ClientId;

			_context.SaveChanges();

			return RedirectToAction("Index");
		}

		[HttpPost, ActionName("Delete")]
		public IActionResult DeleteConfirmed(int id)
		{
			var user = _context.ClientUsers.FirstOrDefault(x => x.Id == id);

			if (user == null)
				return NotFound();

			//  Soft delete
			user.IsDeleted = true;

			_context.SaveChanges();

			return RedirectToAction("Index");
		}
	}
}
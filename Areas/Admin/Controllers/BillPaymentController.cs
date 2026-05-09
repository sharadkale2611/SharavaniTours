using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharavaniTours.Data;
using SharavaniTours.Models;

namespace SharavaniTours.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BillPaymentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BillPaymentController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // 📋 LIST
        // =====================================================

        public async Task<IActionResult> Index(
            BillPaymentStatus? status)
        {
            var query = _context.BillPayments
                .Include(x => x.Trip)
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            // =====================================================
            // FILTER
            // =====================================================

            if (status.HasValue)
            {
                query = query.Where(x =>
                    x.PaymentStatus == status.Value);
            }

            var bills = await query
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            ViewBag.SelectedStatus = status;

            return View(bills);
        }

        // =====================================================
        // 🔍 DETAILS
        // =====================================================

        public async Task<IActionResult> Details(int id)
        {
            var bill = await _context.BillPayments
                .Include(x => x.Trip)
                .FirstOrDefaultAsync(x =>
                    x.Id == id);

            if (bill == null)
                return NotFound();

            return View(bill);
        }

        // =====================================================
        // ✏️ EDIT
        // =====================================================

        public async Task<IActionResult> Edit(int id)
        {
            var bill = await _context.BillPayments
                .FirstOrDefaultAsync(x =>
                    x.Id == id);

            if (bill == null)
                return NotFound();

            return View(bill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            BillPayment model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var bill = await _context.BillPayments
                .FirstOrDefaultAsync(x =>
                    x.Id == model.Id);

            if (bill == null)
                return NotFound();

            // =================================================
            // UPDATE
            // =================================================

            bill.BillDate = model.BillDate;

            bill.Location = model.Location;

            bill.VehicleNo = model.VehicleNo;

            bill.VehicleType = model.VehicleType;

            bill.DutySlipNo = model.DutySlipNo;

            bill.BillNo = model.BillNo;

            bill.Amount = model.Amount;

            bill.TollParkingAmount =
                model.TollParkingAmount;

            bill.TDS = model.TDS;

            bill.FinalAmount =
                model.FinalAmount;

            bill.PaymentStatus =
                model.PaymentStatus;

            bill.PaymentDate =
                model.PaymentDate;

            bill.Remarks =
                model.Remarks;

            bill.UpdatedAt =
                DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // =====================================================
        // ❌ DELETE
        // =====================================================

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var bill = await _context.BillPayments
                .FirstOrDefaultAsync(x =>
                    x.Id == id);

            if (bill == null)
                return NotFound();

            bill.IsDeleted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharavaniTours.Data;
using SharavaniTours.Models;

namespace SharavaniTours.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TripController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TripController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // =======================
        // CREATE (GET)
        // =======================
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();


            // ✅ Prevent null error
            //ViewBag.ClientUsers = new List<ClientUser>();

            return View();
        }


        // =======================
        // CREATE (POST)
        // =======================
        [HttpPost]
        public async Task<IActionResult> Create(Trip trip)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();

                ViewBag.ClientUsers = _context.ClientUsers
                    .Where(x => x.ClientId == trip.ClientId)
                    .ToList();

                // 🔹 Reload RateCards on validation error
                if (trip.RequestedVehicleId.HasValue)
                {
                    var vehicleData = await _context.Vehicles
                        .FirstOrDefaultAsync(v =>
                            v.Id == trip.RequestedVehicleId);

                    if (vehicleData != null)
                    {
                        ViewBag.RateCards = _context.RateCards
                            .Where(r =>
                                r.VehicleTypeId ==
                                vehicleData.VehicleTypeId
                                &&
                                !r.IsDeleted)
                            .ToList();
                    }
                }

                return View(trip);
            }

            // =====================================================
            // 🚦 SET STATUS
            // =====================================================

            if (!string.IsNullOrEmpty(trip.DriverId))
            {
                if (trip.StartDate.HasValue &&
                    trip.EndDate.HasValue)
                {
                    trip.Status = TripStatus.Completed;
                }
                else if (trip.StartDate.HasValue)
                {
                    trip.Status = TripStatus.Started;
                }
                else
                {
                    trip.Status = TripStatus.Assigned;
                }
            }
            else
            {
                trip.Status = TripStatus.Pending;
            }

            // =====================================================
            // 🚗 ASSIGN SENT VEHICLE
            // =====================================================

            if (trip.RequestedVehicleId.HasValue)
            {
                trip.SentVehicleId =
                    trip.RequestedVehicleId;
            }

            // =====================================================
            // 💰 AUTO CALCULATE TOTAL AMOUNT
            // =====================================================

            var rateCard = await _context.RateCards
                .FirstOrDefaultAsync(x =>
                    x.Id == trip.RateCardId);

            decimal finalTotal = 0;

            if (rateCard != null)
            {
                // =================================================
                // 🚗 KM CALCULATION
                // =================================================

                int totalKm =
                    (trip.EndKM ?? 0)
                    -
                    (trip.StartKM ?? 0);

                int baseKm =
                    rateCard.BaseKM ?? 0;

                int extraKm =
                    Math.Max(0, totalKm - baseKm);

                decimal extraKmAmount =
                    extraKm *
                    (rateCard.ExtraKMRate ?? 0);

                // =================================================
                // ⏱️ TIME CALCULATION
                // =================================================

                DateTime startTime =
                    trip.StartDate ?? DateTime.Now;

                DateTime endTime =
                    trip.EndDate ?? DateTime.Now;

                var tripDuration =
                    endTime - startTime;

                int extraHours = 0;

                decimal totalDays =
                    (decimal)Math.Floor(
                        tripDuration.TotalDays
                    );

                // =================================================
                // 🚕 LOCAL TRIP
                // =================================================

                if (trip.TripType ==
                    TripType.Local)
                {
                    int totalHours =
                        (int)Math.Round(
                            tripDuration.TotalHours
                        );

                    int baseHours =
                        rateCard.BaseHours ?? 0;

                    extraHours =
                        Math.Max(
                            0,
                            totalHours - baseHours
                        );

                    finalTotal =
                        (rateCard.BasePrice ?? 0)
                        +
                        extraKmAmount
                        +
                        (
                            extraHours *
                            (rateCard.ExtraHourRate ?? 0)
                        )
                        +
                        trip.DriverAllowance
                        +
                        trip.TollCharges
                        +
                        trip.ParkingCharges;
                }

                // =================================================
                // 🌍 OUTSTATION TRIP
                // =================================================

                else
                {
                    // 🔹 Remaining Hours
                    extraHours =
                        tripDuration.Hours;

                    // 🔹 Optional rounding
                    if (tripDuration.Minutes >= 30)
                    {
                        extraHours++;
                    }

                    finalTotal =
                        (
                            totalDays *
                            (
                                rateCard.OutstationRatePerDay
                                ?? 0
                            )
                        )
                        +
                        extraKmAmount
                        +
                        (
                            extraHours *
                            (
                                rateCard.ExtraHourRate
                                ?? 0
                            )
                        )
                        +
                        trip.DriverAllowance
                        +
                        trip.TollCharges
                        +
                        trip.ParkingCharges;
                }
            }

            // 🔹 Final Amount
            trip.TotalAmount = finalTotal;

            // =====================================================
            // 💾 SAVE TRIP
            // =====================================================

            _context.Trips.Add(trip);

            await _context.SaveChangesAsync();

            // =====================================================
            // 🧾 CREATE DUTY SLIP
            // =====================================================

            DutySlip? dutySlip = null;

            if (trip.Status == TripStatus.Started
                ||
                trip.Status == TripStatus.Completed)
            {
                dutySlip = new DutySlip
                {
                    TripId = trip.Id,

                    DutySlipNo =
                        !string.IsNullOrWhiteSpace(trip.DutySlipNo)
                            ? trip.DutySlipNo
                            : $"DS-{DateTime.Now:yyyyMMddHHmmss}",

                    // 🚀 Trip Execution
                    StartKM = trip.StartKM ?? 0,

                    EndKM = trip.EndKM ?? 0,

                    StartTime =
                        trip.StartDate ?? DateTime.Now,

                    EndTime =
                        trip.EndDate ?? DateTime.Now,

                    // 💰 Charges
                    TollCharges = trip.TollCharges,

                    ParkingCharges =
                        trip.ParkingCharges,

                    // 📍 Reporting
                    ReportingTime =
                        trip.ReportingTime
                        ?? DateTime.Now,

                    ReportingAddress =
                        trip.ReportingAddress
                        ?? "N/A",

                    // 📄 Duty Details
                    DutyType =
                        trip.DutyType
                        ?? "Local",

                    PaymentMode =
                        trip.PaymentMode
                        ?? "Cash",

                    NextDayInstruction =
                        trip.NextDayInstruction
                        ?? "-"
                };

                _context.DutySlips.Add(dutySlip);

                await _context.SaveChangesAsync();
            }

            // =====================================================
            // 💳 AUTO BILL PAYMENT
            // =====================================================

            if (trip.Status == TripStatus.Completed)
            {
                var vehicle = await _context.Vehicles
                    .Include(v => v.VehicleType)
                    .FirstOrDefaultAsync(v =>
                        v.Id == trip.SentVehicleId);

                decimal tollParking =
                    trip.TollCharges
                    +
                    trip.ParkingCharges;

                var billPayment =
                    new BillPayment
                    {
                        TripId = trip.Id,

                        BillDate = DateTime.Now,

                        Location =
                            trip.DropLocation,

                        VehicleNo =
                            vehicle?.VehicleNo ?? "",

                        VehicleType =
                            vehicle?.VehicleType?.Name
                            ?? "",

                        DutySlipNo =
                            dutySlip?.DutySlipNo ?? "",

                        BillNo =
                            $"BILL-{DateTime.Now.Ticks}",

                        Amount =
                            trip.TotalAmount,

                        TollParkingAmount =
                            tollParking,

                        TDS = 0,

                        FinalAmount =
                            trip.TotalAmount,

                        PaymentStatus =
                            BillPaymentStatus.Pending,

                        PaymentDate = null,

                        Remarks =
                            $"Auto generated from Trip ID {trip.Id}"
                    };

                _context.BillPayments
                    .Add(billPayment);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }


        // =======================
        // LIST
        // =======================
        public IActionResult Index()
        {
            var trips = _context.Trips
                .Include(t => t.Client)
                .Include(t => t.Driver)
                .Include(t => t.RequestedVehicle)
                .Include(t => t.SentVehicle)
                .Include(t => t.RateCard)
                .Include(t => t.DutySlip)
                .ToList();

            return View(trips);
        }

        // =======================
        // DETAILS
        // =======================
        public IActionResult Details(int id)
        {
            var trip = _context.Trips
                .Include(t => t.Client)
                .Include(t => t.ClientUser)
                .Include(t => t.Driver)
                .Include(t => t.RequestedVehicle)
                .Include(t => t.SentVehicle)
                .Include(t => t.RateCard)
                .Include(t => t.DutySlip)
                .FirstOrDefault(t => t.Id == id);

            if (trip == null)
                return NotFound();

            return View(trip);
        }

        // =======================
        // EDIT (GET)
        // =======================
        public async Task<IActionResult> Edit(int id)
        {
            var trip = await _context.Trips
                .Include(x => x.DutySlip)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (trip == null)
                return NotFound();

            await LoadDropdowns();

            // ✅ Load ClientUsers based on selected client
            ViewBag.ClientUsers = _context.ClientUsers
                .Where(x => x.ClientId == trip.ClientId)
                .ToList();

            if (trip.RequestedVehicleId.HasValue)
            {
                var vehicle = await _context.Vehicles
                    .FirstOrDefaultAsync(v =>
                        v.Id == trip.RequestedVehicleId);

                if (vehicle != null)
                {
                    ViewBag.RateCards = _context.RateCards
                        .Where(r =>
                            r.VehicleTypeId ==
                            vehicle.VehicleTypeId
                            &&
                            !r.IsDeleted)
                        .ToList();
                }
            }

            return View(trip);
        }

        // =======================
        // EDIT (POST)
        // =======================
        [HttpPost]
        public async Task<IActionResult> Edit(Trip model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();

                ViewBag.ClientUsers = _context.ClientUsers
                    .Where(x => x.ClientId == model.ClientId)
                    .ToList();

                return View(model);
            }

            var trip = _context.Trips.FirstOrDefault(x => x.Id == model.Id);

            if (trip == null)
                return NotFound();

            // ✅ Allowed updates
            // =====================================================
            // BASIC DETAILS
            // =====================================================

            trip.ClientId = model.ClientId;

            trip.ClientUserId = model.ClientUserId;

            trip.DriverId = model.DriverId;

            trip.RateCardId = model.RateCardId;

            trip.RequestedVehicleId = model.RequestedVehicleId;

            trip.SentVehicleId = model.RequestedVehicleId;

            trip.BookedBy = model.BookedBy;

            trip.BookedDate = model.BookedDate;

            trip.ItineraryCode = model.ItineraryCode;

            // =====================================================
            // TRIP INFO
            // =====================================================

            trip.StartDate = model.StartDate;

            trip.EndDate = model.EndDate;

            trip.PickupLocation = model.PickupLocation;

            trip.DropLocation = model.DropLocation;

            trip.TripType = model.TripType;

            // =====================================================
            // PRICING
            // =====================================================

            trip.DriverAllowance = model.DriverAllowance;

            // =====================================================
            // DUTY SLIP DETAILS
            // =====================================================

            trip.StartKM = model.StartKM;

            trip.EndKM = model.EndKM;

            trip.TollCharges = model.TollCharges;

            trip.ParkingCharges = model.ParkingCharges;

            trip.ReportingTime = model.ReportingTime;

            trip.ReportingAddress = model.ReportingAddress;

            trip.DutyType = model.DutyType;

            trip.PaymentMode = model.PaymentMode;

            trip.NextDayInstruction =
                model.NextDayInstruction;

            trip.DutySlipNo = model.DutySlipNo;

            var dutySlip = await _context.DutySlips
                .FirstOrDefaultAsync(x => x.TripId == trip.Id);

            if (dutySlip != null)
            {
                dutySlip.DutySlipNo =
                    !string.IsNullOrWhiteSpace(model.DutySlipNo)
                        ? model.DutySlipNo
                        : dutySlip.DutySlipNo;

                dutySlip.StartKM = model.StartKM ?? 0;

                dutySlip.EndKM = model.EndKM ?? 0;

                dutySlip.StartTime =
                    model.StartDate ?? DateTime.Now;

                dutySlip.EndTime =
                    model.EndDate ?? DateTime.Now;

                dutySlip.TollCharges =
                    model.TollCharges;

                dutySlip.ParkingCharges =
                    model.ParkingCharges;

                dutySlip.ReportingTime =
                    model.ReportingTime ?? DateTime.Now;

                dutySlip.ReportingAddress =
                    model.ReportingAddress ?? "";

                dutySlip.DutyType =
                    model.DutyType ?? "";

                dutySlip.PaymentMode =
                    model.PaymentMode ?? "";

                dutySlip.NextDayInstruction =
                    model.NextDayInstruction ?? "";
            }


            // =====================================================
            // 🚦 AUTO STATUS
            // =====================================================

            if (!string.IsNullOrEmpty(trip.DriverId))
            {
                if (trip.StartDate.HasValue &&
                    trip.EndDate.HasValue)
                {
                    trip.Status = TripStatus.Completed;
                }
                else if (trip.StartDate.HasValue)
                {
                    trip.Status = TripStatus.Started;
                }
                else
                {
                    trip.Status = TripStatus.Assigned;
                }
            }
            else
            {
                trip.Status = TripStatus.Pending;
            }

            // =====================================================
            // 💰 RECALCULATE TOTAL
            // =====================================================

            var rateCard = await _context.RateCards
                .FirstOrDefaultAsync(x =>
                    x.Id == trip.RateCardId);

            decimal finalTotal = 0;

            if (rateCard != null)
            {
                int totalKm =
                    (trip.EndKM ?? 0)
                    -
                    (trip.StartKM ?? 0);

                int baseKm =
                    rateCard.BaseKM ?? 0;

                int extraKm =
                    Math.Max(0, totalKm - baseKm);

                decimal extraKmAmount =
                    extraKm *
                    (rateCard.ExtraKMRate ?? 0);

                DateTime startTime =
                    trip.StartDate ?? DateTime.Now;

                DateTime endTime =
                    trip.EndDate ?? DateTime.Now;

                var tripDuration =
                    endTime - startTime;

                int extraHours = 0;

                decimal totalDays =
                    (decimal)Math.Floor(
                        tripDuration.TotalDays
                    );

                // =================================================
                // LOCAL
                // =================================================

                if (trip.TripType == TripType.Local)
                {
                    int totalHours =
                        (int)Math.Round(
                            tripDuration.TotalHours
                        );

                    int baseHours =
                        rateCard.BaseHours ?? 0;

                    extraHours =
                        Math.Max(
                            0,
                            totalHours - baseHours
                        );

                    finalTotal =
                        (rateCard.BasePrice ?? 0)
                        +
                        extraKmAmount
                        +
                        (
                            extraHours *
                            (rateCard.ExtraHourRate ?? 0)
                        )
                        +
                        trip.DriverAllowance
                        +
                        trip.TollCharges
                        +
                        trip.ParkingCharges;
                }

                // =================================================
                // OUTSTATION
                // =================================================

                else
                {
                    extraHours =
                        tripDuration.Hours;

                    if (tripDuration.Minutes >= 30)
                    {
                        extraHours++;
                    }

                    finalTotal =
                        (
                            totalDays *
                            (
                                rateCard.OutstationRatePerDay
                                ?? 0
                            )
                        )
                        +
                        extraKmAmount
                        +
                        (
                            extraHours *
                            (
                                rateCard.ExtraHourRate
                                ?? 0
                            )
                        )
                        +
                        trip.DriverAllowance
                        +
                        trip.TollCharges
                        +
                        trip.ParkingCharges;
                }
            }

            trip.TotalAmount = finalTotal;

            // =====================================================
            // 🧾 CREATE DUTY SLIP IF NOT EXISTS
            // =====================================================

            if (dutySlip == null &&
                (
                    trip.Status == TripStatus.Started
                    ||
                    trip.Status == TripStatus.Completed
                ))
            {
                dutySlip = new DutySlip
                {
                    TripId = trip.Id,

                    DutySlipNo =
                        !string.IsNullOrWhiteSpace(
                            trip.DutySlipNo)
                        ? trip.DutySlipNo
                        : $"DS-{DateTime.Now:yyyyMMddHHmmss}",

                    StartKM = trip.StartKM ?? 0,

                    EndKM = trip.EndKM ?? 0,

                    StartTime =
                        trip.StartDate ?? DateTime.Now,

                    EndTime =
                        trip.EndDate ?? DateTime.Now,

                    TollCharges =
                        trip.TollCharges,

                    ParkingCharges =
                        trip.ParkingCharges,

                    ReportingTime =
                        trip.ReportingTime ?? DateTime.Now,

                    ReportingAddress =
                        trip.ReportingAddress ?? "",

                    DutyType =
                        trip.DutyType ?? "",

                    PaymentMode =
                        trip.PaymentMode ?? "",

                    NextDayInstruction =
                        trip.NextDayInstruction ?? ""
                };

                _context.DutySlips.Add(dutySlip);
            }

            // =====================================================
            // 💾 SAVE
            // =====================================================

            await _context.SaveChangesAsync();


            return RedirectToAction("Index");
        }


        // =======================
        // DELETE (SOFT DELETE)
        // =======================
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var trip = _context.Trips.FirstOrDefault(x => x.Id == id);

            if (trip == null)
                return NotFound();

            trip.IsDeleted = true;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult GetRateCardsByVehicle(int vehicleId)
        {
            var vehicle = _context.Vehicles
                .FirstOrDefault(v => v.Id == vehicleId);

            if (vehicle == null)
                return Json(new List<object>());

            var rateCards = _context.RateCards
                .Where(r => r.VehicleTypeId == vehicle.VehicleTypeId && !r.IsDeleted)
                .Select(r => new
                {
                    r.Id,
                    r.Name,
                    r.TripType,
                    Price = r.TripType == TripType.Local
                        ? r.BasePrice
                        : r.OutstationRatePerDay,
                    DriverAllowance = r.DriverAllowancePerDay // ✅ ADD THIS
                })
                .ToList();

            return Json(rateCards);
        }

        // =======================
        // 🔁 COMMON DROPDOWN LOADER
        // =======================
        private async Task LoadDropdowns()
        {
            ViewBag.Clients = _context.Clients.ToList();

            ViewBag.ClientUsers = _context.ClientUsers.Any()
                ? _context.ClientUsers.ToList()
                : new List<ClientUser>();

            ViewBag.Vehicles = _context.Vehicles
                .Include(v => v.VehicleType) // 🔥 MUST
                .ToList();

            ViewBag.RateCards = new List<RateCard>(); // ❌ not preloaded anymore

            ViewBag.Drivers = await _userManager.GetUsersInRoleAsync("Driver");
        }
    }
}
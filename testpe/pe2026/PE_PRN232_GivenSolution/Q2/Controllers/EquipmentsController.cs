using Microsoft.AspNetCore.Mvc;
using Q2.Models;
using Q2.Services;

namespace Q2.Controllers
{
    public class EquipmentsController : Controller
    {
        private readonly ApiClient _api;
        public EquipmentsController(ApiClient api) => _api = api;

        public async Task<IActionResult> Index()
        {
            var equipments = await _api.GetEquipmentsAsync();
            return View(equipments);
        }

        public async Task<IActionResult> Details(int id)
        {
            var equipment = await _api.GetEquipmentAsync(id);
            if (equipment is null) return NotFound();
            return View(equipment);
        }

        [HttpGet]
        public async Task<IActionResult> Rent(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                return RedirectToAction("Login", "Account");

            var equipment = await _api.GetEquipmentAsync(id);
            if (equipment is null) return NotFound();

            ViewBag.Equipment = equipment;
            return View(new CreateRentalDto { EquipmentId = id });
        }

        [HttpPost]
        public async Task<IActionResult> Rent(CreateRentalDto model)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            var (ok, error) = await _api.CreateRentalAsync(token, model);
            if (!ok)
            {
                var equipment = await _api.GetEquipmentAsync(model.EquipmentId);
                ViewBag.Equipment = equipment;
                ViewBag.Error = error;
                return View(model);
            }

            return RedirectToAction("Details", new { id = model.EquipmentId });
        }
    }
}

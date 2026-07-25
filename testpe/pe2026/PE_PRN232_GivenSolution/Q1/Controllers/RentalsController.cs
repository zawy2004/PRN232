using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Q1.DTOs;
using Q1.Models;

namespace Q1.Controllers
{
    [ApiController]
    [Route("api/rentals")]
    [Authorize]
    public class RentalsController : ControllerBase
    {
        private readonly FPTGearDbContext _db;
        private readonly IMapper _mapper;

        public RentalsController(FPTGearDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRentalDto dto)
        {
            var equipment = await _db.Equipments.FindAsync(dto.EquipmentId);
            if (equipment is null) return NotFound("Equipment not found");
            if (dto.Quantity <= 0 || dto.Quantity > equipment.StockQuantity)
                return BadRequest("Invalid quantity or not enough stock");

            equipment.StockQuantity -= dto.Quantity;

            var rental = new Rental
            {
                EquipmentId = dto.EquipmentId,
                UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                Quantity = dto.Quantity,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = "Active",
                FineAmount = 0,
                RentalDate = DateTime.Now
            };

            _db.Rentals.Add(rental);
            await _db.SaveChangesAsync();

            rental.Equipment = equipment;
            return CreatedAtAction(nameof(GetById), new { id = rental.RentalId }, _mapper.Map<RentalDto>(rental));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var rental = await _db.Rentals.Include(r => r.Equipment).FirstOrDefaultAsync(r => r.RentalId == id);
            return rental is null ? NotFound() : Ok(_mapper.Map<RentalDto>(rental));
        }

        [HttpPatch("{id}/return")]
        public async Task<IActionResult> Return(int id)
        {
            var rental = await _db.Rentals.Include(r => r.Equipment).FirstOrDefaultAsync(r => r.RentalId == id);
            if (rental is null) return NotFound();
            if (rental.Status == "Returned") return BadRequest("Already returned");

            if (DateTime.Now > rental.EndDate)
            {
                var daysLate = (DateTime.Now.Date - rental.EndDate.Date).Days;
                rental.FineAmount = 0.1m * rental.Equipment.PricePerDay * daysLate * rental.Quantity;
            }

            rental.Status = "Returned";
            rental.Equipment.StockQuantity += rental.Quantity;

            await _db.SaveChangesAsync();
            return Ok(_mapper.Map<RentalDto>(rental));
        }
    }
}

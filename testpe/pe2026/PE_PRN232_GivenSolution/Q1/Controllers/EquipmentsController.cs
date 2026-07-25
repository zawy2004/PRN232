using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Q1.DTOs;
using Q1.Models;
using Q1.Repositories;

namespace Q1.Controllers
{
    [ApiController]
    [Route("api/equipments")]
    public class EquipmentsController : ControllerBase
    {
        private readonly IEquipmentRepository _repo;
        private readonly IMapper _mapper;

        public EquipmentsController(IEquipmentRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(_mapper.Map<List<EquipmentDto>>(await _repo.GetQueryable().ToListAsync()));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var eq = await _repo.GetByIdAsync(id);
            return eq is null ? NotFound() : Ok(_mapper.Map<EquipmentDto>(eq));
        }

        [HttpPost, Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(CreateEquipmentDto dto)
        {
            var entity = _mapper.Map<Equipment>(dto);
            entity.CreatedAt = DateTime.Now;
            entity.UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _repo.AddAsync(entity);
            await _repo.SaveAsync();
            var created = await _repo.GetByIdAsync(entity.EquipmentId);
            return CreatedAtAction(nameof(GetById), new { id = entity.EquipmentId }, _mapper.Map<EquipmentDto>(created));
        }

        [HttpPut("{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, CreateEquipmentDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity is null) return NotFound();
            _mapper.Map(dto, entity);
            _repo.Update(entity);
            await _repo.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity is null) return NotFound();
            _repo.Remove(entity);
            await _repo.SaveAsync();
            return NoContent();
        }

        [HttpGet, Route("/odata/Equipments"), EnableQuery]
        public IQueryable<Equipment> GetOData() => _repo.GetQueryable();
    }
}

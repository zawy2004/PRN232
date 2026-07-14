using DemoAspWithJavascript.Models;
using Microsoft.AspNetCore.Mvc;

namespace DemoAspWithJavascript.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReservationController : ControllerBase
{
    private IRepository repository;
    private IWebHostEnvironment webHostEnvironment;

    public ReservationController(IRepository repo, IWebHostEnvironment environment)
    {
        repository = repo;
        webHostEnvironment = environment;
    }

    [HttpGet]
    public IEnumerable<Reservation> Get() => repository.Reservations;

    [HttpGet("{id}")]
    public ActionResult<Reservation> Get(int id)
    {
        Reservation? reservation = repository[id];
        if (reservation == null)
        {
            return NotFound();
        }

        return reservation;
    }

    [HttpPost]
    public ActionResult<Reservation> Post([FromBody] Reservation reservation, [FromHeader(Name = "Key")] string? key)
    {
        if (key != "Secret@123")
        {
            return Unauthorized();
        }

        return repository.AddReservation(reservation);
    }

    [HttpPut]
    public ActionResult<Reservation> Put([FromForm] Reservation reservation)
    {
        Reservation? updated = repository.UpdateReservation(reservation);
        if (updated == null)
        {
            return NotFound();
        }

        return updated;
    }

    [HttpDelete("{id}")]
    public void Delete(int id) => repository.DeleteReservation(id);
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ODataDemo2.Models;

namespace ODataDemo2.Controllers;

public class PressesController : ODataController
{
    private readonly BookStoreContext _db;

    public PressesController(BookStoreContext context)
    {
        _db = context;
    }

    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_db.Presses);
    }
}

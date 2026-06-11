using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using ODataASPNETCoreDemo.Data;

namespace ODataASPNETCoreDemo.Controllers;

[Route("gadget")]
[ApiController]
public class GadgetsController : ControllerBase
{
    private readonly MyWorldDbContext _myWorldDbContext;

    public GadgetsController(MyWorldDbContext myWorldDbContext)
    {
        _myWorldDbContext = myWorldDbContext;
    }

    [EnableQuery]
    [HttpGet("Get")]
    public IActionResult Get()
    {
        return Ok(_myWorldDbContext.Gadgets.AsQueryable());
    }
}

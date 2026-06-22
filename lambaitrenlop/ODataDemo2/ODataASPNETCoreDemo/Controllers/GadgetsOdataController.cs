using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ODataASPNETCoreDemo.Data;

namespace ODataASPNETCoreDemo.Controllers;

public class GadgetsOdataController : ODataController
{
    private readonly MyWorldDbContext _myworldDbContext;

    public GadgetsOdataController(MyWorldDbContext myworldDbContext)
    {
        _myworldDbContext = myworldDbContext;
    }

    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_myworldDbContext.Gadgets.AsQueryable());
    }
}

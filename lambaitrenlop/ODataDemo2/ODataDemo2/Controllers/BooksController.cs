using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using ODataDemo2.Models;

namespace ODataDemo2.Controllers;

public class BooksController : ODataController
{
    private readonly BookStoreContext _db;

    public BooksController(BookStoreContext context)
    {
        _db = context;
        _db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    [EnableQuery(PageSize = 1)]
    public IActionResult Get()
    {
        return Ok(_db.Books);
    }

    [EnableQuery]
    public IActionResult Get(int key)
    {
        var book = _db.Books.FirstOrDefault(c => c.Id == key);
        if (book == null)
        {
            return NotFound();
        }
        return Ok(book);
    }

    [EnableQuery]
    public IActionResult Post([FromBody] Book book)
    {
        if (book.Press != null && book.Press.Id != 0)
        {
            _db.Entry(book.Press).State = EntityState.Unchanged;
        }
        _db.Books.Add(book);
        _db.SaveChanges();
        return Created(book);
    }

    [EnableQuery]
    public IActionResult Delete(int key)
    {
        var book = _db.Books.FirstOrDefault(c => c.Id == key);
        if (book == null)
        {
            return NotFound();
        }

        _db.Books.Remove(book);
        _db.SaveChanges();
        return Ok();
    }
}

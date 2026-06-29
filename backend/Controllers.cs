using Microsoft.AspNetCore.Mvc;

using backend.Models;

namespace backend.Controllers;

// I wrote this file based on this section of Microsoft's docs:
// https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-10.0

// NOTE: I didn't follow the usual REST pattern for these endpoints, which
// would have been something like:
//
//   GET    /items
//   GET    /items/{id}
//   POST   /items/{id}
//   PUT    /items/{id}
//   PATCH  /items/{id}
//   DELETE /items/{id}
//
// Instead, I went with this:
//
//   GET  /items/list
//   GET  /items/details/{id}
//   POST /items/create
//   POST /items/update/{id}
//
// And the reason for this is that I don't actually *want* to implement
// arbitrary CRUD operations.
// If I leave PUT/PATCH/DELETE around, then I need to worry about the user
// noticing what calls their browser is making, and trying to fiddle around
// with them, finding weird edge cases.
// Maybe I end up having to manually prevent PATCH from updating the itemId
// field, etc.
// No thank you!.. I would rather present a minimalist interface which does
// exactly what I need for use in the frontend.
// A potential downside of this could be that e.g. with a single update
// endpoint, and no PATCH, we would always need to send all the fields.
// If some of the fields can be large (e.g. contain a lot of text), that
// might be undesireable.
// I get around that in this case by having the update endpoint only update
// non-null fields in the POST body.
[Route("/items")]
public class TodoItemController : ControllerBase {

    // Based on:
    // https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/dependency-injection?view=aspnetcore-10.0
    private readonly BackendDbContext _db;
    public TodoItemController(BackendDbContext db) {
        _db = db;
    }

    [HttpGet]
    [Route("list")]
    public IActionResult List(
        // Django REST Framework has a fancy automatic query parameter filter
        // system, but I couldn't find one for .NET Core / Entity Framework.
        // So I've just implemented one filter here, allowing the fontend to
        // show only "undone" TODO items.
        // Of course, a fancier system could be implemented, so that you could
        // do things like:
        //
        //   GET /items/list?completed=false&created_at__lt=2020-01-01&limit=10
        //
        // ...corresponding to:
        //
        //   WHERE completed = false AND created_at < '2020-01-01' LIMIT 10
        //
        // ...but I'm under a deadline, so I'm keeping this simple for now!
        //
        // NOTE: I also haven't implemented pagination.
        // That could be done via explicit "page" and "limit" query params, or
        // via a fancy open-ended filter system as described above, leaving it
        // to the frontend to decide how pages are defined.
        // In either case, I would need to implement pagination on the frontend
        // as well.
        // Ideally, there would be some kind of framework used by both the
        // frontend and backend, so that pagination could simply be configured,
        // as opposed to manually implemented.
        // Django has that kind of thing, and I image .NET Core's MVC stuff
        // probably does too, but in this case we're only using .NET Core on
        // the backend, and writing the frontend by hand in React... no shared
        // framework!
        // So in order to keep within deadlines, I'm not going to implement
        // pagination.
        [FromQuery(Name = "undone")] bool undone
    ) {
        IQueryable<TodoItem> items = _db.TodoItems;
        if (undone) items = items.Where(item => item.Completed);
        return Ok(items); // Do we need to do items.ToList()?..
    }

    [HttpGet]
    [Route("details/{itemId}")]
    public IActionResult Details(int itemId) {
        var item = _db.TodoItems.Find(itemId);
        if (item is null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    [Route("create")]
    public IActionResult Create(CreateBody body) {
        var item = new TodoItem { Title = body.title, Content = "" };
        _db.TodoItems.Add(item);
        _db.SaveChanges();
        return CreatedAtAction(nameof(Details), item);
    }

    [HttpPost]
    [Route("update/{itemId}")]
    public IActionResult Update(int itemId, UpdateBody body) {
        // NOTE: we are getting the item, then updating the fields of the
        // resulting C# object, then saving the item.
        // That's 2 SQL queries!.. we could presumably use one UPDATE.
        // I did some searching and it looks like _db.TodoItems.ExecuteUpdate
        // would be the thing to use.
        // It returns an int, so if it was 0, we'd return NotFound(),
        // otherwise, we would return Ok(...something...), but it's not
        // obvious what the something would be.
        // (We could return a 204 No Content, which it looks like... would
        // be NoContent()!.. straightforward.)
        // Anyway, might as well grab the item with a separate query, so we
        // can return what it ends up looking like.
        var item = _db.TodoItems.Find(itemId);
        if (item is null) return NotFound();

        // Only use non-null fields of the POST body, so e.g. we can complete
        // a TODO item without accidentally changing its title or content.
        if (body.title is not null) item.Title = body.title;
        if (body.content is not null) item.Content = body.content;
        item.Completed = body.completed;

        _db.SaveChanges();
        return Ok(item);
    }

}


// The expected format of the POST body of the Create endpoint.
// Fun fact, I believe that the fact that our create/update endpoints
// are using totally separate structures from TodoItem means that we're
// doing what Martin Fowler calls CQS, "Command Query Separation".
public record CreateBody(string title);

// The expected format of the POST body of the Update endpoint.
public record UpdateBody(string title, string content, bool completed);

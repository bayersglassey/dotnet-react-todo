using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class BackendDbContext : DbContext {

    public DbSet<TodoItem> TodoItems { get; set; }

    public BackendDbContext(DbContextOptions<BackendDbContext> options) : base(options) {}

}

public class TodoItem {

    // The primary keys are integers.
    // We could do strings instead, and generate UUIDs or something, which
    // is more "secure" in the sense that it's harder for users to try and
    // guess at valid keys... but this is just a TODO app, after all.
    // Plus, even UUIDs give away some information. If security's your goal,
    // there are various cyphers and things which can be used.
    // Also, having an indexed, auto-incrementing field is handy for making
    // sure the TODO items always appear in the same order in the frontend.
    // If we didn't want to use (abuse?) the PK for that, we could add a
    // separate "order" field, give the user the ability to re-order their
    // items, etc.
    // We would want to throw an Index attribute on TodoItem for that field,
    // and stick DatabaseGeneratedOption.Computed on the field itself.
    [Key]
    public int ItemId { get; set; }

    public required string Title { get; set; }

    public required string Content { get; set; }

    // When a TODO item is completed, we just flip this boolean.
    // That's pretty simple; more complicated things we could do would be
    // e.g. to add a CompletedAt datetime field.
    // But then you run into design questions like, "can the user un-complete
    // a TODO item?.. and if so, does CompletedAt get set to null, or do we
    // want to start tracking "completions" in a separate table?"
    // I'd like to keep the TODO aspects of this project simple, so I can
    // maybe have time for interesting things like an HL7 listener...
    // So, a plain old boolean field it is!
    public bool Completed { get; set; }

    // We could add a CreatedAt field, although then we need to decide where
    // to populate it.
    // We could have the database auto-populated it, using
    // DatabaseGeneratedOption.Computed, or we could have the class
    // constructor auto-populate it.
    // But if I add the field, then I'll feel like I need to render it in the
    // frontend, and I'd prefer to keep things simple for now...
    //public DateTime? CreatedAt { get; set; }

}

// The expected format of the POST body of the Create endpoint.
// Fun fact, I believe that the fact that our create/update endpoints
// are using totally separate structures from TodoItem means that we're
// doing what Martin Fowler calls CQS, "Command Query Separation".
public class TodoItemCreateBody {
    public required string Title { get; set; }
    public required string Content { get; set; }
}

// The expected format of the POST body of the Update endpoint.
public class TodoItemUpdateBody {
    public string? Title { get; set; }
    public string? Content { get; set; }
    public bool? Completed { get; set; }
}

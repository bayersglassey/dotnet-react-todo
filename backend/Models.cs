using Microsoft.EntityFrameworkCore;

namespace backend.Models;

public class BackendDbContext : DbContext {
    public DbSet<TodoItem> TodoItems { get; set; }
}

public class TodoItem {
    public int ItemId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public bool Complete { get; set; }
}

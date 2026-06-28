using Microsoft.EntityFrameworkCore;

namespace backend.Models;

public class BackendDbContext : DbContext {
    public DbSet<TodoItem> TodoItems { get; set; }
    public BackendDbContext(DbContextOptions<BackendDbContext> options) : base(options) {}
}

public class TodoItem {
    public required int ItemId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public bool Complete { get; set; }
}

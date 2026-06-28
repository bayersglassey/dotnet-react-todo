using Microsoft.EntityFrameworkCore;
using backend.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString(
    "DefaultConnection");
builder.Services.AddDbContext<BackendDbContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

// To be production-ready, we would want to add at least the following:
//app.UseHttpsRedirection();
//app.UseAuthorization();

app.Run();

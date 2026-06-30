using Microsoft.EntityFrameworkCore;
using backend.Models;

var builder = WebApplication.CreateBuilder(args);

// Keeping it simple for now, hardcoded SQLite database.
// Maybe at some point, we would want to scale our app, and add support for
// SQL Server in staging/production.
// We could keep around support for SQLite for testing and development, which
// I assume would mean adding an if-statement here, with SQLite and SQL Server
// paths for db configuration.
// Although SQLite's SQL dialect is kind of weird; from my Django days, I know
// it's sometimes not worth trying to support it in addition to something else
// (it was Postgres in my case).
// Presumably you can just run SQL Server locally.
// I know when working with Python+Postgres projects, it's even possible to
// have unit test fixtures which spin up little Postgres databases, so you
// know your code is being tested with exactly the same SQL queries and
// behaviour it would use in production.
var connectionString = builder.Configuration.GetConnectionString(
    "DefaultConnection");
builder.Services.AddDbContext<BackendDbContext>(options =>
    options.UseSqlite(connectionString));

// Needed for app.MapControllers() below
builder.Services.AddControllers();

// Authentication & authorization:
// I'm trying to learn .NET Core under a deadline here, so I'm not going to
// get too deep into this right now.
// But at a high level, let's say our TODO app is a SaaS product, so our
// clients will access the frontend, which will cause their browser to make
// calls to the backend, which is deployed in the cloud.
// So, we need a login page, maybe with some kind of SSO support, using OpenID
// Connect or Okta or whatever.
// Somehow or other, the login process should result in a token being saved
// in user's cookies, so it gets sent with every call they make to the API.
// It looks to me like AddAuthentication().AddJwtBearer() is the middleware
// thing we would use on the backend, and then we would map JWT tokens to
// users (presumably corresponding to a User model in the db).
// In Django, there's support for users and even sessions out of the box; I
// searched for a .NET Core equivalent, and found "Session and state management
// in ASP.NET Core", which mentions builder.Services.AddSession().
// Anyway, for now, we don't do any of that: each instance of our app just
// has a global set of unsecured TODO items living in an SQLite database. :)
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/security?view=aspnetcore-10.0
//builder.Services.AddAuthentication().AddJwtBearer();
//builder.Services.AddAuthorization();

// Allowing our frontend to hit our backend when running locally.
// https://learn.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-10.0
var CorsPolicyName = "_cors_policy";
builder.Services.AddCors(options => {
    options.AddPolicy(
        name: CorsPolicyName,
        policy => {
            policy.WithOrigins("http://localhost:3000");
        }
    );
});

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    // Make sure our frontend can hit our backend when running locally!..
    app.UseCors(CorsPolicyName);
}

// Logging isn't configured here, it's configured in appsettings.json and
// appsettings.Development.json, which were produced for me by `dotnet new`.
// So all I know of at the moment is that we can configure default logging
// levels there (the usual INFO, WARNING, etc).
// But reading the docs, I see that there is an ILogger interface and a whole
// system for configuring its behaviour (similar to Java, I believe?.. I have
// lots of experience with Python's logging module, which is based on Java's
// logging system, and I have done some Java work as well, where I recall
// working with log4j).
// If we were going to deploy this app to the cloud somewhere, we might want
// to have it do something with its logs other than writing them to stderr,
// e.g. we might want to send them to a service like Sentry or Datadog.
// These days I'm used to deploying apps in GKE (Google Cloud's managed
// Kubernetes-as-a-service), where you do indeed dump logs to stderr, but
// formatted as single lines of JSON, which are picked up by a log collecting
// agent.
// A long time ago, I was used to dumping logs to stderr, and having a cron
// job periodically grab the logfile and gzip it, leaving an empty one behind
// for the app to continue appending to!..
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-10.0
//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();

app.MapGet("/error-test", () => {
    /* Just testing what .NET Core does with uncaught exceptions. :)

        $ curl http://localhost:3001/error-test
        System.Exception: What does our app do with an uncaught exception?
           at Program.<>c.<<Main>$>b__0_1() in /home/bag/ownrepos/function-health/backend/Backend.cs:line 52
           at lambda_method1(Closure, Object, HttpContext)
           at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddlewareImpl.Invoke(HttpContext context)

        HEADERS
        =======
        Accept: ...
        Host: localhost:3001
        User-Agent: curl/8.5.0

    ...and the following is logged to stderr:

        fail: Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware[1]
              An unhandled exception has occurred while executing the request.
              System.Exception: What does our app do with an uncaught exception?
                 at Program.<>c.<<Main>$>b__0_1() in /home/bag/ownrepos/function-health/backend/Backend.cs:line 52
                 at lambda_method1(Closure, Object, HttpContext)
                 at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddlewareImpl.Invoke(HttpContext context)

    */
    throw new Exception("What does our app do with an uncaught exception?");
});

// We would want to add this in production!
// For now, I'm producing an MVP under a deadline, so I'm going to keep
// things simple.
//app.UseHttpsRedirection();
//app.UseHSTS();

// See Controllers.cs for the controllers, i.e. the API endpoints.
// https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-10.0#ar6
app.MapControllers();

app.Run();

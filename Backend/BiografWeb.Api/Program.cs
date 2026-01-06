using BiografWeb.Infrastructure;
using BiografWeb.Application.Movies;
using BiografWeb.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using BiografWeb.Infrastructure.Seed;

var builder = WebApplication.CreateBuilder(args);

// Swagger for OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Controllers
builder.Services.AddControllers();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// DI registrations
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<BiografWeb.Application.Auditoriums.IAuditoriumRepository, AuditoriumRepository>();
builder.Services.AddScoped<BiografWeb.Application.Auditoriums.IAuditoriumService, BiografWeb.Application.Auditoriums.AuditoriumService>();
builder.Services.AddScoped<BiografWeb.Application.TicketTypes.ITicketTypesRepository, TicketTypesRepository>();
builder.Services.AddScoped<BiografWeb.Application.TicketTypes.ITicketTypesService, BiografWeb.Application.TicketTypes.TicketTypesService>();
builder.Services.AddScoped<BiografWeb.Application.Screenings.IScreeningsRepository, ScreeningsRepository>();
builder.Services.AddScoped<BiografWeb.Application.Screenings.IScreeningsService, BiografWeb.Application.Screenings.ScreeningsService>();
builder.Services.AddScoped<BiografWeb.Application.Users.IUsersRepository, UsersRepository>();
builder.Services.AddScoped<BiografWeb.Application.Users.IUsersService, BiografWeb.Application.Users.UsersService>();
builder.Services.AddScoped<BiografWeb.Application.Bookings.IBookingsRepository, BookingsRepository>();
builder.Services.AddScoped<BiografWeb.Application.Bookings.IBookingsService, BiografWeb.Application.Bookings.BookingsService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Flags for seeding
var shouldSeed = string.Equals(System.Environment.GetEnvironmentVariable("SEED_DB"), "true", System.StringComparison.OrdinalIgnoreCase);
var seedOnly = string.Equals(System.Environment.GetEnvironmentVariable("SEED_ONLY"), "true", System.StringComparison.OrdinalIgnoreCase);

// Ensure database schema is up to date on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (shouldSeed)
    {
        await SeedData.ApplyAsync(db);
    }
}

if (seedOnly)
{
    return;
}

app.MapControllers();

app.Run();
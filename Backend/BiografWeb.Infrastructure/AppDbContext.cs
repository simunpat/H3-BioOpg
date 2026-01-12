using BiografWeb.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BiografWeb.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Auditorium> Auditoriums => Set<Auditorium>();
    public DbSet<TicketType> TicketTypes => Set<TicketType>();
    public DbSet<Screening> Screenings => Set<Screening>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingSeat> BookingSeats => Set<BookingSeat>();
    public DbSet<BookingItem> BookingItems => Set<BookingItem>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                SetPropertyIfExists(entry, "CreatedAt", now);
                SetPropertyIfExists(entry, "UpdatedAt", now);
            }
            else if (entry.State == EntityState.Modified)
            {
                SetPropertyIfExists(entry, "UpdatedAt", now);
            }
        }
    }

    private static void SetPropertyIfExists(EntityEntry entry, string propertyName, DateTime value)
    {
        var prop = entry.Properties.FirstOrDefault(p => p.Metadata.Name == propertyName);
        if (prop is not null)
        {
            prop.CurrentValue = value;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Movie>(e =>
        {
            e.ToTable("movies");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.Property(x => x.Genre).HasMaxLength(100).IsRequired();
            e.Property(x => x.CreatedAt).IsRequired();
            e.Property(x => x.UpdatedAt).IsRequired();
        });

        modelBuilder.Entity<Auditorium>(e =>
        {
            e.ToTable("auditoriums");
            e.HasKey(a => a.Id);
            e.Property(a => a.Id).ValueGeneratedOnAdd();
            e.Property(a => a.Name).HasMaxLength(200).IsRequired();
            e.Property(a => a.CreatedAt).IsRequired();
            e.Property(a => a.UpdatedAt).IsRequired();
        });

        modelBuilder.Entity<TicketType>(e =>
        {
            e.ToTable("ticket_types");
            e.HasKey(t => t.Id);
            e.Property(t => t.Id).ValueGeneratedOnAdd();
            e.Property(t => t.Name).HasMaxLength(100).IsRequired();
            e.Property(t => t.Multiplier).HasColumnType("decimal(10,2)");
            e.Property(t => t.CreatedAt).IsRequired();
            e.Property(t => t.UpdatedAt).IsRequired();
        });

        modelBuilder.Entity<Screening>(e =>
        {
            e.ToTable("screenings");
            e.HasKey(s => s.Id);
            e.Property(s => s.Id).ValueGeneratedOnAdd();
            e.Property(s => s.Price).HasColumnType("decimal(10,2)");
            e.Property(s => s.StartTime).IsRequired();
            e.Property(s => s.CreatedAt).IsRequired();
            e.Property(s => s.UpdatedAt).IsRequired();
        });

        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("users");
            e.HasKey(u => u.Id);
            e.Property(u => u.Id).ValueGeneratedOnAdd();
            e.Property(u => u.Email).HasMaxLength(200).IsRequired();
            e.Property(u => u.IsAdmin).IsRequired();
            e.Property(u => u.PasswordHash).HasMaxLength(512).IsRequired();
            e.Property(u => u.CreatedAt).IsRequired();
            e.Property(u => u.UpdatedAt).IsRequired();
        });

        modelBuilder.Entity<Booking>(e =>
        {
            e.ToTable("bookings");
            e.HasKey(b => b.Id);
            e.Property(b => b.Id).ValueGeneratedOnAdd();
            e.Property(b => b.TotalPrice).HasColumnType("decimal(10,2)");
            e.Property(b => b.CreatedAt).IsRequired();
            e.Property(b => b.UpdatedAt).IsRequired();
        });

        modelBuilder.Entity<BookingSeat>(e =>
        {
            e.ToTable("booking_seats");
            e.HasKey(s => new { s.BookingId, s.Row, s.Number });
            e.Property(s => s.Row).IsRequired();
            e.Property(s => s.Number).IsRequired();
            e.HasOne<Booking>()
                .WithMany(b => b.Seats)
                .HasForeignKey(s => s.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
            e.Property(s => s.CreatedAt).IsRequired();
            e.Property(s => s.UpdatedAt).IsRequired();
        });

        modelBuilder.Entity<BookingItem>(e =>
        {
            e.ToTable("booking_items");
            e.HasKey(i => i.Id);
            e.Property(i => i.Id).ValueGeneratedOnAdd();
            e.HasOne<Booking>()
                .WithMany(b => b.Items)
                .HasForeignKey(i => i.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
            e.Property(i => i.CreatedAt).IsRequired();
            e.Property(i => i.UpdatedAt).IsRequired();
        });
    }
}

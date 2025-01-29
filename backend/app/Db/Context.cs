namespace Services;

public class Context(DbContextOptions<Context> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<VmBooking> VmBookings => Set<VmBooking>();
    public DbSet<VmBookingExtention> VmBookingExtention => Set<VmBookingExtention>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User
        modelBuilder.Entity<User>()
            .Property(x => x.Email)
            .IsRequired()
            .HasDefaultValue("");

        modelBuilder.Entity<User>()
            .Property(x => x.Name)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(x => x.Surname)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(x => x.Password)
            .IsRequired()
            .HasDefaultValue("");

        // Vm booking
        modelBuilder.Entity<VmBooking>()
            .HasOne(b => b.Owner)
            .WithMany()
            .HasForeignKey("OwnerId");

        modelBuilder.Entity<VmBooking>()
            .HasOne(b => b.Assigned)
            .WithMany()
            .HasForeignKey("AssignedId");

        modelBuilder.Entity<VmBooking>()
            .HasMany(b => b.Extentions)
            .WithOne()
            .HasForeignKey("BookingId");
        
    }

    static public void MakeMigration(WebApplication app)
    {
        using IServiceScope scopeDb = app.Services.CreateScope();
        Context dbContext = scopeDb.ServiceProvider.GetRequiredService<Context>();
        bool wasCreated = dbContext.Database.EnsureCreated();
    }
}
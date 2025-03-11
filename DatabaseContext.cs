using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class DatabaseContext : IdentityDbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    // ✅ Voeg de Environments tabel toe aan de database
    public DbSet<Environment2D> Environments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ✅ Zorg ervoor dat de relatie tussen gebruikers en werelden klopt
        builder.Entity<Environment2D>()
            .HasOne<IdentityUser>() // 🔹 Een Environment heeft één User
            .WithMany() // 🔹 Een User kan meerdere Environments hebben
            .HasForeignKey(e => e.UserId) // 🔹 Koppel de UserId als Foreign Key
            .OnDelete(DeleteBehavior.Cascade); // 🔹 Verwijder werelden als gebruiker verwijderd wordt
    }
}

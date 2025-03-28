using Microsoft.EntityFrameworkCore;
using WpfApp2.Data;
using WpfApp2.Shared.Classes;

namespace WpfApp2.Shared.DataAccess;

public class CountryService
{
    public bool SimulateFailures { get; set; } = false;
    public double FailureProbability { get; set; } = 0.3;
    public double FailureProbabilityOnLoad { get; set; } = 0.3;
    private static readonly Random RandomGenerator = new();

    private static AppDbContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite("Data Source=Inventory.db");
        return new AppDbContext(optionsBuilder.Options);
    }

    public async Task<List<Country>> GetAllAsync()
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbabilityOnLoad)
            throw new Exception("Simulated database failure during GetAllCountrysAsync");

        await using var context = CreateDbContext();
        return await context.Countries.ToListAsync();
    }

    public async Task<Country> AddAsync(Country country)
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbability)
            throw new Exception("Simulated database failure during AddCountryAsync");

        await using var context = CreateDbContext();
        context.Countries.Add(country);
        await context.SaveChangesAsync();
        return country;
    }

    public async Task<Country> UpdateAsync(Country country)
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbability)
            throw new Exception("Simulated database failure during UpdateCountryAsync");

        await using var context = CreateDbContext();
        context.Countries.Update(country);
        await context.SaveChangesAsync();
        return country;
    }

    public async Task<Country?> DeleteAsync(int id)
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbability)
            throw new Exception("Simulated database failure during DeleteCountryAsync");

        await using var context = CreateDbContext();
        var country = await context.Countries.FindAsync(id);
        if (country == null) return country;

        context.Countries.Remove(country);
        await context.SaveChangesAsync();
        return country;
    }
}
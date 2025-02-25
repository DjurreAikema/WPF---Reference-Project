using Microsoft.EntityFrameworkCore;
using WpfApp1.Shared.Classes;

namespace WpfApp1.Data;

public class SnackDbContextV2(DbContextOptions<SnackDbContextV2> options) : DbContext(options)
{
    public DbSet<SnackV2> Snacks { get; set; }
}
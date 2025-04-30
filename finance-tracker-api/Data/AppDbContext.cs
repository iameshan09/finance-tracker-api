using finance_tracker_api.Models;
using Microsoft.EntityFrameworkCore;

namespace finance_tracker_api.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { 
        }

        public DbSet<Transaction> Transactions { get; set; }
    }
}

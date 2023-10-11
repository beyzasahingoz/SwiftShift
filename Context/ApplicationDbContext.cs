using Bitirme.Models;
using Microsoft.EntityFrameworkCore;

namespace Bitirme.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> contextOptions) 
            : base(contextOptions)
        {

        }

        public DbSet<Employee> Employees { get; set; }

    }
}

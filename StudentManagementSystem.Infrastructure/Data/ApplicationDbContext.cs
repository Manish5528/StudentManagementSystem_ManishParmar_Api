using Microsoft.EntityFrameworkCore;

namespace StudentManagementSystem.Infrastructure.Data;

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
    }

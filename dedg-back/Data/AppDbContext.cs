using Microsoft.EntityFrameworkCore;

namespace dedg_back.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // DbSets das entidades serão adicionados aqui conforme as models forem criadas
    // ex: public DbSet<User> Users { get; set; }
}

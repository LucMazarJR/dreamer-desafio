using dedg_back.Models.Entities;
using dedg_back.Models.Enums;

namespace dedg_back.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        if (context.Users.Any())
            return;

        logger.LogWarning("Nenhum usuário encontrado. Criando usuário admin padrão...");

        var admin = new User
        {
            Name = "Admin",
            Email = "admin@ddgroup.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = UserRole.HrAdmin,
            TimeZone = "America/Sao_Paulo",
            IsActive = true
        };

        context.Users.Add(admin);
        await context.SaveChangesAsync();

        logger.LogWarning("Usuário admin criado. Email: admin@ddgroup.com | Senha: Admin@123");
    }
}

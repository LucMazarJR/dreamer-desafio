using dedg_back.Models.Entities;
using dedg_back.Models.Enums;

namespace dedg_back.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        const string adminEmail = "admin@ddgroup.com";
        const string adminPassword = "Admin@123";

        var admin = context.Users.FirstOrDefault(u => u.Email == adminEmail);

        if (admin == null)
        {
            logger.LogWarning("Nenhum usuário admin encontrado. Criando usuário admin padrão...");

            admin = new User
            {
                Name = "Admin",
                Email = adminEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                Role = UserRole.HrAdmin,
                TimeZone = "America/Sao_Paulo",
                IsActive = true
            };

            context.Users.Add(admin);
            await context.SaveChangesAsync();

            logger.LogWarning("Usuário admin criado. Email: {Email} | Senha: {Password}", adminEmail, adminPassword);
            return;
        }

        if (!BCrypt.Net.BCrypt.Verify(adminPassword, admin.PasswordHash))
        {
            logger.LogWarning("Hash do admin inválido. Corrigindo...");
            admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword);
            await context.SaveChangesAsync();
            logger.LogWarning("Hash do admin corrigido.");
        }
    }
}

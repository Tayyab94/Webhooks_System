using Microsoft.EntityFrameworkCore;
using WebHook.API.Data;

namespace WebHook.API.Extensions
{
    public static class WebApplicationExtensions
    {
        public static async Task ApplyMigrationAsync(this WebApplication webApplication)
        {
            using var scope = webApplication.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<WebhookContext>();
            await dbContext.Database.MigrateAsync();
        }
    }
}

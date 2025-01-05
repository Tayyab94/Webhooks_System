using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design.Internal;
using WebHook.API.Models;

namespace WebHook.API.Data
{
    internal sealed class WebhookContext(DbContextOptions<WebhookContext>options):DbContext(options)
    {
        public DbSet<Order> Orders { get; set; }

        public DbSet<WebhookSubscription> WebhookSubscriptions { get;set; }
        public DbSet<WebhookDeliveryAttempt> WebhookDeliveryAttempts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Order>(s =>
            {
                s.HasKey(s => s.Id);
                s.ToTable("orders");
            });

            modelBuilder.Entity<WebhookSubscription>(s =>
            {
                s.HasKey(s => s.Id);
                s.ToTable("subscriptions","webhooks");
            });

            modelBuilder.Entity<WebhookDeliveryAttempt>(s =>
            {
                s.HasKey(s => s.Id);
                s.ToTable("delivery_Attempts", "webhooks");

                s.HasOne<WebhookSubscription>().WithMany().HasForeignKey(s =>s.WebhookSubscriptionId);
            });
        }
    }
}

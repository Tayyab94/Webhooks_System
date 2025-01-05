using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using WebHook.API.Data;
using WebHook.API.Extensions;
using WebHook.API.Models;
using WebHook.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new() { Title = "WebHook.API", Version = "v1" });
});


builder.Services.AddScoped<WebhookDispatcher>();
builder.Services.AddDbContext<WebhookContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("webhooks"));
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("openai/v1.json", "WebHook.API");
    });


    await app.ApplyMigrationAsync();
}

app.UseHttpsRedirection();


app.MapPost("webhooks/subscription", async (WebhookContext context, 
    CreateWebhookRequest request) =>
{
    var subscription = new WebhookSubscription(Guid.NewGuid(), request.EventType, request.WebhookUrl, DateTime.UtcNow);
    context.WebhookSubscriptions.Add(subscription);
    await context.SaveChangesAsync();
    return Results.Ok(subscription);
});

app.MapPost("/Orders",
    async(WebhookContext context,
    CreateOrderRequest request, 
    WebhookDispatcher webhookDispatcher) =>
{
    var order = new Order(Guid.NewGuid(), request.CustomerName, request.Amount, DateTime.UtcNow);
    context.Orders.Add(order);
    await context.SaveChangesAsync();
    await webhookDispatcher.DispatcherAsync("Order.Created", order);

    return Results.Ok(order);
}).WithTags("Orders");


app.MapGet("/Orders",async(WebhookContext context) =>
{
    return Results.Ok(await context.Orders.ToListAsync());
}).WithTags("Orders");

app.Run();

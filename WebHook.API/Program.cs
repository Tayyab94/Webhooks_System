using Microsoft.AspNetCore.Builder;
using WebHook.API.Models;
using WebHook.API.Repositories;
using WebHook.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new() { Title = "WebHook.API", Version = "v1" });
});

builder.Services.AddSingleton<InMemoryOrderRepository>();
builder.Services.AddSingleton<InMemoryWebhookSubscriptionRepository>();
builder.Services.AddHttpClient<WebhookDispatcher>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost("webhooks/subscription", (InMemoryWebhookSubscriptionRepository 
    repository, 
    CreateWebhookRequest request) =>
{
    var subscription = new WebhookSubscription(Guid.NewGuid(), request.EventType, request.WebhookUrl, DateTime.UtcNow);
    repository.Add(subscription);
    return Results.Ok(subscription);
});

app.MapPost("/Orders",
    async(InMemoryOrderRepository orderRepo,
    CreateOrderRequest request, 
    WebhookDispatcher webhookDispatcher) =>
{
    var order = new Order(Guid.NewGuid(), request.CustomerName, request.Amount, DateTime.UtcNow);
    orderRepo.Add(order);

    await webhookDispatcher.DispatcherAsync("Order.Created", order);

    return Results.Ok(order);
}).WithTags("Orders");


app.MapGet("/Orders",(InMemoryOrderRepository orderRepo) =>
{
    return Results.Ok(orderRepo.GetAll());
}).WithTags("Orders");

app.Run();

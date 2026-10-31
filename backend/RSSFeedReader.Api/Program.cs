using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using RSSFeedReader.Api.Subscriptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<SubscriptionStore>();
builder.Services.AddCors(options =>
{
    var frontendOrigin = builder.Configuration["Cors:AllowedOrigin"]
        ?? throw new InvalidOperationException("Cors:AllowedOrigin must be configured.");

    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(frontendOrigin)
            .WithMethods("GET", "POST")
            .AllowAnyHeader());
});

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred."
        });
    });
});

app.UseCors("Frontend");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/api/subscriptions", (SubscriptionStore store) =>
    store.GetAll().Select(subscription => subscription.Url).ToArray());

app.MapPost(
    "/api/subscriptions",
    Results<Ok<SubscriptionResponse>, ProblemHttpResult> (AddSubscriptionRequest? request, SubscriptionStore store) =>
    {
        if (string.IsNullOrWhiteSpace(request?.Url))
        {
            return TypedResults.Problem(
                title: "A non-empty subscription URL is required.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var subscription = new Subscription(request.Url);
        store.Add(subscription);
        return TypedResults.Ok(new SubscriptionResponse(subscription.Url));
    });

app.Run();

public partial class Program { }

internal sealed record AddSubscriptionRequest(string? Url);

internal sealed record SubscriptionResponse(string Url);

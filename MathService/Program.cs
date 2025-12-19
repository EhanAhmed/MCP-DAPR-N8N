using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Routing;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// ======================
// BUSINESS ENDPOINTS
// ======================

// Sum two numbers
app.MapGet("/sum", (int a, int b) => new { result = a + b })
   .WithName("SumNumbers")
   .WithMetadata("Add two numbers together") // description
   .WithMetadata(new { name = "a", type = "int" })
   .WithMetadata(new { name = "b", type = "int" });

// Multiply two numbers
app.MapPost("/multiply", (int a, int b) => new { result = a * b })
   .WithName("MultiplyNumbers")
   .WithMetadata("Multiply two numbers") // description
   .WithMetadata(new { name = "a", type = "int" })
   .WithMetadata(new { name = "b", type = "int" });

// ======================
// AUTO-GENERATED TOOL LIST
// ======================
app.MapGet("/__tools", (HttpContext context) =>
{
    var toolList = app.Services.GetRequiredService<EndpointDataSource>()
        .Endpoints
        .Where(e => e is RouteEndpoint)
        .Cast<RouteEndpoint>()
        .Where(r => r.RoutePattern.RawText != "/__tools")
        .Select(r => new
        {
            name = $"HTTP: {string.Join(",", r.Metadata
                        .OfType<Microsoft.AspNetCore.Routing.HttpMethodMetadata>()
                        .FirstOrDefault()?.HttpMethods ?? new[] { "GET" })} {r.RoutePattern.RawText}",
            // description from string metadata
            description = r.Metadata
                            .OfType<object>()
                            .FirstOrDefault(m => m is string)
                            ?.ToString()
                            ?? $"Auto-generated tool for {r.RoutePattern.RawText}",
            methods = r.Metadata
                        .OfType<Microsoft.AspNetCore.Routing.HttpMethodMetadata>()
                        .FirstOrDefault()?.HttpMethods ?? new[] { "GET" },
            // parameters from object metadata
            parameters = r.Metadata
                          .Where(m => m.GetType().GetProperty("name") != null)
                          .Select(p => new { 
                              name = p.GetType().GetProperty("name")!.GetValue(p), 
                              type = p.GetType().GetProperty("type")!.GetValue(p) 
                          })
                          .Cast<object>()
                          .ToArray()
        })
        .ToArray();

    return Results.Json(toolList);
});

app.Run();

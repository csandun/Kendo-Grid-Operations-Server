using KendoGridOperations;
using KendoGridOperations.Models;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using QueryDesignerCore;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMvc(option => option.EnableEndpointRouting = false)
              .AddNewtonsoftJson(settings =>
              settings.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<NorthwindContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
       new WeatherForecast
       (
           DateTime.Now.AddDays(index),
           Random.Shared.Next(-20, 55),
           summaries[Random.Shared.Next(summaries.Length)]
       ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");


app.MapPost("/filter", async (NorthwindContext context, FilterContainer filter) =>
{
    var (query, total) = context.Customers.AsNoTracking().AsQueryable<Customers>()
         .Include(o => o.Orders)
         //.ThenInclude(o => o.ShipViaNavigation)
         .ThenInclude(o => o.OrderDetails)
         .ThenInclude(o => o.Product)
         .Request<Customers>(filter);
    
    var data = await query.ToListAsync();
    var pageCount = (int)Math.Ceiling((float)total / (float)filter.Take);
    var pageNo = ((int)(filter.Skip / filter.Take)) + 1;
    var take = filter.Take;
    var skip = filter.Skip;
    return new { data, total, pageCount, pageNo, take, skip };
})
.WithName("FilterCustomer");


app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
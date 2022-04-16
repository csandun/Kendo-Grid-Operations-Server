using KendoGridOperations;
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



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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


app.MapGet("/get", async (NorthwindContext context) =>
{
    var filter = new FilterContainer
    {
        Filter = new Filter
        {
            Logic = TreeFilterType.Or,
            Filters = new List<Filter>
            {
                new Filter
                {
                    Logic = TreeFilterType.Or,
                    Filters = new List<Filter>
                    {
                        new Filter
                        {
                            Field = "Orders.OrderDetails.Product.ProductId",
                            Operator = WhereFilterType.Equal,
                            Value = 1
                        }
                    }
                }
            }
        }
    };


    var customers = await context.Customers
        .Include(o => o.Orders)
        //.ThenInclude(o => o.ShipViaNavigation)
        .ThenInclude(o => o.OrderDetails)
        .ThenInclude(o => o.Product)
        .Request(filter)
        .ToListAsync();    
    return customers;
})
.WithName("Get ");


app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
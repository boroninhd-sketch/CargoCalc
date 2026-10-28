using CargoCalc.Data;
using CargoCalc.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("CargoCalc")
        ?? "Data Source=cargocalc.db"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Customers.Any())
    {
        var customer = new Customer
        {
            Name = "ООО ТрансЛогистика",
            Phone = "+7 900 111-22-33"
        };
        var carrier1 = new Carrier
        {
            Name = "Быстрая доставка",
            Phone = "+7 900 222-33-44"
        };
        var carrier2 = new Carrier
        {
            Name = "Надёжный перевозчик",
            Phone = "+7 900 333-44-55"
        };

        db.Customers.Add(customer);
        db.Carriers.AddRange(carrier1, carrier2);
        db.SaveChanges();

        var shipment = new Shipment
        {
            FromCity = "Екатеринбург",
            ToCity = "Новосибирск",
            CargoDescription = "Коробки с оборудованием",
            WeightKg = 500,
            CustomerId = customer.Id
        };

        db.Shipments.Add(shipment);
        db.SaveChanges();

        db.Offers.AddRange(
            new Offer { ShipmentId = shipment.Id, CarrierId = carrier1.Id, Price = 18500, DeliveryHours = 30 },
            new Offer { ShipmentId = shipment.Id, CarrierId = carrier2.Id, Price = 21000, DeliveryHours = 24 }
        );
        db.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.Run();

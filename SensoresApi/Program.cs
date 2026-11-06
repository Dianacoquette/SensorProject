using Microsoft.EntityFrameworkCore;
using SensoresApi.Data;
using SensoresApi.Hubs;
using SensoresApi.Models;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .SetIsOriginAllowed(_ => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost.UseUrls("http://0.0.0.0:5287");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.MapPost("/api/lecturas", async (Lectura lectura, AppDbContext db, IHubContext<SensoresHub> hub) =>
{
    lectura.Fecha = DateTime.UtcNow;
    db.Lecturas.Add(lectura);
    await db.SaveChangesAsync();
    await hub.Clients.All.SendAsync("RecibirLectura", lectura);
    return Results.Created($"/api/lecturas/{lectura.Id}", lectura);
});

app.MapGet("/api/lecturas", async (AppDbContext db) =>
{
    var lecturas = await db.Lecturas
        .OrderByDescending(l => l.Fecha)
        .Take(50)
        .ToListAsync();
    return Results.Ok(lecturas);
});

app.MapGet("/api/lecturas/ultima", async (AppDbContext db) =>
{
    var ultima = await db.Lecturas.OrderByDescending(l => l.Fecha).FirstOrDefaultAsync();
    return ultima is not null ? Results.Ok(ultima) : Results.NotFound();
});

app.MapHub<SensoresHub>("/hubs/sensores");

app.Run();
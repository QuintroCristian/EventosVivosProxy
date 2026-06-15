using EventosVivos.Application.Interfaces.Services;
using EventosVivos.Application.UseCases.Services;
using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Interfaces.Repositories;
using EventosVivos.Infrastructure.Persistence;
using EventosVivos.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// 1. Base de datos en memoria
builder.Services.AddDbContext<EventosVivosDbContext>(options =>
    options.UseInMemoryDatabase("EventosVivosDB"));

// 2. Inyección de dependencias
builder.Services.AddScoped<IEventoRepository, EventoRepository>();
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();
builder.Services.AddScoped<IEventoService, EventoService>();
builder.Services.AddScoped<IReservaService, ReservaService>();

// 3. CORS para Angular (http://localhost:4200)
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAngular", policy =>
//        policy.WithOrigins("http://localhost:4200")
//              .AllowAnyHeader()
//              .AllowAnyMethod());
//});
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "EventosVivos API",
        Version     = "v1",
        Description = "API REST para la gestión de eventos en vivo y sus reservas."
    });
    options.EnableAnnotations();
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// 4. Inicializar base de datos en memoria + seed de prueba
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EventosVivosDbContext>();
    db.Database.EnsureCreated();

    // Seed de Eventos (la BD InMemory se vacía con cada sesión de depuración)
    if (!db.Eventos.Any())
    {
        var plaza     = db.Venues.First(v => v.Id == 1); // Plaza Mayor        — cap: 3000
        var teatro    = db.Venues.First(v => v.Id == 2); // Teatro Metropolitano — cap: 1634
        var auditorio = db.Venues.First(v => v.Id == 4); // Auditorio Principal  — cap: 150

        var baseDate = DateTime.UtcNow.Date.AddDays(30);

        db.Eventos.AddRange(
            new Evento(
                "Cumbre de Innovación Tecnológica 2025",
                "Conferencia anual sobre tendencias en IA, cloud computing y desarrollo de software moderno.",
                plaza, 300,
                baseDate.AddHours(18), baseDate.AddHours(22),
                120.00m, TipoEvento.Conferencia),

            new Evento(
                "Taller Práctico de Clean Architecture",
                "Sesión práctica para implementar arquitectura limpia en proyectos .NET con ejemplos reales.",
                auditorio, 50,
                baseDate.AddDays(5).AddHours(9), baseDate.AddDays(5).AddHours(17),
                75.00m, TipoEvento.Taller),

            new Evento(
                "Noche de Jazz en Vivo 2025",
                "Concierto de jazz con artistas locales e internacionales en un ambiente íntimo y exclusivo.",
                teatro, 500,
                baseDate.AddDays(10).AddHours(20), baseDate.AddDays(10).AddHours(23),
                95.00m, TipoEvento.Concierto)
        );

        db.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

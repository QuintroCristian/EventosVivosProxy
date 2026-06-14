using EventosVivos.Application.Interfaces.Services;
using EventosVivos.Application.UseCases.Services;
using EventosVivos.Domain.Interfaces.Repositories;
using EventosVivos.Infrastructure.Persistence;
using EventosVivos.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

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
builder.Services.AddSwaggerGen();

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

//app.UseCors("AllowAngular");
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

using Microsoft.EntityFrameworkCore;
using tr1.v2.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("Connection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMyOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:4200")  // Agrega aquí tu origen de frontend (puede ser tu aplicación Angular/React)
              .AllowAnyMethod()                   // Permitir cualquier método HTTP (GET, POST, PUT, DELETE, etc.)
              .AllowAnyHeader()                   // Permitir cualquier encabezado
              .AllowCredentials();                // Permitir el envío de credenciales (si las necesitas)
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Usar CORS
app.UseCors("AllowMyOrigin");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

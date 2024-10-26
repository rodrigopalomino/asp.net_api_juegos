using Microsoft.AspNetCore.Authentication.Cookies; // Asegúrate de incluir este espacio de nombres
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
        policy.WithOrigins("http://localhost:4200")  // Agrega aquí tu origen de frontend
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Configuración de la autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "token"; // Cambia "MiCookiePersonalizada" al nombre que desees
        options.LoginPath = "/api/usuarios/login"; // Mantén esta línea si deseas redirigir a esta ruta
        options.LogoutPath = "/api/usuarios/logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Duración de la cookie
        options.SlidingExpiration = true; // Renovar la cookie en cada solicitud
    });

builder.Services.AddControllers();
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

// Configurar la redirección HTTPS y habilitar autenticación y autorización
app.UseHttpsRedirection();
app.UseAuthentication();  // Asegúrate de que esté antes de UseAuthorization
app.UseAuthorization();    // Luego aquí

app.MapControllers();

app.Run();

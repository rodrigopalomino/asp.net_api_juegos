using Microsoft.AspNetCore.Authentication.Cookies; // Aseg�rate de incluir este espacio de nombres
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
        policy.WithOrigins("http://localhost:4200")  // Agrega aqu� tu origen de frontend
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Configuraci�n de la autenticaci�n
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "token"; // Cambia "MiCookiePersonalizada" al nombre que desees
        options.LoginPath = "/api/usuarios/login"; // Mant�n esta l�nea si deseas redirigir a esta ruta
        options.LogoutPath = "/api/usuarios/logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Duraci�n de la cookie
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

// Configurar la redirecci�n HTTPS y habilitar autenticaci�n y autorizaci�n
app.UseHttpsRedirection();
app.UseAuthentication();  // Aseg�rate de que est� antes de UseAuthorization
app.UseAuthorization();    // Luego aqu�

app.MapControllers();

app.Run();

using Microsoft.EntityFrameworkCore;
using tr1.v2.Models;

namespace tr1.v2.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Juego> Juegos { get; set; }
        public DbSet<Consola> Consolas { get; set; }
        public DbSet<Genero> Generos { get; set; }
        public DbSet<JuegoConsola> JuegoConsola { get; set; }
        public DbSet<JuegoGenero> JuegoGenero { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar la clave primaria compuesta para JuegoConsola
            modelBuilder.Entity<JuegoConsola>()
                        .HasKey(jc => new { jc.JuegoId, jc.ConsolaId });

            // Configurar la clave primaria compuesta para JuegoGenero
            modelBuilder.Entity<JuegoGenero>()
                        .HasKey(jg => new { jg.JuegoId, jg.GeneroId });

            // Configurar la columna Reseña como TEXT en el modelo Juego
            modelBuilder.Entity<Juego>(entity =>
            {
                entity.Property(e => e.Descripcion)
                      .HasColumnType("TEXT");
            });
        }
    }
}

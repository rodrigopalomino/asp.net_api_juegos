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
        public DbSet<Valoracion> Valoraciones { get; set; }  // DbSet para Valoraciones
        public DbSet<Comentario> Comentarios { get; set; }  // DbSet para Comentarios
        public DbSet<Usuario> Usuarios { get; set; }  // Agregar DbSet para Usuarios

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar la clave primaria compuesta para JuegoConsola
            modelBuilder.Entity<JuegoConsola>()
                        .HasKey(jc => new { jc.JuegoId, jc.ConsolaId });

            // Configurar la clave primaria compuesta para JuegoGenero
            modelBuilder.Entity<JuegoGenero>()
                        .HasKey(jg => new { jg.JuegoId, jg.GeneroId });

            // Configurar la columna Descripcion como TEXT en el modelo Juego
            modelBuilder.Entity<Juego>(entity =>
            {
                entity.Property(e => e.Descripcion)
                      .HasColumnType("TEXT");

                // Configurar valores predeterminados para CantLikes, CantDislikes y CantComentarios
                entity.Property(e => e.CantLikes)
                      .HasDefaultValue(0);

                entity.Property(e => e.CantDislikes)
                      .HasDefaultValue(0);

                entity.Property(e => e.CantComentarios)
                      .HasDefaultValue(0);
            });

            // Configuración para la entidad Valoracion
            modelBuilder.Entity<Valoracion>(entity =>
            {
                entity.HasIndex(v => new { v.UsuarioId, v.JuegoId }).IsUnique();
            });

            // Configuración para la entidad Comentario
            modelBuilder.Entity<Comentario>(entity =>
            {
                entity.Property(c => c.Texto)
                      .HasColumnType("TEXT");

                entity.Property(c => c.Fecha)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configuración para la entidad Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();  // Asegurarse que el nombre de usuario sea único
            });
        }

    }
}

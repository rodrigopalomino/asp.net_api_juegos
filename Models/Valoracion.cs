namespace tr1.v2.Models
{
    public class Valoracion
    {
        public int Id { get; set; }
        public required string Tipo { get; set; }

        public required int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public required int JuegoId { get; set; }
        public Juego? Juego { get; set; }
    }
}

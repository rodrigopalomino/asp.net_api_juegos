namespace tr1.v2.DTOs
{
    public class JuegoDTO
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string Descripcion { get; set; }
        public required DateTime Fecha { get; set; }
        public required string Desarrolladora { get; set; }
        public required string UrlImg { get; set; }

        // Listas de IDs para géneros y consolas relacionados con el juego
        public List<int> Generos { get; set; } = new List<int>();
        public List<int> Consolas { get; set; } = new List<int>();

        public int? CantLikes { get; set; }
        public int? CantDislikes { get; set; }
        public int? CantComentarios { get; set; }
    }
}

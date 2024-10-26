namespace tr1.v2.Models
{
    public class Juego
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string Descripcion { get; set; }
        public required DateTime Fecha { get; set; }
        public required string Desarrolladora { get; set; }
        public required string UrlImg { get; set; }
        public int? CantLikes { get; set; }
        public int? CantDislikes { get; set; }
        public int? CantComentarios { get; set; }
    }
}

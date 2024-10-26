namespace tr1.v2.Models
{
    public class JuegoGenero
    {
        public int JuegoId { get; set; }
        public  Juego? Juego { get; set; }

        public int GeneroId { get; set; }
        public  Genero? Genero { get; set; }
    }
}

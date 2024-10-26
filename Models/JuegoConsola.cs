namespace tr1.v2.Models
{
    public class JuegoConsola
    {
        public int JuegoId { get; set; }
        public  Juego? Juego { get; set; }

        public int ConsolaId { get; set; }
        public  Consola? Consola { get; set; }
    }
}

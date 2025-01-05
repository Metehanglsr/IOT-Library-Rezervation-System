namespace IOTWebAPI.Models
{
    public class Masa
    {
        public int MasaId { get; set; }
        public string MasaName { get; set; }
        public MasaDurumu MasaDurumu { get; set; }
        public int? SensorId { get; set; }
        public Sensor? Sensor { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }
    }
    public enum MasaDurumu
    {
        Bos,
        Dolu,
        Rezerve
    }
}

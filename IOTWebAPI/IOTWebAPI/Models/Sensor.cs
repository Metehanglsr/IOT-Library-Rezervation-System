namespace IOTWebAPI.Models
{
    public class Sensor
    {
        public int SensorId { get; set; }
        public string SensorName { get; set; }
        public List<SensorValue> SensorValues { get; set; }
        public int MasaId { get; set; }
        public Masa Masa { get; set; }
    }
}

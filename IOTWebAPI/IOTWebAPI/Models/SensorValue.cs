namespace IOTWebAPI.Models
{
    public class SensorValue
    {
        public int SensorId { get; set; }
        public Sensor Sensor { get; set; }
        public int ValueId { get; set; }
        public Value Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
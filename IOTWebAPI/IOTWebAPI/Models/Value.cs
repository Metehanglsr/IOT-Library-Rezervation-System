namespace IOTWebAPI.Models
{
    public class Value
    {
        public int ValueId { get; set; }
        public double MeasurementValue { get; set; }
        public List<SensorValue> SensorValues { get; set; }
    }
}

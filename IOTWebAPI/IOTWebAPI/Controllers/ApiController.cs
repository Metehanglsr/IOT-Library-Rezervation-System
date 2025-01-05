using IOTWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IOTWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        private readonly IOTDbContext _context;

        public ApiController(IOTDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("SensorVerisiEkle")]
        public async Task<IActionResult> SensorVerisiEkle([FromBody] List<SensorVerisiDto> sensorVerisiList)
        {
            if (sensorVerisiList == null || !sensorVerisiList.Any())
            {
                return BadRequest("Veri eksik.");
            }

            try
            {
                foreach (var sensorVerisi in sensorVerisiList)
                {
                    var value = new Value
                    {
                        MeasurementValue = sensorVerisi.MeasurementValue
                    };
                    _context.Values.Add(value);
                    await _context.SaveChangesAsync();

                    var sensorValue = new SensorValue
                    {
                        SensorId = sensorVerisi.SensorId,
                        ValueId = value.ValueId,
                        Value = value,
                        Timestamp = DateTime.UtcNow
                    };
                    _context.SensorValues.Add(sensorValue);
                    await _context.SaveChangesAsync();
                }

                return Ok("Veri başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Sunucu hatası: {ex.Message}");
            }
        }

    }
}

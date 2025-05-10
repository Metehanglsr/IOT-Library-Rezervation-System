using IOTWebAPI.Models;
using IOTWebAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IOTWebAPI.Controllers
{
    public class MasaController : Controller
    {
        private readonly IOTDbContext _context;
        private readonly DbContextOptions<IOTDbContext> _options;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly Timer _timer;

        public MasaController(IOTDbContext context, DbContextOptions<IOTDbContext> options,IServiceScopeFactory serviceScopeFactory)
        {
            _context = context;
            _options = options;
            _serviceScopeFactory = serviceScopeFactory;
            _timer = new Timer(CheckSensor!, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }


        public async Task<IActionResult> Index(int UserId)
        {
            if (TempData["msg"] != null)
            {
                _ = Baslat30SaniyelikKontrol(UserId);
            }

            var masalar = await _context.Masalar.Include(x => x.User).ToListAsync();
            var user = await _context.Users.Include(x => x.Masa).FirstOrDefaultAsync(x => x.UserId == UserId);
            var viewmodel = new MasaUserIdViewModel
            {
                Masalar = masalar,
                User = user!,
            };
            return View(viewmodel);
        }

        public async Task Baslat30SaniyelikKontrol(int UserId)
        {
            bool oturumBasarili = false;
            try
            {
                var baslangicZamani = DateTime.Now;
               

                    while ((DateTime.Now - baslangicZamani).TotalSeconds < 20)
                    {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<IOTDbContext>();
                        var masa = await context.Masalar
                            .Include(m => m.Sensor)
                            .FirstOrDefaultAsync(x => x.UserId == UserId);

                        if (masa == null)
                        {
                            Console.WriteLine("Masa bulunamadı.");
                            return;
                        }



                    if (masa.MasaDurumu == MasaDurumu.Rezerve)
                        {
                            var sensorValue = await context.SensorValues
                                .Include(x => x.Value)
                                .Where(x => x.SensorId == masa.SensorId)
                                .OrderByDescending(x => x.Timestamp)
                                .FirstOrDefaultAsync();

                            if (sensorValue != null && sensorValue.Value.MeasurementValue <= 8)
                            {
                                masa.MasaDurumu = MasaDurumu.Dolu;
                                masa.UserId = UserId;
                                oturumBasarili = true;
                                await context.SaveChangesAsync();
                                break;
                            }
                        }
                        await Task.Delay(1000);
                    }
                }
                if (!oturumBasarili)
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<IOTDbContext>();
                        var masa = await context.Masalar
                        .Include(m => m.Sensor)
                        .FirstOrDefaultAsync(x => x.UserId == UserId);
                        masa!.MasaDurumu = MasaDurumu.Bos;
                        masa.UserId = null;
                        await context.SaveChangesAsync();
                        RedirectToAction("Index", new { UserId });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata oluştu: {ex.Message}");
            }
        }


        public async Task<IActionResult> MolaAl(int UserId)
        {

            var user = await _context.Users
                                     .FirstOrDefaultAsync(x => x.UserId == UserId);
            HelperMethods.moladaMi = true;
            var timer = new Timer(async _ =>
            {
                await MolaBitti(UserId);
            }, null, TimeSpan.FromSeconds(20), Timeout.InfiniteTimeSpan);
            return RedirectToAction("Index", new { UserId });
        }
        public async Task<IActionResult> MolaBitti(int UserId)
        {
            HelperMethods.moladaMi = false;
            return RedirectToAction("Index", new { UserId });
        }
        public async Task<IActionResult> RandevuAl(int UserId, int MasaId)
        {
            var sensor = await _context.Sensorler
                .Include(s => s.SensorValues)
                .ThenInclude(x=>x.Value)
                .FirstOrDefaultAsync(x => x.MasaId == MasaId);

            var user = await _context.Users.FindAsync(UserId);
            var masa = await _context.Masalar.FirstOrDefaultAsync(x => x.MasaId == MasaId);
            if (user == null || masa == null || sensor == null)
            {
                return NotFound("User, Masa or Sensor not found.");
            }
            if (user.MasaId == null)
            {
                TempData["msg"] = "Zaten bir masada randevunuz mevcut.";
            }
            user.MasaId = masa.MasaId;
            TempData["msg"] = "Randevunuz alındı onaylamak için 30 saniye içerisinde masanıza oturmanız gerekmektedir.";
            masa.MasaDurumu = MasaDurumu.Rezerve;
            masa.UserId = UserId;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index",new { user.UserId });
        }

        private async void CheckSensor(object state)
        {
            if(HelperMethods.moladaMi == true)
            {
                return;
            }
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<IOTDbContext>();
                    var masa = await context.Masalar.Include(m => m.Sensor).Include(x=>x.User).ToListAsync();
                    foreach (var item in masa)
                    {
                        if(item.MasaDurumu == MasaDurumu.Dolu)
                        {

                        var sensorValues = await context.SensorValues
                                                        .Where(x => x.SensorId == item.SensorId)
                                                        .OrderByDescending(x => x.Timestamp)
                                                        .Include(x => x.Value)
                                                        .FirstOrDefaultAsync();
                            if (sensorValues != null && sensorValues.Value.MeasurementValue <= 8)
                            {
                                if (item.User != null)
                                {
                                    item.MasaDurumu = MasaDurumu.Dolu;
                                    HelperMethods.moladaMi = false;
                                }
                            }
                            else
                            {
                                if(!HelperMethods.moladaMi)
                                {
                                    item.MasaDurumu = MasaDurumu.Bos;
                                    item.UserId = null;
                                    item.User!.MasaId = null;
                                }
                            }
                            
                    }
                    }
                        await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata oluştu: {ex.Message}");
            }
        }
        private async Task<bool> CheckSensorValueAsync(Masa masa)
        {
            for (int i = 0; i < 30; i++)
            {
                using (var newContext = new IOTDbContext(_options))
                {
                    var latestSensorValue = await newContext.SensorValues
                        .Include(x => x.Value)
                        .Where(x => x.SensorId == masa.SensorId)
                        .OrderByDescending(x => x.Timestamp)
                        .FirstOrDefaultAsync();

                    if (latestSensorValue?.Value?.MeasurementValue < 5)
                    {
                        return true;
                    }
                }

                await Task.Delay(1000);
            }

            return false;
        }
    }
}
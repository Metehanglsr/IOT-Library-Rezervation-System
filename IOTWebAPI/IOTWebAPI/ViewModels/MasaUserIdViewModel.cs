using IOTWebAPI.Models;

namespace IOTWebAPI.ViewModels
{
    public class MasaUserIdViewModel
    {
        public List<Masa> Masalar { get; set; } = default!;
        public User User { get; set; } = default!;
    }
}

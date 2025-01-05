using System.ComponentModel.DataAnnotations;

namespace IOTWebAPI.Models
{
    public class User
    {
        public int UserId{ get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool moladaMi { get; set; }
        [Required]
        public string UserName { get; set; }
        public string Password { get; set; }
        public int? MasaId { get; set; }
        public Masa? Masa{ get; set; }
    }
}
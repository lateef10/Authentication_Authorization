using System.ComponentModel.DataAnnotations;

namespace AuthenticationAuthorization.Model
{
    public class RegisterUserDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        public string Nationality { get; set; }
        public DateTime? DateOFBirth { get; set; }
        public int RoleId { get; set; } = 1;
    }
}

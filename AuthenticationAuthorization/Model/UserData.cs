using Microsoft.AspNetCore.Identity;

namespace AuthenticationAuthorization.Model
{
    public class UserProfile : IdentityUser
    {
        public string Nationality { get; set; }
        public DateTime? DateOFBirth { get; set; }
    }
}

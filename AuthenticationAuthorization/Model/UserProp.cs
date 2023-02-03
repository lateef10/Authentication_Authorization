namespace AuthenticationAuthorization.Model
{
    public class UserProp
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nationality { get; set; }
        public DateTime? DateOFBirth { get; set; }
        public string PasswordHash { get; set; }
        public int RoleId { get; set; }
        public RoleProp Role { get; set; }
    }
}

namespace AuthenticationAuthorization.Common
{
    public class JwtOptions
    {
        public string JwtKey { get; set; }
        public string JwtIssuer { get; set; }
        public string JwtExpireInDays { get; set; }
    }
}

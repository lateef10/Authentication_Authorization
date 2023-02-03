using AuthenticationAuthorization.Common;
using AuthenticationAuthorization.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationAuthorization.Controllers
{
    public class AccountUserPropController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IPasswordHasher<UserProp> _passwordHasher;
        private readonly JwtOptions _jwtOptions;

        public AccountUserPropController(ApplicationDbContext applicationDbContext, IPasswordHasher<UserProp> passwordHasher, JwtOptions jwtOptions)
        {
            _dbContext = applicationDbContext;
            _passwordHasher = passwordHasher;
            _jwtOptions = jwtOptions;
        }

        [HttpPost("Register")]
        public IActionResult Register([FromBody] RegisterUserDto registerUserDto)
        {
            var newUser = new UserProp
            {
                Email = registerUserDto.Email,
                DateOFBirth = registerUserDto.DateOFBirth,
                RoleId = registerUserDto.RoleId,
                Nationality = registerUserDto.Nationality,
            };
            var passwordHash = _passwordHasher.HashPassword(newUser, registerUserDto.Password);
            newUser.PasswordHash = passwordHash;

            _dbContext.Add(newUser);
            _dbContext.SaveChanges();

            return Ok();
        }

        [HttpPost("Login")]
        public ActionResult Login(string email, string password)
        {
            var user = _dbContext.userProps.Include(u=>u.Role).FirstOrDefaultAsync(u=>u.Email == email).Result;

            if (user == null)
                return BadRequest("Invalid email or password");

            var passVerify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if(passVerify == PasswordVerificationResult.Failed)
                return BadRequest("Invalid email or password");

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.RoleName),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("DateOfBirth", user.DateOFBirth.Value.ToString("MM-dd-yyyy")),
            };

            if (!string.IsNullOrEmpty(user.Nationality))
                claims.Add(new Claim("Nationality", user.Nationality));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.JwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddSeconds(Convert.ToDouble(_jwtOptions.JwtExpireInDays));

            var token = new JwtSecurityToken(
                    _jwtOptions.JwtIssuer,
                    _jwtOptions.JwtIssuer,
                    claims,
                    expires: expires,
                    signingCredentials: creds
                );

            var tokenHandler = new JwtSecurityTokenHandler();

            return Ok(tokenHandler.WriteToken(token));
        }
    }
}

using AuthenticationAuthorization.Common;
using AuthenticationAuthorization.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationAuthorization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountIdentityController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtOptions _jwtOptions;

        public AccountIdentityController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, JwtOptions jwtOptions)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtOptions = jwtOptions;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {
            var newUser = new IdentityUser()
            {
                UserName = registerUserDto.Email,
                Email = registerUserDto.Email
            };
            var result = await _userManager.CreateAsync(newUser, registerUserDto.Password);

            if (!result.Succeeded)
            {
                var errors = new ModelStateDictionary();
                foreach (var err in result.Errors)
                {
                    errors.AddModelError(err.Code, err.Description);
                }
                return new BadRequestObjectResult(new { Message = "User Registration Failed", Errors = errors });
            }

            if (!await _roleManager.RoleExistsAsync("Users"))
            {
                var role = new IdentityRole();
                role.Name = "Users";
                await _roleManager.CreateAsync(role);
            }
            await _userManager.AddToRoleAsync(newUser, "Users");

            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            return Ok(new { Message = "User Registration Successful", EmailTokenConfirmation = confirmationToken });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userManager.FindByNameAsync(email);
            if (user == null)
                return BadRequest("Invalid email or password");

            var passVerify = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (passVerify == PasswordVerificationResult.Failed)
                return BadRequest("Invalid email or password");

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return BadRequest("Please confirm your email");

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                new Claim(ClaimTypes.Name, user.Email),
                //new Claim("DateOfBirth", user.DateOFBirth.Value.ToString("MM-dd-yyyy")),
            };

            //if (!string.IsNullOrEmpty(user.Nationality))
            //claims.Add(new Claim("Nationality", user.Nationality));

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

        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            var user = await _userManager.FindByNameAsync(email);
            if (user == null)
                return BadRequest("Failed to validate email");

            var confirmEmail = await _userManager.ConfirmEmailAsync(user, token);
            if (!confirmEmail.Succeeded)
                return BadRequest("Failed to validate email");

            return Ok(new { Message = "Email address is successfully confirm, you can proceed to login" });
        }

        [HttpPost("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            //You can't destroy JWT token at will but you can do the following
            //Simply remove the token from the client
            //Obviously this does nothing for server side security, but it does stop an attacker by removing the token from existence
            //(ie.they would have to have stolen the token prior to logout).
            //Create a token blocklist: You have a blacklist that holds these tokens until their expiration date is hit but need to check
            //the blacklist on every Auth request call
            //You can keep token expiry times short and refresh them often

            return Ok(new { Message = "Logged Out" });
        }
    }
}

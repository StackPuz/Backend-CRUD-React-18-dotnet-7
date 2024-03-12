using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using BC = BCrypt.Net.BCrypt;
using App.Models;

namespace App.Controllers
{
    public class LoginController : Controller
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public LoginController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserAccount login)
        {
            var user = await _context.UserAccount.FirstOrDefaultAsync(e => e.Name == login.Name);
            if (user != null && user.Active && BC.Verify(login.Password, user.Password)) {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name)
                };
                var roles = _context.UserRole
                .Join(
                    _context.Role,
                    userRole => userRole.RoleId,
                    role => role.Id,
                    (userRole, role) => new { userRole, role }
                )
                .Where(e => e.userRole.UserId == user.Id)
                .Select(e => e.role.Name);
                foreach (var role in roles) {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
                var identity = new ClaimsIdentity(claims);
                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["JWT-Secret"]));  
                var token = new JwtSecurityToken(  
                    expires: DateTime.Now.AddHours(24),
                    claims: claims,  
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)  
                );
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    user = new
                    {
                        name = identity.Name,
                        menu = GetMenu(identity.Claims)
                    }
                });
            }
            var message = (user != null && !user.Active ? "User is disabled" : "Invalid credentials");
            return StatusCode(400, new { message });
        }


        [HttpGet("User")]
        public IActionResult GetUser()
        {
            if (User.Identity.IsAuthenticated) {
                return Ok(new
                {
                    name = User.Identity.Name,
                    menu = GetMenu(User.Claims)
                });
            }
            return StatusCode(401);
        }

        public Array GetMenu(IEnumerable<Claim> claims) {
            var roles = claims.Where(e => e.Type == ClaimTypes.Role).Select(e => e.Value);
            var menus = _configuration.GetSection("Menu").GetChildren().ToList().Select(e => new { Title = e.GetValue<string>("Title"), Path = e.GetValue<string>("Path"), Roles = e.GetValue<string>("Roles"), Show = e.GetValue<bool>("Show") });
            return menus.Where(e => e.Show && (e.Roles == null || e.Roles.Split(",").Any(role => roles.Contains(role)))).Select(e => new { title = e.Title, path = e.Path }).ToArray();
        }

        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            return Ok();
        }

        [HttpPost("ResetPassword")]
        public IActionResult ResetPasswordPost([FromBody] JsonElement body)
        {
            var email = body.GetProperty("email").GetString();
            var user = _context.UserAccount.FirstOrDefault(e => e.Email == email);
            if (user != null) {
                var token = Guid.NewGuid().ToString();
                user.PasswordResetToken = token;
                _context.SaveChanges();
                Util.SentMail("Reset", email, token);
                return Ok();
            }
            else {
                return NotFound();
            }
        }
        
        [HttpGet("ChangePassword/{token}")]
        public IActionResult ChangePassword(string token)
        {
            var user = _context.UserAccount.FirstOrDefault(e => e.PasswordResetToken == token);
            if (user != null) {
                return Ok();
            }
            else {
                return NotFound();
            }
        }

        [HttpPost("ChangePassword/{token}")]
        public IActionResult ChangePasswordPost(string token, [FromBody] JsonElement body)
        {
            var user = _context.UserAccount.FirstOrDefault(e => e.PasswordResetToken == token);
            if (user != null) {
                var password = body.GetProperty("password").GetString();
                user.Password = BC.HashPassword(password);
                user.PasswordResetToken = null;
                _context.SaveChanges();
                return Ok();
            }
            else {
                return NotFound();
            }
        }
    }
}
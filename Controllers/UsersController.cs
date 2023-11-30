using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using semigura.DAL;
using semigura.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserRepository _repository;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger, IConfiguration configuration, UserRepository repository)
        {
            _configuration = configuration;
            _repository = repository;
            _logger = logger;
        }

        public class UserDTO
        {
            public string Account { get; set; } = "";
            public string Password { get; set; } = "";
            public string Role { get; set; } = "";
        }

        [HttpGet]
        public IQueryable<User> Get()
        {
            return _repository.GetAll();
        }

        [HttpPost]
        public async Task<ActionResult> Post(User obj)
        {
            var addedUser = await _repository.Add(obj);
            return Ok(addedUser);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        // NOTE: set cookie authen
        public async Task<ActionResult> LoginWithUserPassword(UserDTO user)
        {
            (var statusCode, string msg, string role) = Login(user);
            if (statusCode == 200)
            {
                await SetCookieAuthen(user.Account, role);
                string stringToken = GenerateToken(user.Account, role);
                return Ok(new { token = stringToken });
            }
            return new ObjectResult(msg) { StatusCode = statusCode };
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("login")]
        // NOTE: not set cookie authen
        public ActionResult LoginWithUserPassword2(string username, string password)
        {
            (var statusCode, string msg, string role) = Login(new UserDTO { Account = username, Password = password });
            if (statusCode == StatusCodes.Status200OK)
            {
                string stringToken = GenerateToken(username, role);
                return new ObjectResult(stringToken) { StatusCode = statusCode };
            }

            return new ObjectResult(msg) { StatusCode = statusCode };
        }

        private (int, string, string) Login(UserDTO user)
        {
            string msg = "Unauthorized!";
            string role = "";
            int statusCodes = StatusCodes.Status401Unauthorized;
            do
            {

                if (user.Account == "admin" && user.Password == "123456")
                {
                    role = "admin";
                    msg = "Default account!";
                    statusCodes = StatusCodes.Status200OK;
                    break;
                }

                var found = _repository.GetAll()
                                .Where(u => u.Account != null && u.Account == user.Account)
                                .Select(u => new { Role = u.Role, HashedPassword = u.HashedPassword })
                                .FirstOrDefault()
                                ;

                if (found == null)
                {
                    msg = "Invalid account!";
                    statusCodes = StatusCodes.Status404NotFound;
                    break;
                }

                if (BCrypt.Net.BCrypt.Verify(user.Password, found.HashedPassword))
                {
                    role = found.Role;
                    statusCodes = StatusCodes.Status200OK;
                }
            } while (false);

            return (statusCodes, msg, role);
        }

        private async Task SetCookieAuthen(string account, string role)
        {
            var identity = new ClaimsIdentity(
                                CookieAuthenticationDefaults.AuthenticationScheme,
                                ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.Name, account));
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(1)
                });
        }

        [HttpGet]
        [Route("me")]
        public ActionResult GetMe()
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User;
            return Ok(new
            {
                account = claimsPrincipal.FindFirstValue(ClaimTypes.Name),
                role = claimsPrincipal.FindFirstValue(ClaimTypes.Role)
            });
        }

        private string GenerateToken(string account, string role)
        {
            var jwtConfig = _configuration.GetSection("Jwt").Get<JwtConfig>();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                    {
                            new Claim("Id", Guid.NewGuid().ToString()),
                            new Claim(ClaimsIdentity.DefaultNameClaimType, account),
                            new Claim(ClaimsIdentity.DefaultRoleClaimType, role),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        }),
                Expires = DateTime.UtcNow.AddMonths(3),
                Issuer = jwtConfig.Issuer,
                Audience = jwtConfig.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.Key)),
                    SecurityAlgorithms.HmacSha512Signature
                    )
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var stringToken = tokenHandler.WriteToken(token);
            return stringToken;
        }
    }
}

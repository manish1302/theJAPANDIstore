using Dapper;
using Ecom.Models;
using Ecom.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Ecom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config; 
        }

        [HttpPost("UserNameValidation")]
        public async Task<bool> UserNameValidation(string user)
        {
            using(var conn = new SqlConnection(_config.GetConnectionString("DefaultConnectionString")))
            {
                await conn.OpenAsync();
                int count = await conn.ExecuteScalarAsync<int>("select count(*) from Users where username = @user", new { user = user });
                return count > 0;
            }
         }

        [HttpPost("register")]
        public async Task<ActionResult<int>> Register(NewUser newUser)
        {
            using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnectionString")))
            {
                await conn.OpenAsync();
                bool usernameExists = await UserNameValidation(newUser.Username);
                if(usernameExists)
                {
                    return 0;
                }
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
                int userId = await conn.ExecuteScalarAsync<int>(@"INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName) 
                    VALUES (@Username, @Email, @PasswordHash, @FirstName, @LastName); 
                    SELECT SCOPE_IDENTITY()",
                    new { Username = newUser.Username, Email = newUser.Email, PasswordHash = passwordHash, FirstName = newUser.FirstName, LastName = newUser.LastName });
                
                return Ok(userId);
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(LoginUser User)
        {
            using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnectionString")))
            {
                bool userExists = await UserNameValidation(User.UserName);
                if (!userExists)
                {
                    return "User doesn't exist";
                }
                Users CurrentUser = await conn.QueryFirstOrDefaultAsync<Users>("select * from Users where username = @user", new { user = User.UserName });
                bool passwordCheck = BCrypt.Net.BCrypt.Verify(User.Password, CurrentUser.PasswordHash);
                if(!passwordCheck)
                {
                    return "Wrong Password";
                }
                var jwtToken = CreateToken(CurrentUser);

                return jwtToken;
            }
        }

        private string CreateToken(Users user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    _config.GetSection("AppSettings:jwtTokenKey").Value!
                ));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var Token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1000),
                    signingCredentials: credentials
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(Token);

            return jwt;
        }
    }
}

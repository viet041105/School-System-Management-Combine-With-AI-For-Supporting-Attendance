using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using Backendd.DTOs;
using Backendd.ModelFromDB;

namespace ForTest_Backend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly DBCnhom1 _context;
        private readonly IConfiguration _configuration;

        public AuthenticationController(DBCnhom1 context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] DTO_UserLogin loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _context.TblUsers
                    .FirstOrDefaultAsync(u => u.UsUsername == loginModel.UsernameOrEmail ||
                                              u.UsEmail == loginModel.UsernameOrEmail);

                if (user == null || !VerifyPassword(loginModel.Password, user.UsPassword))
                {
                    return Unauthorized(new { message = "Tên người dùng, Email hoặc mật khẩu không chính xác!" });
                }
                            
                string token = GenerateJwtToken(user);

                return Ok(new { message = "Đăng nhập thành công!", token, userId = user.UsId, username = user.UsUsername , userRole = user.UsRole });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] DTO_UserRegistration userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (await _context.TblUsers.AnyAsync(u => u.UsEmail == userModel.Email))
                    return Conflict(new { message = "Email đã được sử dụng!" });
                if (await _context.TblUsers.AnyAsync(u => u.UsUsername == userModel.Username))
                    return Conflict(new { message = "Tên người dùng đã tồn tại!" });

                TblUser newUser = new TblUser
                {
                    UsId = Guid.NewGuid(),
                    UsUsername = userModel.Username.Trim(),
                    UsEmail = userModel.Email.Trim(),
                    UsPassword = HashPassword(userModel.Password),
                    UsRole = "student"
                };

                _context.TblUsers.Add(newUser);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Đăng ký thành công!", userId = newUser.UsId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(token))
                    return BadRequest(new { message = "Token không hợp lệ!" });

                _context.TblJwtblacklists.Add(new TblJwtblacklist
                {
                    JwtId = Guid.NewGuid(),
                    JwtString = token,
                    JwtListedTime = DateTime.UtcNow,
                    JwtExpiredTime = DateTime.UtcNow.AddHours(2)
                });
                await _context.SaveChangesAsync();
                return Ok(new { message = "Đăng xuất thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }

        }

        [HttpGet("is-token-blacklisted")]
        public async Task<IActionResult> IsTokenBlacklistedEndpoint([FromQuery] string token)
        {
            bool isBlacklisted = await IsTokenBlacklisted(token);
            return Ok(new { token, isBlacklisted });
        }

        private async Task<bool> IsTokenBlacklisted(string token)
        {
            return await _context.TblJwtblacklists.AnyAsync(t => t.JwtString == token);
        }

        private string GenerateJwtToken(TblUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UsUsername),
                new Claim(JwtRegisteredClaimNames.Email, user.UsEmail),
                new Claim("UserId", user.UsId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (!string.IsNullOrWhiteSpace(user.UsRole))
            {
                claims.Add(new Claim(ClaimTypes.Role, user.UsRole));
            }

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
        private bool VerifyPassword(string password, string hashedPassword) => BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}

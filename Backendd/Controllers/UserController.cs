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
using BCrypt.Net;

namespace Backendd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly DBCnhom1 _context;

        public UserController(DBCnhom1 context)
        {
            _context = context;
        }

        // GET api/users/students
        [HttpGet("students")]
        public async Task<IActionResult> GetAllStudents()
        {
            try
            {
                var students = await _context.DTO_StudentsLists
                    .FromSqlRaw("EXEC GetAllStudents")
                    .ToListAsync();

                return Ok(students);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET api/users/teachers
        [HttpGet("teachers")]
        public async Task<IActionResult> GetAllTeachers()
        {
            try
            {
                var teachers = await _context.DTO_TeachersLists
                    .FromSqlRaw("EXEC GetAllTeachers")
                    .ToListAsync();

                return Ok(teachers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        // Thêm người dùng mới
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] DTO_CreateUser newUser)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                .Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { success = false, errors });
            }

            // Tạo đối tượng người dùng mới
            var user = new TblUser
            {
                UsId = Guid.NewGuid(),
                UsEmail = newUser.UsEmail,
                UsUsername = newUser.UsUsername,
                // Lưu ý: Trong thực tế, bạn cần băm (hash) mật khẩu trước khi lưu vào cơ sở dữ liệu
                UsPassword = HashPassword(newUser.UsPassword),
                UsRole = newUser.UsRole
            };

            _context.TblUsers.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Thêm người dùng thành công!" });
        }


        // Cập nhật thông tin người dùng
        [HttpPut("Edit/{userId}")]
        public async Task<IActionResult> Edit(Guid userId, [FromBody] DTO_UpdatedUser updatedUser)
        {
            var user = await _context.TblUsers.FindAsync(userId);
            if (user == null) return NotFound(new { success = false, message = "Người dùng không tồn tại." });

            user.UsUsername = updatedUser.UsUsername;
            user.UsEmail = updatedUser.UsEmail;
            user.UsRole = updatedUser.UsRole;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Cập nhật thành công" });
        }

        // Xóa người dùng
        [HttpDelete("Delete/{userId}")]
        public async Task<IActionResult> Delete(Guid userId)
        {
            var user = await _context.TblUsers.FirstOrDefaultAsync(c => c.UsId == userId);
            if (user == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy người dùng!" });
            }

            _context.TblUsers.Remove(user);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Xóa người dùng thành công!" });
        }

        private string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    }
}

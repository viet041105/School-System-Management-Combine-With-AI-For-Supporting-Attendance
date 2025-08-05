using Backendd.ModelFromDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Backendd.DTOs;

namespace Backendd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : Controller
    {
        private readonly DBCnhom1 _context;

        public ClassController(DBCnhom1 context)
        {
            _context = context;
        }

        // GET: api/Class/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var classes = await _context.TblClasses
                .OrderByDescending(x => x.ClName)
                .ToListAsync();

            return Ok(classes);
        }

        // POST: api/Class/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] DTO_Class newClass)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                .Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { success = false, errors });
            }

            // Tạo đối tượng lớp học mới
            var cls = new TblClass
            {
                ClId = Guid.NewGuid(),
                ClName = newClass.ClName,
                ClLocation = newClass.ClLocation,
                // Đảm bảo các danh sách không null
                TblCameras = new System.Collections.Generic.List<TblCamera>(),
                CuUsers = new System.Collections.Generic.List<TblUser>(),
                CsSubjects = new System.Collections.Generic.List<TblSubject>()
            };

            _context.TblClasses.Add(cls);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Thêm lớp thành công!" });
        }

        // PUT: api/Class/Edit/{class_id}
        [HttpPut("Edit/{class_id}")]
        public async Task<IActionResult> Edit(Guid class_id, [FromBody] DTO_Class updatedClass)
        {
            var cls = await _context.TblClasses.FindAsync(class_id);
            if (cls == null)
            {
                return NotFound(new { success = false, message = "Lớp học không tồn tại." });
            }

            cls.ClName = updatedClass.ClName;
            cls.ClLocation = updatedClass.ClLocation;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Cập nhật thành công" });
        }

        // DELETE: api/Class/Delete/{class_id}
        [HttpDelete("Delete/{class_id}")]
        public async Task<IActionResult> Delete(Guid class_id)
        {
            try
            {
                // Tìm lớp học theo ID
                var cls = await _context.TblClasses
                    .Include(c => c.TblSchedules) // Bao gồm các bản ghi liên quan trong TblSchedule
                    .FirstOrDefaultAsync(c => c.ClId == class_id);

                if (cls == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy lớp học!" });
                }

                // Kiểm tra xem lớp học có lịch trình liên quan không
                if (cls.TblSchedules != null && cls.TblSchedules.Any())
                {
                    // Xóa các bản ghi trong TblSchedule
                    _context.TblSchedules.RemoveRange(cls.TblSchedules);
                }

                // Xóa lớp học
                _context.TblClasses.Remove(cls);

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Xóa lớp học thành công!" });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


    }
}

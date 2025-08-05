using Backendd.ModelFromDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Backendd.DTOs;

namespace Backendd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : Controller
    {
        private readonly DBCnhom1 _context;

        public SubjectController(DBCnhom1 context)
        {
            _context = context;
        }

        // GET: api/Subject/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var subjects = await _context.TblSubjects
                .OrderByDescending(x => x.SbName)
                .ToListAsync();

            return Ok(subjects);
        }

        // POST: api/Subject/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] DTO_Subject newSubject)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                .Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { success = false, errors });
            }

            // Tạo đối tượng môn học mới
            var subject = new TblSubject
            {
                SbId = Guid.NewGuid(),
                SbName = newSubject.SbName,
                // Đảm bảo các danh sách không null
                CsClasses = new List<TblClass>(),
                TblSchedules = new List<TblSchedule>()
            };

            _context.TblSubjects.Add(subject);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Thêm môn học thành công!" });
        }

        // PUT: api/Subject/Edit/{subject_id}
        [HttpPut("Edit/{subject_id}")]
        public async Task<IActionResult> Edit(Guid subject_id, [FromBody] DTO_Subject updatedSubject)
        {
            var subject = await _context.TblSubjects.FindAsync(subject_id);
            if (subject == null)
            {
                return NotFound(new { success = false, message = "Môn học không tồn tại." });
            }

            subject.SbName = updatedSubject.SbName;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Cập nhật môn học thành công!" });
        }

        // DELETE: api/Subject/Delete/{subject_id}
        [HttpDelete("Delete/{subject_id}")]
        public async Task<IActionResult> Delete(Guid subject_id)
        {
            try
            {
                // Tìm môn học theo ID
                var subject = await _context.TblSubjects
                    .Include(s => s.TblSchedules) // Bao gồm các bản ghi liên quan trong TblSchedule
                    .FirstOrDefaultAsync(s => s.SbId == subject_id);

                if (subject == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy môn học!" });
                }

                // Kiểm tra xem môn học có lịch trình liên quan không
                if (subject.TblSchedules != null && subject.TblSchedules.Any())
                {
                    // Xóa các bản ghi trong TblSchedule
                    _context.TblSchedules.RemoveRange(subject.TblSchedules);
                }

                // Xóa môn học
                _context.TblSubjects.Remove(subject);

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Xóa môn học thành công!" });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

}

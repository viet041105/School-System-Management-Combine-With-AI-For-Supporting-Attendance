using Microsoft.AspNetCore.Mvc;
using Backendd.ModelFromDB;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Backendd.DTOs;

namespace Backendd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly DBCnhom1 _context;

        public ScheduleController(DBCnhom1 context)
        {
            _context = context;
        }

        // GET: api/Schedule/GetAll
        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<DTO_Schedule>>> GetSchedules()
        {
            var schedules = await _context.TblSchedules
                                          .Include(s => s.ScSubject)
                                          .Include(s => s.Class)
                                          .ToListAsync();

            var scheduleDTOs = schedules.Select(s => new DTO_Schedule
            {
                ScId = s.ScId,
                SubjectId = s.ScSubjectid,
                SubjectName = s.ScSubject?.SbName, // Lấy tên môn học từ TblSubject
                ScStarttime = s.ScStarttime,
                ScEndtime = s.ScEndtime,
                ScNumstudent = s.ScNumstudent,
                ClassName = s.Class?.ClName, // Lấy tên lớp học từ TblClass
                ClassId = s.ScClassId,
            }).ToList();

            return Ok(scheduleDTOs);
        }

        // GET: api/Schedule/GetById/{id}
        [HttpGet("GetSchedulesByUser/{user_id}")]
        public async Task<ActionResult<IEnumerable<DTO_Schedule>>> GetSchedulesByUser(Guid user_id)
        {
            if (user_id == Guid.Empty)
            {
                return BadRequest("Invalid user ID.");
            }

            var schedules = await _context.TblSchedules
                .Include(s => s.ScSubject)
                .Include(s => s.Class)
                .Where(s => _context.TblScheduleUsers
                                    .Any(su => su.SuScheduleid == s.ScId && su.SuUserid == user_id))
                .ToListAsync();

            var scheduleDTOs = schedules.Select(s => new DTO_Schedule
            {
                ScId = s.ScId,
                SubjectId = s.ScSubjectid,
                SubjectName = s.ScSubject?.SbName,
                ScStarttime = s.ScStarttime,
                ScEndtime = s.ScEndtime,
                ScNumstudent = s.ScNumstudent,
                ClassName = s.Class?.ClName,
                ClassId = s.ScClassId,
            }).ToList();

            return Ok(scheduleDTOs);
        }

        // Lấy trạng thái của schedule và user
        [HttpGet("GetIsArrive")]
        public async Task<IActionResult> GetIsArrive(Guid user_id, Guid schedule_id)
        {
            var scheduleUser = await _context.TblScheduleUsers
                .FirstOrDefaultAsync(su => su.SuUserid == user_id && su.SuScheduleid == schedule_id);

            if (scheduleUser == null)
                return NotFound(new { message = "Không tìm thấy thông tin điểm danh." });

            return Ok(new { isArrive = scheduleUser.SuIsArrive });
        }

        // Cap nhat trang thai user
        [HttpPost("UpdateStatusArrive")]
        public async Task<IActionResult> UpdateStatusArrive([FromBody] DTO_UpdateStatusArrive dto)
        {
            var scheduleUser = await _context.TblScheduleUsers
                .FirstOrDefaultAsync(su => su.SuUserid == dto.user_id && su.SuScheduleid == dto.schedule_id);

            if (scheduleUser == null)
            {
                return NotFound(new { success = false, message = "ScheduleUser không tồn tại." });
            }

            // Cập nhật trạng thái
            scheduleUser.SuIsArrive = dto.isArrive;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Cập nhật thành công." });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi cập nhật dữ liệu.", error = ex.Message });
            }
        }

        // Them user vao schedule da co
        [HttpPost("AddUserToSchedule")]
        public async Task<IActionResult> AddUserToSchedule([FromBody] DTO_UpdateStatusArrive dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dữ liệu không hợp lệ.");

            // Kiểm tra đã tồn tại user trong lịch học chưa
            var existing = await _context.TblScheduleUsers
                .FirstOrDefaultAsync(su =>
                    su.SuScheduleid == dto.schedule_id &&
                    su.SuUserid == dto.user_id);

            if (existing != null)
            {
                // Nếu đã có, cập nhật trạng thái arrive
                existing.SuIsArrive = dto.isArrive;
            }
            else
            {
                // Nếu chưa có, thêm mới
                var newEntry = new TblScheduleUser
                {
                    SuScheduleid = dto.schedule_id,
                    SuUserid = dto.user_id,
                    SuIsArrive = false
                };
                _context.TblScheduleUsers.Add(newEntry);
            }

            await _context.SaveChangesAsync();
            return Ok("✅ Cập nhật thành công.");
        }

        [HttpDelete("RemoveStudentFromSchedule")]
        public async Task<IActionResult> RemoveStudentFromSchedule([FromQuery] Guid userId, [FromQuery] Guid scheduleId)
        {
            if (userId == Guid.Empty || scheduleId == Guid.Empty)
                return BadRequest("Thiếu userId hoặc scheduleId hợp lệ.");

            var entry = await _context.TblScheduleUsers
                .FirstOrDefaultAsync(su => su.SuUserid == userId && su.SuScheduleid == scheduleId);
            
            if (entry == null)
                return NotFound("❌ Không tìm thấy sinh viên trong lịch học.");

            _context.TblScheduleUsers.Remove(entry);
            await _context.SaveChangesAsync();

            return Ok("🗑️ Xoá sinh viên khỏi lịch học thành công.");
        }

        [HttpDelete("RemoveTeacherFromSchedule")]
        public async Task<IActionResult> RemoveTeacherFromSchedule([FromQuery] Guid userId, [FromQuery] Guid scheduleId)
        {
            if (userId == Guid.Empty || scheduleId == Guid.Empty)
                return BadRequest("Thiếu userId hoặc scheduleId hợp lệ.");

            var entry = await _context.TblScheduleUsers
                .FirstOrDefaultAsync(su => su.SuUserid == userId && su.SuScheduleid == scheduleId);

            if (entry == null)
                return NotFound("❌ Không tìm thấy giáo viên trong lịch học.");

            _context.TblScheduleUsers.Remove(entry);
            await _context.SaveChangesAsync();

            return Ok("🗑️ Xoá giáo viên khỏi lịch học thành công.");
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateSchedule([FromBody] DTO_Schedule dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid schedule data.");
            }

            var newSchedule = new TblSchedule
            {
                ScId = Guid.NewGuid(),
                ScSubjectid = dto.SubjectId,
                ScStarttime = dto.ScStarttime,
                ScEndtime = dto.ScEndtime,
                ScNumstudent = dto.ScNumstudent,
                ScClassId = dto.ClassId
            };

            _context.TblSchedules.Add(newSchedule);

            // Nếu muốn thêm tên lớp và tên môn (hiển thị):
            var subject = await _context.TblSubjects.FindAsync(dto.SubjectId);
            var classEntity = await _context.TblClasses.FindAsync(dto.ClassId);
            var subjectName = subject?.SbName ?? "";
            var className = classEntity?.ClName ?? "";

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Tạo lịch học thành công.",
                createdId = newSchedule.ScId,
                subjectName,
                className
            });
        }


        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> EditSchedule(Guid id, DTO_Schedule dto)
        {
            var existingSchedule = await _context.TblSchedules.FindAsync(id);
            if (existingSchedule == null)
            {
                return NotFound();
            }

            // Cập nhật từ DTO
            existingSchedule.ScSubjectid = dto.SubjectId;
            existingSchedule.ScStarttime = dto.ScStarttime;
            existingSchedule.ScEndtime = dto.ScEndtime;
            existingSchedule.ScNumstudent = dto.ScNumstudent;
            existingSchedule.ScClassId = dto.ClassId;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Cập nhật thành công." });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScheduleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }



        // DELETE: api/Schedule/Delete/{id}
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteSchedule(Guid id)
        {
            var schedule = await _context.TblSchedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }

            _context.TblSchedules.Remove(schedule);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Xóa thành công." });
        }

        private bool ScheduleExists(Guid id)
        {
            return _context.TblSchedules.Any(e => e.ScId == id);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Backendd.DTOs;
using Backendd.ModelFromDB;
using System.Data;

namespace TimeTable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeTableForTeacherController : ControllerBase
    {
        private readonly DBCnhom1 _context;

        public TimeTableForTeacherController(DBCnhom1 context)
        {
            _context = context;
        }

        // Lấy thời khóa biểu theo UserId và khoảng thời gian
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetTimeTableByUserId(Guid userId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var paramUserId = new SqlParameter("@UserId", userId);
                var paramStartDate = new SqlParameter("@StartDate", startDate);
                var paramEndDate = new SqlParameter("@EndDate", endDate);

                var result = await _context.TimeTableDtos
                    .FromSqlRaw("EXEC GetClassScheduleInfoByUserIdAndDateRange @UserId, @StartDate, @EndDate",
                        paramUserId, paramStartDate, paramEndDate)
                    .ToListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpGet("schedule/{scheduleId}/teachers")]
        public async Task<IActionResult> GetTeachersBySchedule(Guid scheduleId)
        {
            if (scheduleId == Guid.Empty)
                return BadRequest("Thiếu scheduleId.");

            var teachers = await _context.TblScheduleUsers
                .Where(su => su.SuScheduleid == scheduleId)
                .Join(_context.TblUsers,
                      su => su.SuUserid,
                      u => u.UsId,
                      (su, u) => new { su, u })
                .Where(joined => joined.u.UsRole == "Teacher") // 📌 Chỉnh theo cột phân biệt role
                .Select(joined => new
                {
                    us_id = joined.u.UsId,
                    fullName = joined.u.UsUsername,
                    email = joined.u.UsEmail
                })
                .ToListAsync();

            return Ok(teachers);
        }
        [HttpGet("schedule/{scheduleId}/students")]
        public async Task<IActionResult> GetStudentsBySchedule(Guid scheduleId)
        {
            var students = await _context.TblScheduleUsers
                .Where(su => su.SuScheduleid == scheduleId)
                .Join(_context.TblUsers,
                    su => su.SuUserid,
                    u => u.UsId,
                    (su, u) => new { su, u })
                .Where(x => x.u.UsRole == "student")
                .Select(x => new
                {
                    us_id = x.su.SuUserid,
                    fullName = x.u.UsUsername,
                    isArrive = x.su.SuIsArrive
                })
                .ToListAsync();

            return Ok(students);
        }
        [HttpPost("schedule/update-attendance")]
        public async Task<IActionResult> UpdateAttendanceForAllStudents([FromBody] UpdateAttendanceDto dto)
        {
            try
            {
                if (dto == null || dto.ScheduleId == Guid.Empty || dto.AttendanceRecords == null || !dto.AttendanceRecords.Any())
                {
                    return BadRequest(new { message = "Dữ liệu đầu vào không hợp lệ." });
                }

                int totalRowsAffected = 0;

                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                foreach (var record in dto.AttendanceRecords)
                {
                    using var command = new SqlCommand("UpdateSingleStudentAttendance", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@ScheduleId", SqlDbType.UniqueIdentifier) { Value = dto.ScheduleId });
                    command.Parameters.Add(new SqlParameter("@StudentId", SqlDbType.UniqueIdentifier) { Value = record.UsId });
                    command.Parameters.Add(new SqlParameter("@IsArrive", SqlDbType.Bit) { Value = record.IsArrive });

                    var rowsAffected = (int)(await command.ExecuteScalarAsync() ?? 0);
                    totalRowsAffected += rowsAffected;
                    Console.WriteLine($"Updated {record.UsId}: Rows Affected = {rowsAffected}");
                }

                if (totalRowsAffected == 0)
                {
                    Console.WriteLine("Warning: No records were updated. Check ScheduleId and StudentIds.");
                }

                return Ok(new { message = "Cập nhật điểm danh thành công.", rowsAffected = totalRowsAffected });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi khi cập nhật điểm danh: {ex.Message}" });
            }
        }
    }
}
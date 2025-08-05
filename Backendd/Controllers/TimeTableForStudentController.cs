using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Backendd.DTOs;
using Backendd.ModelFromDB;


namespace TimeTable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeTableForStudentController : ControllerBase
    {
        private readonly DBCnhom1 _context;

        public TimeTableForStudentController(DBCnhom1 context)
        {
            _context = context;
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetTimeTableByStudentId(Guid studentId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var paramStudentId = new SqlParameter("@StudentId", studentId);
                var paramStartDate = new SqlParameter("@StartDate", startDate);
                var paramEndDate = new SqlParameter("@EndDate", endDate);

                var result = await _context.TimeTableForStudentDtos
                    .FromSqlRaw("EXEC GetClassScheduleWithAttendanceForStudent @StudentId, @StartDate, @EndDate",
                        paramStudentId, paramStartDate, paramEndDate)
                    .ToListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("schedule/{scheduleId}/students")]
        public async Task<IActionResult> GetStudentsForSchedule(Guid scheduleId)
        {
            try
            {
                var paramScheduleId = new SqlParameter("@ScheduleId", scheduleId);
                var result = await _context.StudentForModalDtos
                    .FromSqlRaw("EXEC GetStudentsForModal @ScheduleId", paramScheduleId)
                    .ToListAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
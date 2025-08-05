using Backendd.ModelFromDB;
using Backendd.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Backendd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly DBCnhom1 _context; // Thay YourDbContext bằng DbContext của bạn

        public LogController(DBCnhom1 context)
        {
            _context = context;
        }

        [HttpPost("save-log")]
        public async Task<IActionResult> SaveLog([FromBody] UnknownLogDto logDto)
        {
            if (logDto == null || string.IsNullOrEmpty(logDto.LogDetails))
            {
                return BadRequest("Thông tin log không hợp lệ.");
            }

            try
            {
                var logEntry = new TblLog
                {
                    LogCommiterId = logDto.LogCommiterId,
                    LogPhone = logDto.LogPhone ?? "N/A",
                    LogDetails = logDto.LogDetails
                };

                _context.TblLogs.Add(logEntry);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Lưu log thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lưu log: {ex.Message}");
            }
        }
    }
}
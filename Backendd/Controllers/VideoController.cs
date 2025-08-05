using Hangfire;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using Backendd.Service;
using Backendd.DTOs;
using Microsoft.AspNetCore.Http;

namespace Video.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoUploadController : ControllerBase
    {
        private readonly VideoProcessingService _videoProcessingService;

        public VideoUploadController(VideoProcessingService videoProcessingService)
        {
            _videoProcessingService = videoProcessingService;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(10L * 1024L * 1024L * 1024L)]
        public async Task<IActionResult> UploadVideo([FromForm] FileUploadDto fileUpload)
        {
            if (fileUpload == null || fileUpload.Video == null || fileUpload.Video.Length == 0)
                return BadRequest(new { message = "No file uploaded." });

            try
            {
                // Tạo thư mục tạm nếu chưa tồn tại
                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "TempVideos");
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                // Tạo tên file duy nhất để tránh xung đột
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileUpload.Video.FileName)}";
                var savePath = Path.Combine(uploadFolder, fileName);

                // Lưu file tạm
                using (var stream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await fileUpload.Video.CopyToAsync(stream);
                }

                // Sinh videoId để theo dõi phiên xử lý video
                var videoId = Guid.NewGuid().ToString();

                // Đẩy job xử lý video vào Hangfire
                BackgroundJob.Enqueue(() => _videoProcessingService.ProcessVideoAsync(savePath, videoId));

                return Ok(new { message = "Video is being processed.", videoId = videoId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}

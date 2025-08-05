using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Backendd.DTOs;

namespace Stream.Controllers

{

    [ApiController]
    [Route("[controller]")]
    public class StreamController : ControllerBase
    {
        private static Dictionary<string, Process> processes = new();

        [HttpPost("start-webcam")]
        public IActionResult StartWebcam([FromBody] StartWebcamRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.CamHttpUrl) || string.IsNullOrWhiteSpace(req.CamStreamKey))
                return BadRequest("Missing cameraUrl or streamKey");

            if (!req.CamHttpUrl.StartsWith("dshow://video="))
                return BadRequest("CameraUrl must start with dshow://video=");


            if (processes.ContainsKey(req.CamStreamKey))
                return BadRequest("A stream with this key is already running.");

            var deviceName = req.CamHttpUrl.Replace("dshow://video=", "");

            var ffmpegArgs = $"-f dshow -i video=\"{deviceName}\" " +
                             "-vcodec libx264 -preset ultrafast -tune zerolatency " +
                             "-f flv rtmp://localhost:1935/hls/" + req.CamStreamKey;

            var startInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = ffmpegArgs,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            var ffmpegProc = new Process { StartInfo = startInfo };

            ffmpegProc.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    Console.WriteLine($"[FFmpeg] {e.Data}");
            };

            ffmpegProc.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    Console.WriteLine($"[FFmpeg Error] {e.Data}");
            };

            ffmpegProc.Exited += (s, e) =>
            {
                processes.Remove(req.CamStreamKey);
                Console.WriteLine($"[FFmpeg] streamKey={req.CamStreamKey} exited.");
            };

            try
            {
                ffmpegProc.Start();
                ffmpegProc.BeginOutputReadLine();
                ffmpegProc.BeginErrorReadLine();
                processes[req.CamStreamKey] = ffmpegProc;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to start streaming process: {ex.Message}");
            }

            return Ok(new { message = "Webcam stream started", streamKey = req.CamStreamKey });
        }

        [HttpPost("stop-stream")]
        public IActionResult StopStream([FromBody] StopStreamRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.CamStreamKey))
                return BadRequest("Missing stream key.");

            if (processes.TryGetValue(req.CamStreamKey, out var proc))
            {
                try
                {
                    if (!proc.HasExited)
                        proc.Kill();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error killing process: {ex.Message}");
                }
                processes.Remove(req.CamStreamKey);
                return Ok("Stream stopped");
            }
            return NotFound("Stream not found");
        }
    }
}
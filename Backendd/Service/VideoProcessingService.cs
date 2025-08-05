using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Backendd.Service
{
    public class VideoProcessingService
    {
        private readonly ILogger<VideoProcessingService> _logger;
        private readonly IHubContext<VideoProcessingHub> _hubContext;
        private readonly IHttpClientFactory _httpClientFactory;

        public VideoProcessingService(ILogger<VideoProcessingService> logger,
                                      IHubContext<VideoProcessingHub> hubContext,
                                      IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _hubContext = hubContext;
            _httpClientFactory = httpClientFactory;
        }

        public async Task ProcessVideoAsync(string videoPath, string videoId)
        {
            try
            {
                _logger.LogInformation($"[Job {videoId}] Bắt đầu xử lý video: {videoPath}");

                var httpClient = _httpClientFactory.CreateClient();
                httpClient.Timeout = TimeSpan.FromMinutes(20);

                using var formContent = new MultipartFormDataContent();
                using var fileStream = new FileStream(videoPath, FileMode.Open, FileAccess.Read);
                var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("video/mp4");
                formContent.Add(streamContent, "video", Path.GetFileName(videoPath));

                var fastApiUrl = "http://localhost:8000/uploadvideo";
                var fastApiResponse = await httpClient.PostAsync(fastApiUrl, formContent);

                var jsonResponse = await fastApiResponse.Content.ReadAsStringAsync();

                if (!fastApiResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"[Job {videoId}] Lỗi khi gọi FastAPI. Status: {fastApiResponse.StatusCode}");
                    await _hubContext.Clients.Group(videoId).SendAsync("ReceiveProcessingResult", jsonResponse);
                    return;
                }

                _logger.LogInformation($"[Job {videoId}] Xử lý video hoàn thành.");
                // Gửi JSON response đến nhóm videoId
                await _hubContext.Clients.Group(videoId).SendAsync("ReceiveProcessingResult", jsonResponse);

                await Task.Delay(2000); // Chờ để đảm bảo file được giải phóng

                int retryCount = 3;
                for (int i = 0; i < retryCount; i++)
                {
                    try
                    {
                        if (File.Exists(videoPath))
                        {
                            File.Delete(videoPath);
                            _logger.LogInformation($"[Job {videoId}] Đã xóa file tạm: {videoPath}");
                            break;
                        }
                    }
                    catch (IOException ex) when (i < retryCount - 1)
                    {
                        _logger.LogWarning($"[Job {videoId}] Lỗi khi xóa file (lần {i + 1}): {ex.Message}. Thử lại...");
                        await Task.Delay(1000);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"[Job {videoId}] Lỗi khi xóa file sau {retryCount} lần thử: {ex.Message}");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Job {videoId}] Exception: {ex.Message}");
                var errorResponse = $"{{\"error\": \"{ex.Message}\"}}";
                await _hubContext.Clients.Group(videoId).SendAsync("ReceiveProcessingResult", errorResponse);
            }
        }
    }
}
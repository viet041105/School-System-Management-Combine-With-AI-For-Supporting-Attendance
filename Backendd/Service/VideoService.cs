//using System;
//using System.IO;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.StaticFiles;

//namespace Video.Services
//{
//    public class VideoService
//    {
//        private readonly HttpClient _httpClient;

//        public VideoService()
//        {
//            _httpClient = new HttpClient();
//        }

//        public async Task<string> UploadVideoAsync(string filePath)
//        {
//            // Đường dẫn tới Flask API upload
//            var requestUrl = "http://127.0.0.1:3000/upload";

//            // Tạo nội dung multipart cho file upload
//            using (var multipartContent = new MultipartFormDataContent())
//            {
//                // Sử dụng FileStream để mở file dưới dạng stream (tránh đọc toàn bộ file vào bộ nhớ)
//                using (var fileStream = File.OpenRead(filePath))
//                {
//                    var fileContent = new StreamContent(fileStream);

//                    // Xác định Content-Type dựa trên phần mở rộng của file
//                    var provider = new FileExtensionContentTypeProvider();
//                    if (!provider.TryGetContentType(filePath, out string contentType))
//                    {
//                        contentType = "application/octet-stream"; // fallback nếu không xác định được
//                    }
//                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);

//                    // Thêm file vào multipart content với key "video" (phải trùng với Flask API)
//                    multipartContent.Add(fileContent, "video", Path.GetFileName(filePath));

//                    // Gửi request POST đến Flask API
//                    var response = await _httpClient.PostAsync(requestUrl, multipartContent);

//                    if (response.IsSuccessStatusCode)
//                    {
//                        // Đọc kết quả trả về từ API (thường là metadata hoặc thông tin video)
//                        var responseContent = await response.Content.ReadAsStringAsync();
//                        return responseContent;
//                    }
//                    else
//                    {
//                        throw new Exception("Error uploading video: " + response.StatusCode);
//                    }
//                }
//            }
//        }
//    }
//}

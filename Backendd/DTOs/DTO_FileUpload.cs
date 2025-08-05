    using Microsoft.AspNetCore.Http;

    namespace Backendd.DTOs
    {
        public class FileUploadDto
        {
            public IFormFile Video { get; set; }
        }
    }

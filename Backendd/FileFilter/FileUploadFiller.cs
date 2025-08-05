using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Backendd.Filter
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Kiểm tra xem action có parameter kiểu IFormFile hay không
            var fileParams = context.ApiDescription.ParameterDescriptions
                .Where(p => p.ModelMetadata != null && p.ModelMetadata.ModelType == typeof(Microsoft.AspNetCore.Http.IFormFile))
                .ToList();

            if (!fileParams.Any())
            {
                return;
            }

            // Xóa tất cả các parameter file đã có (nếu có) từ operation.Parameters
            foreach (var param in fileParams)
            {
                var existingParam = operation.Parameters.FirstOrDefault(p => p.Name == param.Name);
                if (existingParam != null)
                {
                    operation.Parameters.Remove(existingParam);
                }
            }

            // Thêm RequestBody nếu chưa có
            if (operation.RequestBody == null)
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content =
                    {
                        ["multipart/form-data"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties =
                                {
                                    ["video"] = new OpenApiSchema
                                    {
                                        Description = "File upload",
                                        Type = "string",
                                        Format = "binary"
                                    }
                                },
                                Required = new System.Collections.Generic.HashSet<string> { "video" }
                            }
                        }
                    }
                };
            }
        }
    }
}

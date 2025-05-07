using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasFileUpload = context.MethodInfo.GetParameters()
            .Any(p => p.ParameterType == typeof(IFormFile));

        if (!hasFileUpload)
            return;

        operation.RequestBody = new OpenApiRequestBody
        {
            Content = {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = {
                            ["image"] = new OpenApiSchema
                            {
                                Type = "string",
                                Format = "binary"
                            },
                            ["note"] = new OpenApiSchema
                            {
                                Type = "string"
                            }
                        },
                        Required = new HashSet<string> { "image" }
                    }
                }
            }
        };
    }
}

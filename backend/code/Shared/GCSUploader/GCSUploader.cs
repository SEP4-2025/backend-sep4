using Microsoft.AspNetCore.Http;
using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;

namespace GCSUploader;

public class GCSUploader
{
    private readonly StorageClient _storageClient;
    private readonly string _bucketName;

    public GCSUploader()
    {
        // Get credentials from environment variable
        string jsonCredentials = Environment.GetEnvironmentVariable("GCS_CREDENTIALS");
        var googleCredential = GoogleCredential.FromJson(jsonCredentials);
        _storageClient = StorageClient.Create(googleCredential);

        // Get bucket name from environment variable
        _bucketName = Environment.GetEnvironmentVariable("GCS_BUCKET_NAME") ?? "growmate-plant-pictures";
    }

    public async Task<string> UploadImageAsync(IFormFile file, string fileName)
    {
        using var stream = file.OpenReadStream();

        var objectName = $"pictures/{fileName}";
        await _storageClient.UploadObjectAsync(_bucketName, objectName, file.ContentType, stream);

        return $"https://storage.googleapis.com/{_bucketName}/{objectName}";
    }
}
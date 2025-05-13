using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Http;

namespace GoogleCloud;

using Google.Cloud.Storage.V1;

public class GcsUploader
{
    private readonly StorageClient _storageClient;
    private readonly string _bucketName = "growmate-plant-pictures";

    public GcsUploader()
    {
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "gcs-key.json");
        var gcsCredentialJson = Environment.GetEnvironmentVariable("GCS_BACKEND_KEY");
        GoogleCredential credential = GoogleCredential.FromJson(gcsCredentialJson);
        _storageClient = StorageClient.Create();
    }

    public async Task<string> UploadImageAsync(IFormFile file, string fileName)
    {
        using var stream = file.OpenReadStream();

        var objectName = $"pictures/{fileName}";
        await _storageClient.UploadObjectAsync(_bucketName, objectName, file.ContentType, stream);

        return $"https://storage.googleapis.com/{_bucketName}/{objectName}";
    }
}
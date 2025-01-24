using System.Text;
using Application.Interfaces.Services;
using Application.Utilis;
using Infrastructure.Services.Options;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace Application.Services;

public class MinioService : IMinioService
{
    private readonly IMinioClient _minioClient;
    private readonly MinioOptions _minioConfig;

    public MinioService(IOptions<MinioOptions> minioOptions)
    {
        _minioConfig = minioOptions.Value;
        _minioClient = new MinioClient()
            .WithEndpoint(_minioConfig.Endpoint)
            .WithCredentials(_minioConfig.AccessKey, _minioConfig.SecretKey)
            .Build();
    }

    public async Task<Result> CreateDocument(Guid documentId)
    {
        try
        {
            var bucketExists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_minioConfig.BucketName));
            if (!bucketExists)
            {
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_minioConfig.BucketName));
            }

            var objectName = $"{documentId}.txt";

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(string.Empty));
            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(_minioConfig.BucketName)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType("text/plain"));

            return Result.Success();
        }
        catch (Exception exception)
        {
            return Result.Failure(exception);
        }
    }

    public async Task<Result<string>> PullDocument(Guid documentId)
    {
        try
        {
            var objectName = $"{documentId}.txt";
            var content = string.Empty;

            await _minioClient.GetObjectAsync(new GetObjectArgs()
                .WithBucket(_minioConfig.BucketName)
                .WithObject(objectName)
                .WithCallbackStream(stream =>
                {
                    using var reader = new StreamReader(stream);
                    content = reader.ReadToEnd();
                }));

            return Result<string>.Success(content);
        }
        catch (Exception exception)
        {
            return Result<string>.Failure(exception);
        }
    }

    public async Task<Result> PushDocument(Guid documentId, string content)
    {
        try
        {
            var objectName = $"{documentId}.txt";

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(_minioConfig.BucketName)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType("text/plain"));

            return Result.Success();
        }
        catch (Exception exception)
        {
            return Result.Failure(exception);
        }
    }
    
    public async Task<Result> DeleteDocument(Guid documentId)
    {
        try
        {
            var objectName = $"{documentId}.txt";

            await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(_minioConfig.BucketName)
                .WithObject(objectName));

            return Result.Success();
        }
        catch (Exception exception)
        {
            return Result.Failure(exception);
        }
    }
}
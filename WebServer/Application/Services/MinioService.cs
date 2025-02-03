using System.Text;
using Application.Interfaces.Services;
using Application.Services.Options;
using Application.Utils;
using Core.Enum;
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
            .WithSSL(false)
            .Build();
    }

    public async Task<Result> CreateDocument(Guid documentId)
    {
        try
        {
            var bucketExists = await _minioClient.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(_minioConfig.BucketName));
            if (!bucketExists)
            {
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_minioConfig.BucketName));
            }

            var objectName = $"{documentId}.txt";

            using var stream = new MemoryStream("Hello World!"u8.ToArray());
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
            return Result.Failure(new Error(ErrorType.ServerError, exception.Message));
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
            return Result<string>.Failure(new Error(ErrorType.ServerError, exception.Message));
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
            return Result.Failure(new Error(ErrorType.ServerError, exception.Message));
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
            return Result.Failure(new Error(ErrorType.ServerError, exception.Message));
        }
    }
    
    public async Task<Result<bool>> DocumentExists(Guid documentId)
    {
        try
        {
            var objectName = $"{documentId}.txt";

            await _minioClient.StatObjectAsync(new StatObjectArgs()
                .WithBucket(_minioConfig.BucketName)
                .WithObject(objectName));

            return Result<bool>.Success(true);
        }
        catch (Minio.Exceptions.ObjectNotFoundException)
        {
            return Result<bool>.Success(false);
        }
        catch (Exception exception)
        {
            return Result<bool>.Failure(new Error(ErrorType.ServerError, exception.Message));
        }
    }
}
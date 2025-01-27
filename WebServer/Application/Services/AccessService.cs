using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Utils;
using Core.Enum;
using Core.Models;

namespace Application.Services;

public class AccessService(
    IAccessRepository accessRepository, 
    IUserService usersService, 
    IDocumentService documentService) 
    : IAccessService
{
    public async Task<Result<AccessControl>> Check(Guid userId, Guid documentId)
    {
        var checkDocumentResult = await documentService.Check(documentId);
        if (!checkDocumentResult.IsSuccess)
            return Result<AccessControl>.Failure(checkDocumentResult.Error);
        
        var checkUserResult = await usersService.CheckById(Guid.Parse(userId.ToString()));
        if (!checkUserResult.IsSuccess)
            return Result<AccessControl>.Failure(checkUserResult.Error);
        
        return await accessRepository.Check(userId, documentId);
    }

    public async Task<Result> CheckMaster(Guid userId, Guid documentId)
    {
        var checkUserDocumentResult = await Check(userId, documentId);
        if (!checkUserDocumentResult.IsSuccess)
            return Result.Failure(checkUserDocumentResult.Error);

        return checkUserDocumentResult.Value!.Permission == Permissions.Master
            ? Result.Success()
            : Result.Failure(new Error(ErrorType.BadRequest, 
                "Пользователь не является создателем документа"));
    }

    public async Task<Result> Create(Guid userId, Guid documentId, Permissions permission)
    {
        var checkUserDocumentResult = await Check(userId, documentId);
        if (checkUserDocumentResult.IsSuccess)
            return Result.Failure(new Error(ErrorType.BadRequest,
                "Этому пользователю уже присвоен уровень доступа!"));
        
        return await accessRepository.Create(documentId, userId, permission);
    }

    public async Task<Result> Set(Guid userId, Guid documentId, Permissions newPermission)
    {
        var checkUserPermissionResult = await Check(userId, documentId);
        if (!checkUserPermissionResult.IsSuccess || checkUserPermissionResult.Value == null)
            return Result.Failure(new Error(ErrorType.BadRequest,
                "У этого пользователя нет доступа к документу!"));

        return await accessRepository.Set(documentId, userId, newPermission);
    }

    public async Task<Result<IEnumerable<User>>> GetUsers(Guid documentId)
    {
        var checkDocumentResult = await documentService.Check(documentId);
        if (!checkDocumentResult.IsSuccess)
            return Result<IEnumerable<User>>.Failure(checkDocumentResult.Error);
        
        return await accessRepository.Get(documentId);
    }

    public async Task<Result<IEnumerable<User>>> GetReaders(Guid documentId)
    {
        var checkDocumentResult = await documentService.Check(documentId);
        if (!checkDocumentResult.IsSuccess)
            return Result<IEnumerable<User>>.Failure(checkDocumentResult.Error);

        return await accessRepository.GetReaders(documentId);
    }

    public async Task<Result<IEnumerable<User>>> GetWriters(Guid documentId)
    {
        var checkDocumentResult = await documentService.Check(documentId);
        if (!checkDocumentResult.IsSuccess)
            return Result<IEnumerable<User>>.Failure(checkDocumentResult.Error);

        return await accessRepository.GetWriters(documentId);
    }

    public async Task<Result> Delete(Guid documentId, Guid userId)
    {
        var checkDocumentResult = await documentService.Check(documentId);
        if (!checkDocumentResult.IsSuccess)
            return Result<IEnumerable<User>>.Failure(checkDocumentResult.Error);

        return await accessRepository.Delete(documentId, userId);
    }
}
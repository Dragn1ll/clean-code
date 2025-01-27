using Application;
using Application.Interfaces.Repositories;
using Application.Utils;
using Core.Enum;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.DataAccess.Entities;

namespace Persistence.DataAccess.Repositories;

public class DocumentsRepository(WebDbContext dbContext) : IDocumentsRepository
{
    public async Task<Result<Guid>> Create(Guid userId, string title)
    {
        try
        {
            var documentEntity = new DocumentEntity
            {
                Id = Guid.NewGuid(),
                Title = title,
                AuthorId = userId
            };
            
            await dbContext.Documents.AddAsync(documentEntity);
            await dbContext.SaveChangesAsync();
        
            return Result<Guid>.Success(documentEntity.Id);
        }
        catch (Exception exception)
        {
            return Result<Guid>.Failure(new Error(ErrorType.ServerError, exception.Message));
        }
    }

    public async Task<Result> Delete(Guid documentId)
    {
        try
        {
            await dbContext.Documents
                            .Where(document => document.Id == documentId)
                            .ExecuteDeleteAsync();
            
            return Result.Success();
        }
        catch (Exception exception)
        {
            return Result.Failure(new Error(ErrorType.ServerError, exception.Message));
        }
    }

    public async Task<Result> Rename(Guid documentId, string newTitle)
    {
        try
        {
            await dbContext.Documents
                .Where(document => document.Id == documentId)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(d => d.Title, newTitle));
            
            return Result.Success();
        }
        catch (Exception exception)
        {
            return Result.Failure(new Error(ErrorType.ServerError, exception.Message));
        }
    }

    public async Task<Result<MdDocument>> GetById(Guid documentId)
    {
        try
        {
            var document = await dbContext.Documents
                .AsNoTracking()
                .FirstOrDefaultAsync(document => document.Id == documentId);
            
            return Result<MdDocument>.Success(new MdDocument(document!.Id, document.AuthorId, 
                document.Title, document.CreatedAt));
        }
        catch (Exception exception)
        {
            return Result<MdDocument>.Failure(new Error(ErrorType.ServerError, exception.Message));
        }
    }

    public async Task<Result<bool>> Check(Guid documentId)
    {
        try
        {
            var document = await dbContext.Documents
                .FirstOrDefaultAsync(document => document.Id == documentId);
            
            return Result<bool>.Success(document is not null);
        }
        catch (Exception exception)
        {
            return Result<bool>.Failure(new Error(ErrorType.ServerError, exception.Message));
        }
    }

    public async Task<Result<IEnumerable<MdDocument>>> GetByUser(Guid userId)
    {
        try
        {
            var documentsEntities = await dbContext.Documents
                .AsNoTracking()
                .Where(document => document.AuthorId == userId)
                .ToListAsync();
            
            var documents = new List<MdDocument>();
            
            foreach (var documentEntity in documentsEntities)
            {
                documents.Add(new MdDocument(documentEntity.Id, documentEntity.AuthorId, 
                    documentEntity.Title, documentEntity.CreatedAt));
            }
            
            return Result<IEnumerable<MdDocument>>.Success(documents);
        }
        catch (Exception exception)
        {
            return Result<IEnumerable<MdDocument>>.Failure(new Error(ErrorType.ServerError, exception.Message));
        }
    }

    public async Task<Result<IEnumerable<MdDocument>>> GetByUserPermission(Guid userId)
    {
        var documentsEntities = await dbContext.Documents
            .AsNoTracking()
            .Where(document => document.Accesses.FirstOrDefault(up => up.UserId == userId) != null)
            .ToListAsync();
            
        var documents = new List<MdDocument>();
            
        foreach (var documentEntity in documentsEntities)
        {
            documents.Add(new MdDocument(documentEntity.Id, documentEntity.AuthorId, 
                documentEntity.Title, documentEntity.CreatedAt));
        }
            
        return Result<IEnumerable<MdDocument>>.Success(documents);
    }
}
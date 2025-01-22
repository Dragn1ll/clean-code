using Application;
using Application.Interfaces.Repositories;
using Core.Models;
using Infrastructure.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccess.Repositories;

public class DocumentsRepository(WebDbContext dbContext) : IDocumentRepository
{
    public async Task<Result> Create(Guid userId, string title)
    {
        try
        {
            await dbContext.Documents.AddAsync(new DocumentEntity
            {
                Id = Guid.NewGuid(),
                Title = title,
                AuthorId = userId
            });
            await dbContext.SaveChangesAsync();
        
            return Result.Success();
        }
        catch (Exception exception)
        {
            return Result.Failure(exception);
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
            return Result.Failure(exception);
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
            return Result.Failure(exception);
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
            return Result<MdDocument>.Failure(exception);
        }
    }

    public async Task<Result<bool>> Check(Guid documentId)
    {
        try
        {
            var document = await dbContext.Documents
                .AsNoTracking()
                .FirstOrDefaultAsync(document => document.Id == documentId);
            
            return Result<bool>.Success(document is not null);
        }
        catch (Exception exception)
        {
            return Result<bool>.Failure(exception);
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
            return Result<IEnumerable<MdDocument>>.Failure(exception);
        }
    }

    public async Task<Result<IEnumerable<MdDocument>>> GetByUserPermission(Guid userId)
    {
        var documentsEntities = await dbContext.Documents
            .AsNoTracking()
            .Where(document => document.Users.FirstOrDefault(up => up.UserId == userId) != null)
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
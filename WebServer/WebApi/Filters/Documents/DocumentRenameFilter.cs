using System.Security.Claims;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApi.Contracts.Documents;

namespace WebApi.Filters.Documents;

public class DocumentRenameFilter(IAccessService accessService) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionArguments.TryGetValue("request", out var argument))
        {
            var request = argument as RenameDocumentRequest;
            
            var userId = Guid.Parse(context.HttpContext.User.Claims.FirstOrDefault(c => 
                c.Type == ClaimTypes.NameIdentifier)?.Value!);
            
            var checkMasterAccessResult = await accessService.CheckMaster(userId, request!.DocumentId);
            
            if (!checkMasterAccessResult.IsSuccess)
                context.Result = new BadRequestObjectResult("Недостаточно прав для переименования документа!");
        }
        
        if (context.Result == null)
            await next();
    }
}
using System.Security.Claims;
using Application.Interfaces.Services;
using Core.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApi.Contracts.Documents;

namespace WebApi.Filters.Documents;

public class DocumentGetFilter(IAccessService accessService) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionArguments.TryGetValue("documentId", out var argument))
        {
            var documentId = argument as Guid? ?? Guid.Empty;
            
            var userId = Guid.Parse(context.HttpContext.User.Claims.FirstOrDefault(c => 
                c.Type == ClaimTypes.NameIdentifier)?.Value!);
            
            var checkAccessResult = await accessService.Check(userId, documentId);
            
            if (!checkAccessResult.IsSuccess)
                context.Result = new BadRequestObjectResult(checkAccessResult.Error!.Message);
            
            if (checkAccessResult.Value == null || (int)checkAccessResult.Value!.Permission < (int)Permissions.Read)
                context.Result = new BadRequestObjectResult("Недостаточно прав для просмотра документа!");
        }
        
        if (context.Result == null)
            await next();
    }
}
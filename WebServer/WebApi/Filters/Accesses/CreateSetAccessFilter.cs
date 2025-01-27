using System.Security.Claims;
using Application.Interfaces.Services;
using Core.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApi.Contracts.Accesses;

namespace WebApi.Filters.Accesses;

public class CreateSetAccessFilter(IAccessService accessService) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionArguments.TryGetValue("request", out var argument))
        {
            var request = argument as CreateSetAccessRequest;
            
            var userId = Guid.Parse(context.HttpContext.User.Claims.FirstOrDefault(c => 
                c.Type == ClaimTypes.NameIdentifier)?.Value!);
            
            var checkAccessResult = await accessService.Check(userId, request!.DocumentId);
            
            var checkMasterResult = await accessService.CheckMaster(userId, request.DocumentId);
            if (!checkMasterResult.IsSuccess)
                context.Result = new BadRequestObjectResult(checkAccessResult.Error!.Message);
        
            if (request.PermissionId < (int)Permissions.Master)
                context.Result = new BadRequestObjectResult(
                    "Нельзя присвоить обычному пользователю такие возможности!");
        }
        
        if (context.Result == null)
            await next();
    }
}
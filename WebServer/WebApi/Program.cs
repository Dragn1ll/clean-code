using Application.Interfaces.Auth;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using Application.Services.Options;
using FluentValidation;
using Infrastructure.Auth;
using Microsoft.AspNetCore.CookiePolicy;
using Persistence.DataAccess;
using Microsoft.EntityFrameworkCore;
using Persistence.DataAccess.Repositories;
using WebApi.Contracts.Users;
using WebApi.Extensions;
using WebApi.Filters.Accesses;
using WebApi.Filters.Documents;
using WebApi.Middlewares;
using WebApi.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddEnvironmentVariables();

var services = builder.Services;

services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));
services.Configure<MinioOptions>(builder.Configuration.GetSection("Minio"));
services.AddApiAuthentication(builder.Configuration);
services.AddDbContext<WebDbContext>(options => 
{
    options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(WebDbContext)));
});

services.AddControllers();
services.AddLogging();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddScoped<IValidator<RegisterUserRequest>, RegisterValidationRules>();

services.AddScoped<IUserService, UserService>();
services.AddScoped<IDocumentService, DocumentService>();
services.AddScoped<IMinioService, MinioService>();
services.AddScoped<IMdService, MdService>();
services.AddScoped<IAccessService, AccessService>();

services.AddScoped<IUsersRepository, UsersRepository>();
services.AddScoped<IDocumentsRepository, DocumentsRepository>();
services.AddScoped<IAccessRepository, AccessesRepository>();

services.AddScoped<IJwtWorker, JwtWorker>();
services.AddScoped<IPasswordHasher, PasswordHasher>();

services.AddScoped<DocumentGetFilter>();
services.AddScoped<DocumentDeleteFilter>();
services.AddScoped<DocumentChangeFilter>();
services.AddScoped<DocumentRenameFilter>();
services.AddScoped<CreateSetAccessFilter>();
services.AddScoped<DeleteAccessFilter>();
services.AddScoped<GetAccessFilter>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    Secure = CookieSecurePolicy.Always,
    HttpOnly = HttpOnlyPolicy.Always
});

using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider;
    try
    {
        var context = service.GetRequiredService<WebDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = service.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ошибка при применении миграций.");
    }
}

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.Run();

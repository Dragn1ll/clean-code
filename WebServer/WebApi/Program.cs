using Application.Interfaces.Auth;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using Application.Services.Options;
using Infrastructure.Auth;
using Microsoft.AspNetCore.CookiePolicy;
using Persistence.DataAccess;
using Microsoft.EntityFrameworkCore;
using Persistence.DataAccess.Repositories;
using WebApi.Extensions;
using WebApi.Filters.Accesses;
using WebApi.Filters.Documents;
using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.Secrets.json", optional: true, reloadOnChange: true);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));
builder.Services.Configure<MinioOptions>(builder.Configuration.GetSection("Minio"));
builder.Services.AddApiAuthentication(builder.Configuration);
builder.Services.AddDbContext<WebDbContext>(
    options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(WebDbContext)));
    });

builder.Services.AddControllers();
builder.Services.AddLogging();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IMinioService, MinioService>();
builder.Services.AddScoped<IMdService, MdService>();
builder.Services.AddScoped<IAccessService, AccessService>();

builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IDocumentsRepository, DocumentsRepository>();
builder.Services.AddScoped<IAccessRepository, AccessesRepository>();

builder.Services.AddScoped<IJwtWorker, JwtWorker>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

builder.Services.AddScoped<DocumentGetFilter>();
builder.Services.AddScoped<DocumentDeleteFilter>();
builder.Services.AddScoped<DocumentChangeFilter>();
builder.Services.AddScoped<DocumentRenameFilter>();
builder.Services.AddScoped<CreateSetAccessFilter>();
builder.Services.AddScoped<DeleteAccessFilter>();
builder.Services.AddScoped<GetAccessFilter>();

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

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

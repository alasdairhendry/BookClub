using API.Services;
using Data;
using Domain.Interfaces;
using Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<ApiResponseFactory>();
builder.Services.AddScoped<IUserService, UserService>();

// Configure Database & Identity Services
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("AppDatabase"));
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<IdentityUser>().AddEntityFrameworkStores<ApplicationDbContext>();

// Adds endpoints for minimal api controllers used in app.MapIdentityApi<IdentityUser>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    var basePath = "/api/v1/";
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "swagger/{documentName}/swagger.json";
        options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
        {
            // Configure base path
            swaggerDoc.Servers = new List<OpenApiServer>{ new() { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}{basePath}" } };
            
            // Remove existing base path from endpoints
            OpenApiPaths paths = new();
            foreach (var path in swaggerDoc.Paths)
            {
                paths.Add(path.Key.Replace(basePath, "/"), path.Value);
            }
            swaggerDoc.Paths = paths;
        });
    });
    app.UseSwaggerUI();
}

// Adds authentication endpoints 
app.MapGroup("/api/v1/").MapIdentityApi<IdentityUser>();
app.MapSwagger().RequireAuthorization();

app.UseHttpsRedirection();

// Redirect from root directly to swagger docs
var options = new RewriteOptions();
options.AddRedirect("^$", "swagger");
app.UseRewriter(options);

app.UseAuthorization();
app.MapControllers();

app.Run();
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using App;

var builder = WebApplication.CreateBuilder(args);
Util.configuration = builder.Configuration;
Util.rootPath = builder.Environment.ContentRootPath;
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
    options.JsonSerializerOptions.Converters.Add(new TimeSpanConverter());
    options.JsonSerializerOptions.Converters.Add(new BooleanConverter());
    options.JsonSerializerOptions.Converters.Add(new NumberConverterFactory());
});
builder.Services.AddAuthentication(options =>  
{  
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;  
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;  
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;  
}).AddJwtBearer(options =>  
{  
    options.SaveToken = true;  
    options.RequireHttpsMetadata = false;  
    options.TokenValidationParameters = new TokenValidationParameters()  
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT-Secret"]))
    };  
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => {
        policy.AllowAnyOrigin();
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
    });
});
builder.Services.AddDbContext<App.Models.DataContext>(options => options.UseMySQL(builder.Configuration.GetConnectionString("Database")));
var app = builder.Build();
app.UseExceptionHandler(e => e.Run(async context =>
{
    var corsService = context.RequestServices.GetService<ICorsService>();
    var corsPolicyProvider = context.RequestServices.GetService<ICorsPolicyProvider>();
    var corsPolicy = await corsPolicyProvider.GetPolicyAsync(context, null);
    corsService.ApplyResult(corsService.EvaluatePolicy(context, corsPolicy), context.Response);
    var exception = context.Features.Get<IExceptionHandlerFeature>().Error;
    if (exception.InnerException != null) {
        exception = exception.InnerException;
    }
    var error = JsonSerializer.Serialize(new { message = exception.Message });
    await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(error));
}));
app.UseHttpsRedirection();
app.UseCors();
var uploads = Path.Combine(builder.Environment.ContentRootPath, "uploads");
if (!Directory.Exists(uploads)) {
    Directory.CreateDirectory(uploads);
}
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploads), RequestPath = "/uploads"
});
app.UsePathBase(new PathString("/api"));
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
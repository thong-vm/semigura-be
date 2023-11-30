#if !DEBUG 
using Logger;
#endif
using GraphQL;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Models;
using Repositories;
using semigura.Commons;
using semigura.DAL;
using semigura.DBContext.Business;
using semigura.DBContext.Entities;
using semigura.Hubs;
using semigura.Models;
using semigura.Repositories;
using System.Text;
using Template;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSystemWebAdapters();
builder.Services.AddHttpForwarder();

builder.Services.AddPortableObjectLocalization();
builder.Services
    .Configure<RequestLocalizationOptions>(options => options
        .AddSupportedCultures("en", "ja")
        .AddSupportedUICultures("en", "ja"));



builder.Services.AddDbContext<DBEntities>(option =>
{

    var usedCnn = builder.Configuration["UsedConnectionString"];
    var connectionString = string.IsNullOrEmpty(usedCnn)
        ? builder.Configuration.GetConnectionString("DefaultConnection")
        : builder.Configuration.GetConnectionString(usedCnn);
    if (usedCnn == "PSQLDB")
    {
        option.UseNpgsql(connectionString);
    }
    else
    {
        option.UseSqlServer(connectionString);
    }
});

// CORS
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("*")
                            .WithHeaders("*")
                            .WithMethods("*");
                      });
});

Properties.ENVIRONMENT_ROOT_PATH = builder.Environment.ContentRootPath;
builder.Services.Configure<MyConfig>(cfg => { cfg.ContentRootPath = builder.Environment.ContentRootPath; });

// add SignalR to the ASP.NET Core dependency injection
builder.Services.AddSignalR();

// authentication
const string AuthSchemes = "JWT_OR_COOKIE";
builder.Services
.AddAuthentication(options =>
{
    options.DefaultScheme = AuthSchemes;
    options.DefaultChallengeScheme = AuthSchemes;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            };
    })
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.AccessDeniedPath = "/S01001/UnAuthorized";
        options.LoginPath = "/S01001/Index";
    })
    .AddPolicyScheme(AuthSchemes, AuthSchemes, options =>
    {
        options.ForwardDefaultSelector = context =>
        {
            string authorization = context.Request.Headers[HeaderNames.Authorization];
            if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                return JwtBearerDefaults.AuthenticationScheme;

            return CookieAuthenticationDefaults.AuthenticationScheme;
        };
    });

// Repository
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<AuthCookieRepository>();
builder.Services.AddScoped<S01001Business>();
builder.Services.AddScoped<S01002Business>();
builder.Services.AddScoped<S02001Business>();
builder.Services.AddScoped<S02002Business>();
builder.Services.AddScoped<S02003Business>();
builder.Services.AddScoped<S03001Business>();
builder.Services.AddScoped<S03002Business>();
builder.Services.AddScoped<S03005Business>();
builder.Services.AddScoped<S03006Business>();
builder.Services.AddScoped<S09001Business>();
builder.Services.AddScoped<S09002Business>();
builder.Services.AddScoped<S09003Business>();
builder.Services.AddScoped<S09004Business>();
builder.Services.AddScoped<ProcessBusiness>();
builder.Services.AddScoped<TRepository<Sensor, DBEntities>, SensorRepository>();

builder.Services.AddScoped<IChatHubRepository, ChatHubRepository>();

builder.Services.AddGraphQLServer()
    .AddAuthorization()
    .AddProjections()
    .AddFiltering()
    .AddSorting()
    .AddType<UploadType>()
    .AddInMemorySubscriptions()
    .AddQueryType<Query>()
        .AddType<TQueryTypeExtension<Sensor>>()
    .AddMutationType<Mutation>()
        .AddType<TMutateTypeExtension<Sensor>>()
    .AddSubscriptionType<Subscription>()
        .AddTypeExtension<TSubscriptionTypeExtension<Sensor>>()
    ;


// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
// Session 
builder.Services.AddDistributedMemoryCache();

//builder.Services.AddSession(options =>
//{
//    options.Cookie.Name = ".SemiguraWorks.Session";
//    options.IdleTimeout = TimeSpan.FromSeconds(10);
//    options.Cookie.IsEssential = true;
//});

// log
#if !DEBUG
builder.Host.ConfigureLogging((hostContext, logBuilder) =>
        logBuilder.ClearProviders()
            .AddFileLogger(configuration =>
            {
                hostContext.Configuration.GetSection("Logging").GetSection("RoundTheCodeFile").GetSection("Options").Bind(configuration);
            }));
#endif

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(System.IO.Path.Combine(builder.Environment.ContentRootPath, "Content")),
    RequestPath = "/Content"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(System.IO.Path.Combine(builder.Environment.ContentRootPath, "wwwroot"))
});

app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.UseRequestLocalization();

// user Session
//app.UseSession();

app.UseSystemWebAdapters();

app.MapControllerRoute("S01001", "account/{action=Index}/{id?}", new { controller = "S01001" });

app.MapControllerRoute("S01002", "home/{action=Index}/{id?}", new { controller = "S01002" });

app.MapControllerRoute("S02001", "moromi/{action=Index}/{id?}", new { controller = "S02001" });

app.MapControllerRoute("S02002", "seigiku/{action=Index}/{id?}", new { controller = "S02002" });

app.MapControllerRoute("S02003", "location/{action=Index}/{id?}", new { controller = "S02003" });

app.MapControllerRoute("S03001", "lot/{action=Index}/{id?}", new { controller = "S03001" });

app.MapControllerRoute("S03002", "tank/{action=Index}/{id?}", new { controller = "S03002" });

app.MapControllerRoute("S03005", "alert/{action=Index}/{id?}", new { controller = "S03005" });

app.MapDefaultControllerRoute();
app.MapForwarder("/{**catch-all}", app.Configuration["ProxyTo"]).Add(static builder => ((RouteEndpointBuilder)builder).Order = int.MaxValue);

app.MapControllerRoute("Default", "{controller=S01002}/{action=Index}/{id?}");

app.MapGraphQL("/graphql");
// add SignalR to routing systems
app.MapHub<StronglyTypedChatHub>("/chatHub");
app.Run();

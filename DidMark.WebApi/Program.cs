using CodecellShare.Interfaces;
using DidMark.Core.Security;
using DidMark.Core.Services.Implementations;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Convertors;
using DidMark.DataLayer.Db;
using DidMark.DataLayer.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PayamakCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with JWT authentication
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DidMark API",
        Version = "v1",
        Description = "API for DidMark application with JWT authentication"
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token in the format: Bearer {token}"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// Add configuration
builder.Services.AddSingleton<IConfiguration>(
    new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build()
);

#region Database Configuration
ConfigurationManager configuration = builder.Configuration;
builder.Services.AddDbContext<MasterContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
#endregion

#region Application Services
builder.Services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddPayamakService();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();
builder.Services.AddScoped<IProductGalleryService, ProductGalleryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddScoped<ISliderService, SliderService>();
builder.Services.AddScoped<IPasswordHelper, PasswordHelper>();
builder.Services.AddScoped<IMailSender, SendEmail>();
builder.Services.AddScoped<IViewRenderService, RenderViewToString>();
builder.Services.AddScoped<IContactUs, ContactUsService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAccessService, AccessService>();
builder.Services.AddScoped<IOffCodeService, OffCodeService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ITransactionLogService, TransactionLogService>();
builder.Services.AddScoped<IZarinPalService, ZarinPalService>();
builder.Services.AddScoped<IAttributeService, AttributeService>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddMvc();
#endregion

#region Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["JwtSettings:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"] ?? "DidmarkJwtBearer"))
        };
    });
#endregion

#region CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("EnableCors", builder =>
    {
        //builder.WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>())
        //       .AllowAnyHeader()
        //       .AllowAnyMethod()
        //       .AllowCredentials();
        builder.AllowAnyOrigin()
       .AllowAnyHeader()
       .AllowAnyMethod();
    });
});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "DidMark API v1");
        options.RoutePrefix = "swagger"; // Set Swagger UI to be accessible at /swagger
    });
    app.Logger.LogInformation("Swagger UI is enabled at /swagger");
}
else
{
    app.Logger.LogInformation("Swagger UI is disabled in non-Development environment");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseDefaultFiles();
app.UseCors("EnableCors");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

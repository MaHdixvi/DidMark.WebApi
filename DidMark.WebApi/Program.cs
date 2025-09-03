using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;
using DidMark.DataLayer.Db;
using DidMark.DataLayer.Repository;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Services.Implementations;
using DidMark.Core.Security;
using DidMark.Core.Utilities.Convertors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SpaServices.AngularCli;

var builder = WebApplication.CreateBuilder(args);

//Add services to the container.

builder.Services.AddControllers();
//Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton<IConfiguration>(
                new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile($"appsettings.json")
                    .Build()
            );



#region Add DbContext

ConfigurationManager configuration = builder.Configuration;
builder.Services.AddDbContext<MasterContext>(options =>
    options.UseSqlServer(
        configuration.GetConnectionString("DefaultConnection"))
    );

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

#endregion

#region Application Services

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISliderService, SliderService>();
builder.Services.AddScoped<IPasswordHelper, PasswordHelper>();
builder.Services.AddScoped<IMailSender, SendEmail>();
builder.Services.AddScoped<IViewRenderService, RenderViewToString>();
builder.Services.AddScoped<IContactUs, ContactUsService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAccessService, AccessService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddMvcCore().AddRazorViewEngine();
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddCors();
builder.Services.AddMvc();

IInputFormatter GetJsonPatchInputFormatter()
{
    throw new NotImplementedException();
}

#endregion


#region Authentication
//https://localhost:7265
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "https://api.Allapar.com",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("DidmarkJwtBearer"))
        };
    });

#endregion

#region CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("EnableCors", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .Build();
    });
});

#endregion

builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "frontend-project/dist/vira-aron-raika";
});



var app = builder.Build();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (!app.Environment.IsDevelopment())
{
    app.UseSpaStaticFiles();
}
app.UseSpa(spa =>
{
    spa.Options.SourcePath = "frontend-project";
    if (!app.Environment.IsDevelopment())
    {
        spa.UseAngularCliServer(npmScript: "start");
    }
});


app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseCors("EnableCors");
app.UseCors(builder => builder
    .AllowAnyHeader()
    .AllowAnyMethod()
    .SetIsOriginAllowed((host) => true)
    .AllowCredentials()
);


app.UseAuthentication();

app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();

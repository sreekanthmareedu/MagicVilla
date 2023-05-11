using MagicVilla_VillaAPI;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.logging;
using MagicVilla_VillaAPI.Repository;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Configuring logger for serilog 
/*Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
    .WriteTo.File("logs/Villa.txt",rollingInterval:RollingInterval.Day).CreateLogger();
builder.Host.UseSerilog();*/

//Custom logging
builder.Services.AddSingleton<Ilogging,LoggingV2>();

var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");
//link connection string with DBcontext.cs
builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLString"));

});
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});


builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;

});

builder.Services.AddResponseCaching();

builder.Services.AddScoped<IVillaRepository,VillaRepository>();
builder.Services.AddScoped<IUserRepository,UserRepository>();
builder.Services.AddScoped<IVillaNumberRepository,VillaNumberRepository>();
builder.Services.AddAuthentication(u =>
{
    u.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

    u.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(u => {

    u.RequireHttpsMetadata = false;
    u.SaveToken = true;
    u.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience= false
    };
});


//Add auto mapper

builder.Services.AddAutoMapper(typeof(MappingConfig));

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.CacheProfiles.Add("Default30", new CacheProfile()
    {


        Duration = 30
    }) ;



});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
                 {
                     Type = ReferenceType.SecurityScheme,
                      Id = "Bearer"
                  },
              Scheme = "oauth2",
              Name = "Bearer",
              In = ParameterLocation.Header,

            },
            new List<string>()
          }
        });

    options.SwaggerDoc("v1", new OpenApiInfo
    {

        Version = "v1",
        Title = "Magic Villa v1",
        Description = "API to Manage Villa",
        TermsOfService = new Uri("http://example.com/terms")
    });

    options.SwaggerDoc("v2", new OpenApiInfo
    {

        Version = "v2",
        Title = "Magic Villa v2",
        Description = "API to Manage Villa",
        TermsOfService = new Uri("http://example.com/terms")
    });
}
    
    
    
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json","Magic_VillaV1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "Magic_VillaV2");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

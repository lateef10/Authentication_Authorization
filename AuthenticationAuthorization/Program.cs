using AuthenticationAuthorization.Common;
using AuthenticationAuthorization.Custome_Authorization;
using AuthenticationAuthorization.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));
builder.Services.AddDbContext<ApplicationDbContextIdentity>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(option =>
{
    option.Password.RequiredLength = 5;
    option.Password.RequireNonAlphanumeric = false;
    option.Password.RequireDigit = false;
    option.Password.RequireUppercase = false;

    option.Lockout.MaxFailedAccessAttempts = 5;
    option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

    option.User.RequireUniqueEmail = true;
    option.SignIn.RequireConfirmedEmail = true;
}).AddEntityFrameworkStores<ApplicationDbContextIdentity>()
.AddDefaultTokenProviders(); //add to generate token for email or other verification

var jwtOptions = new JwtOptions();
builder.Configuration.GetSection("jwt").Bind(jwtOptions);

builder.Services.AddSingleton(jwtOptions);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtOptions.JwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtOptions.JwtIssuer,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.JwtKey)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

builder.Services.AddAuthorization(options =>
{
    //options.AddPolicy("HasNationality", builder => builder.RequireClaim("Nationality"));
    options.AddPolicy("AtLeast18", builder => builder.AddRequirements(new MinimumAgeRequirement(18)));
});

builder.Services.AddScoped<IAuthorizationHandler, MinimumAgeHandler>();

builder.Services.AddScoped<IPasswordHasher<UserProp>, PasswordHasher<UserProp>>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWTToken_Auth_API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

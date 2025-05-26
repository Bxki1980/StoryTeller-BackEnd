using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StoryTeller.StoryTeller.Backend.StoryTeller.API.Middlewares;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Services;
using StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.Repositories;
using StoryTeller.StoryTeller.Backend.StoryTeller.Shared.Logger;
using StoryTeller.StoryTeller.Backend.StoryTeller.Shared.Setting;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Register services before Build
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<IUserRepository, UserRepository>(); // Your Cosmos repo
builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IUnitOfWork, CosmosUnitOfWork>();
builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddSingleton<ILoggerManager, LoggerManager>();




// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            )
        };
    });


builder.Services.AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection("Jwt"))
    .ValidateDataAnnotations()
    .ValidateOnStart(); // optional but recommended (validates at startup)


// Google Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});



var app = builder.Build();

// Middlewares
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Azure.Cosmos;
using Microsoft.IdentityModel.Tokens;
using StoryTeller.StoryTeller.Backend.StoryTeller.API.Middlewares;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Repositories.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Repositories.Book;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Services.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Interfaces.Services.Book;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Services.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Services.Book;
using StoryTeller.StoryTeller.Backend.StoryTeller.Application.Validators;
using StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.BlobStorage;
using StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.Repositories.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.Repositories.Book;
using StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.Services.Auth;
using StoryTeller.StoryTeller.Backend.StoryTeller.Shared.Logger;
using StoryTeller.StoryTeller.Backend.StoryTeller.Shared.Setting;
using System.Text;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);
//this line to retrieve the configuration object
var config = builder.Configuration;

// Register services before Build

builder.Services
    .AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();

builder.Services.AddValidatorsFromAssemblyContaining<CreateBookDtoValidator>();

builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton<CosmosClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return StoryTeller.StoryTeller.Backend.StoryTeller.Infrastructure.Database.CosmosDbFactory.CreateClient(config);
});


builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IPageRepository, PageRepository>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IPageService, PageService>();
builder.Services.AddScoped<IUserRepository, UserRepository>(); // Your Cosmos repo
builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
builder.Services.AddScoped<IGoogleClaimsParser, GoogleClaimsParser>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IGoogleClaimsParser, GoogleClaimsParser>();
builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddSingleton<ILoggerManager, LoggerManager>();
builder.Services.AddScoped<IBlobUrlGenerator, BlobUrlGenerator>();



Console.WriteLine($"Cosmos DB: {builder.Configuration["Cosmos:Database"]}");
Console.WriteLine($"Cosmos Container: {builder.Configuration["Cosmos:UserContainer"]}");


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
builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();


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

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5224); // for HTTP
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



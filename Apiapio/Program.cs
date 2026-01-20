using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Apiapio.Repositories;
using Apiapio.Services;
using Apiapio.Middleware;
using System.Text;
using Apiapio.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with JWT support
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Photos Gateway API",
        Version = "v1",
        Description = "API Gateway con autenticación JWT que consume JSONPlaceholder"
    });

    // Configurar JWT en Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa tu token JWT en este formato: Bearer {tu token}"
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
            Array.Empty<string>()
        }
    });
});

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register Caches as Singleton
builder.Services.AddSingleton<InMemoryUserCache>();
builder.Services.AddSingleton<InMemoryAlbumCache>();
builder.Services.AddSingleton<InMemoryUserStore>();

// Register HttpClients and Repositories
builder.Services.AddHttpClient<IPhotoRepository, PhotoRepository>(client =>
{
    var baseUrl = builder.Configuration["ExternalApis:JsonPlaceholder:BaseUrl"] 
        ?? "https://jsonplaceholder.typicode.com/";
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    
    var timeout = builder.Configuration.GetValue<int>("ExternalApis:JsonPlaceholder:Timeout", 30);
    client.Timeout = TimeSpan.FromSeconds(timeout);
});

builder.Services.AddHttpClient<IUserRepository, UserRepository>(client =>
{
    var baseUrl = builder.Configuration["ExternalApis:JsonPlaceholder:BaseUrl"] 
        ?? "https://jsonplaceholder.typicode.com/";
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    
    var timeout = builder.Configuration.GetValue<int>("ExternalApis:JsonPlaceholder:Timeout", 30);
    client.Timeout = TimeSpan.FromSeconds(timeout);
});

builder.Services.AddHttpClient<IAlbumRepository, AlbumRepository>(client =>
{
    var baseUrl = builder.Configuration["ExternalApis:JsonPlaceholder:BaseUrl"] 
        ?? "https://jsonplaceholder.typicode.com/";
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    
    var timeout = builder.Configuration.GetValue<int>("ExternalApis:JsonPlaceholder:Timeout", 30);
    client.Timeout = TimeSpan.FromSeconds(timeout);
});

// Register Services
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// IMPORTANTE: El orden es crucial
app.UseAuthentication();  // Primero autenticación
app.UseAuthorization();   // Luego autorización

app.MapControllers();

app.Run();

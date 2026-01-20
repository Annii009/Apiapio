using Apiapio.Repositories;
using Apiapio.Services;
using Apiapio.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Photos Gateway API",
        Version = "v1",
        Description = "API Gateway que consume JSONPlaceholder para gestión de Photos, Users y Albums"
    });
});

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
app.UseAuthorization();
app.MapControllers();

app.Run();

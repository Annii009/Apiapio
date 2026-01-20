
using Apiapio.Services;
using Apiapio.Middleware;
using Apiapio.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Register Repository and Service
builder.Services.AddHttpClient<IPhotoRepository, PhotoRepository>(client =>
{
    var baseUrl = builder.Configuration["ExternalApis:JsonPlaceholder:BaseUrl"] 
        ?? "https://jsonplaceholder.typicode.com/";
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    
    var timeout = builder.Configuration.GetValue<int>("ExternalApis:JsonPlaceholder:Timeout", 30);
    client.Timeout = TimeSpan.FromSeconds(timeout);
});

builder.Services.AddScoped<IPhotoService, PhotoService>();

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

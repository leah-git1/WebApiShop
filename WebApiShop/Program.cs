using Microsoft.EntityFrameworkCore;
using Repository;
using Services;
using WebApiShop;
using WebApiShop.Middleware;
using StackExchange.Redis;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IUsersServices, UsersServices>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddDbContext<ShopContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Redis connection with proper options
var redisHost = builder.Configuration["RedisSettings:Host"] ?? "localhost";
var redisPort = int.TryParse(builder.Configuration["RedisSettings:Port"], out var port) ? port : 6379;
var redisPassword = builder.Configuration["RedisSettings:Password"];
var connectTimeout = int.TryParse(builder.Configuration["RedisSettings:ConnectTimeoutMs"], out var timeout) ? timeout : 5000;
var abortOnConnectFail = bool.TryParse(builder.Configuration["RedisSettings:AbortOnConnectFail"], out var abort) ? abort : false;

var redisOptions = new ConfigurationOptions
{
    EndPoints = { { redisHost, redisPort } },
    Password = redisPassword,
    ConnectTimeout = connectTimeout,
    AbortOnConnectFail = abortOnConnectFail,
    AllowAdmin = false,
    Ssl = false
};

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<Program>>();
    try
    {
        var connection = ConnectionMultiplexer.Connect(redisOptions);
        logger.LogInformation("✓ Redis connection established successfully: {Host}:{Port}", redisHost, redisPort);
        return connection;
    }
    catch (RedisConnectionException ex)
    {
        logger.LogError("✗ Redis connection failed: {Message}", ex.Message);
        logger.LogWarning("Continuing without Redis caching. Ensure Redis is running and credentials are correct.");
        throw;
    }
    catch (Exception ex)
    {
        logger.LogError("✗ Unexpected error connecting to Redis: {Message}", ex.Message);
        throw;
    }
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseErrorHandling();

app.UseRating();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();

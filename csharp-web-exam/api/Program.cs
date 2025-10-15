using api.API.Endpoints;
using api.API.Middleware;
using api.Application.Interfaces;
using api.Application.Services;
using api.Application.UseCases.Categories;
using api.Application.UseCases.Products;
using api.Infrastructure.Data;
using api.Infrastructure.Repositories;
using api.Infrastructure.Security;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Configure log4net
ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly()!);
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

ILog log = LogManager.GetLogger(typeof(Program));
log.Info("Application starting...");

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "CSharp Web Exam API",
        Version = "v1",
        Description = "RESTful API for managing products and categories with Clean Architecture, Minimal API, Dapper ORM, and JWT Authentication"
    });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure JWT Authentication
IConfigurationSection jwtSettings = builder.Configuration.GetSection("JwtSettings");
string secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

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

// Get API Settings from configuration
IConfigurationSection apiSettings = builder.Configuration.GetSection("ApiSettings");
bool enableSwagger = apiSettings.GetValue<bool>("EnableSwagger", true);
bool enableCors = apiSettings.GetValue<bool>("EnableCors", true);
string corsOrigins = apiSettings.GetValue<string>("CorsOrigins", "*")!;

// Configure CORS based on environment
if (enableCors)
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("ApiCorsPolicy", policy =>
        {
            if (corsOrigins == "*")
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            }
            else
            {
                string[] origins = corsOrigins!.Split(',', StringSplitOptions.RemoveEmptyEntries);
                policy.WithOrigins(origins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            }
        });
    });
}

// Register Database Connection Factory
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

builder.Services.AddSingleton<IDbConnectionFactory>(sp => 
    new SqliteConnectionFactory(connectionString));

// Register Repositories
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register Services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Register JWT Token Generator
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

// Register Use Cases - Categories
builder.Services.AddScoped<GetAllCategoriesUseCase>();
builder.Services.AddScoped<GetCategoryByIdUseCase>();
builder.Services.AddScoped<CreateCategoryUseCase>();
builder.Services.AddScoped<UpdateCategoryUseCase>();
builder.Services.AddScoped<DeleteCategoryUseCase>();

// Register Use Cases - Products
builder.Services.AddScoped<GetProductsUseCase>();
builder.Services.AddScoped<GetProductByIdUseCase>();
builder.Services.AddScoped<GetGroupedProductsUseCase>();
builder.Services.AddScoped<CreateProductUseCase>();
builder.Services.AddScoped<UpdateProductUseCase>();
builder.Services.AddScoped<DeleteProductUseCase>();

// Register DbInitializer
builder.Services.AddScoped<DbInitializer>();

WebApplication app = builder.Build();

// Initialize database
using (IServiceScope scope = app.Services.CreateScope())
{
    try
    {
        DbInitializer dbInitializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
        await dbInitializer.InitializeAsync();
        log.Info("Database initialized successfully");
    }
    catch (Exception ex)
    {
        log.Error("Error initializing database", ex);
        throw;
    }
}

// Configure the HTTP request pipeline
log.Info($"Running in {app.Environment.EnvironmentName} environment");

// Enable Swagger based on configuration
if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CSharp Web Exam API v1");
        options.RoutePrefix = string.Empty; // Swagger at root
        options.DocumentTitle = $"API Documentation - {app.Environment.EnvironmentName}";
    });
    log.Info("Swagger UI enabled");
}
else
{
    log.Info("Swagger UI disabled");
}

// Global exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

// Use CORS if enabled
if (enableCors)
{
    app.UseCors("ApiCorsPolicy");
    log.Info($"CORS enabled with origins: {corsOrigins}");
}

// Use Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();
log.Info("JWT Authentication and Authorization enabled");

// Map Minimal API Endpoints
app.MapAuthEndpoints();
app.MapCategoryEndpoints();
app.MapProductEndpoints();

log.Info($"Application started successfully on {app.Environment.EnvironmentName} environment");

app.Run();
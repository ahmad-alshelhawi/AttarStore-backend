using AttarStore.Api.Utils;
using AttarStore.Repositories;
using AttarStore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.Always;
});

// 1) Load configuration (including JWT settings) from appsettings.json

// 2) Read JWT settings
//var jwt_key = builder.Configuration["JWT:Key"];
//var jwt_issuer = builder.Configuration["JWT:Issuer"];
//var jwt_audience = builder.Configuration["JWT:Audience"];

// 3) CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AttarStorePolicy", policy =>
       policy.WithOrigins("http://localhost:3000") // your React dev origin
             .AllowAnyMethod()
             .AllowAnyHeader()
             .AllowCredentials());
});

// ✅ Add this so SameSite=None cookies are honored:
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


// 4) Authentication (JWT Bearer)
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
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])
        )
    };

    // 👇 THIS allows reading from cookie instead of Authorization header
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("accessToken"))
            {
                context.Token = context.Request.Cookies["accessToken"];
            }
            return Task.CompletedTask;
        }
    };
});

// ?? **NEW**: add authorization middleware registration
builder.Services.AddAuthorization();

// 5) EF Core DbContext (unchanged)
builder.Services.AddDbContextPool<AppDbContext>(e =>
{
    var conStr = builder.Environment.IsDevelopment() ? "dev" : "main";
    e.UseSqlServer(builder.Configuration.GetConnectionString(conStr));
});

// 6) AutoMapper, Repos & Services (unchanged + one new)
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IActionLogRepository, ActionLogRepository>();
builder.Services.AddScoped<IManufacturerRepository, ManufacturerRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IBillingRepository, BillingRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();  // ?? **NEW**
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<TokenService>();

// 7) Controllers & Swagger (unchanged)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Attar Store", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token 'ONLY'."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();
// ✅ Use the cookie policy before auth


// 8) Middleware pipeline (unchanged)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCookiePolicy();
app.UseCors("AttarStorePolicy");
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();   // ensure this is before UseAuthorization()
app.UseAuthorization();

app.MapControllers();
app.ApplyMigrations();
app.Run();

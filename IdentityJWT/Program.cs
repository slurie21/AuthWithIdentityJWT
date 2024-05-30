using IdentityJWT.DataAccess.Context;
using IdentityJWT.Models.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
var connString = builder.Configuration.GetConnectionString("DefaultConn") ?? throw new InvalidOperationException("no connection string defined");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connString)
);
//builder.Services.AddIdentity
//builder.Services.AddIdentityCore
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddApiEndpoints()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

//var jwt_secret = Environment.GetEnvironmentVariable("JWT_Secret") ?? throw new InvalidOperationException("no JWT secret set");
var jwt_secret = builder.Configuration["JWT_Secret"] ?? throw new InvalidOperationException("no JWT secret set");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt_secret)), // Ensure your key is secure
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true
    };
    // If your JWT tokens include expiration, ensure clock skew is considered
    options.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

using System.IdentityModel.Tokens.Jwt;
using System.Text;
using DemoRest20232;
using DemoRest20232.Auth;
using DemoRest20232.Auth.Model;
using DemoRest20232.Data;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// Microsoft.AspNetCore.Identity
// Microsoft.AspNetCore.Identity.EntityFrameworkCore
// Microsoft.AspNetCore.Authentication.JwtBearer

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ForumDbContext>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddTransient<JwtTokenService>();
builder.Services.AddScoped<AuthDbSeeder>();

builder.Services.AddIdentity<ForumRestUser, IdentityRole>()
    .AddEntityFrameworkStores<ForumDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters.ValidAudience = builder.Configuration["Jwt:ValidAudience"];
    options.TokenValidationParameters.ValidIssuer = builder.Configuration["Jwt:ValidIssuer"];
    options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]));
});

builder.Services.AddAuthorization();

var app = builder.Build();

var topicsGroup = app.MapGroup("/api").WithValidationFilter();
TopicEndpoints.AddTopicApi(topicsGroup);

// app.AddPostApi();

app.AddAuthApi();

app.UseAuthentication();
app.UseAuthorization();

using var scope = app.Services.CreateScope();

var dbContext = scope.ServiceProvider.GetRequiredService<ForumDbContext>();
dbContext.Database.Migrate();

var dbSeeder = scope.ServiceProvider.GetRequiredService<AuthDbSeeder>();
await dbSeeder.SeedAsync();

app.Run();

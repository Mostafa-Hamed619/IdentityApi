using AdminFullStack.Data;
using AdminFullStack.Models;
using AdminFullStack.Services;
using Google.Apis.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<Context>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentityCore<User>(opt =>
{
    opt.Password.RequiredLength = 6;
    opt.Password.RequireDigit = false;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireNonAlphanumeric = false;
    
    opt.SignIn.RequireConfirmedEmail = true;
    
}).AddRoles<IdentityRole>() //Enable adding roles
.AddRoleManager<RoleManager<IdentityRole>>() // be able to use of RoleManager
.AddEntityFrameworkStores<Context>() // providing our Context
.AddSignInManager<SignInManager<User>>(). // Make use of Signin manager
AddUserManager<UserManager<User>>() // Make the use of UserManager to create
.AddDefaultTokenProviders(); // be able to create token for email confirmation

// be able to inject jwtservices inside our controllers
builder.Services.AddScoped<JWTServices>();
builder.Services.AddScoped<EmailServices>();
builder.Services.AddScoped<ContextSeedServices>();

// be able to authenticate users using jwt
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidateAudience = false,
        };
    });
builder.Services.AddCors();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = action =>
    {
        var error = action.ModelState.
        Where(x => x.Value.Errors.Count > 0)
        .SelectMany(x => x.Value.Errors)
        .Select(x => x.ErrorMessage).ToArray();

        var toReturn = new
        {
            Errors = error
        };
        return new BadRequestObjectResult(toReturn);
    };
});

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("Adminpolicy", policy => policy.RequireRole("Admin"));
    opt.AddPolicy("Managerpolicy", policy => policy.RequireRole("Manager"));
    opt.AddPolicy("Playerpolicy", policy => policy.RequireRole("Player"));

    opt.AddPolicy("AdminOrManagerPolicy", policy => policy.RequireRole("Admin", "Manager"));
    opt.AddPolicy("AdminAndManagerPolicy", policy => policy.RequireRole("Admin").RequireRole("Manager"));
    opt.AddPolicy("AllRolePolicy", policy => policy.RequireRole("Admin", "Manager","Player"));

    opt.AddPolicy("AdminEmailPolicy", policy => policy.RequireClaim(ClaimTypes.Email, "MosSEffy21@gmail.com"));
    opt.AddPolicy("IanSureNamePolicy", policy => policy.RequireClaim(ClaimTypes.Surname, "Nepomniachtchi"));
});
var app = builder.Build();

app.UseCors(opt =>
{
    opt.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(builder.Configuration["JWT:ClientUrl"]);
    //so we get the url from appsettings.development.json in order to put multiple url
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

#region ContextSeed
using var scope = app.Services.CreateScope();
#endregion
try
{
    var contextSeedServices = scope.ServiceProvider.GetService<ContextSeedServices>();
    await contextSeedServices.InitializeContextAsync();
}
catch (Exception ex)
{
    var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
    logger.LogError(ex.Message,"Failed to initialize and seed the database");
}

app.Run();

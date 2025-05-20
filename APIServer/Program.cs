using AccountData;
using EntityFramework;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Server.Seeders;

namespace Server;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Configuration.AddEnvironmentVariables();
        builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
        
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                policy => policy.WithOrigins("https://localhost:3000") // Match frontend URL
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()); // 🔥 Required for cookies
        });

        
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // 🔥 Required for HTTPS
            options.Cookie.SameSite = SameSiteMode.None;  // 🔥 Required for cross-origin requests
            options.Cookie.Path = "/";
        });



        
        builder.Services.AddDbContext<Context>(options =>
        {
            options.UseAzureSql(builder.Configuration.GetConnectionString("AzureSqlDataConnectionString"));
        });
        
        builder.Services.AddDbContext<AccountContext>(options =>
        {
            options.UseAzureSql(builder.Configuration.GetConnectionString("AzureSqlAccountConnectionString"));
        });

        builder.Services.AddAuthorization();
        builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 1;
        }).AddEntityFrameworkStores<AccountContext>();
        
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        
        //Roles
        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var roleSeeder = new RoleSeeder(roleManager);
            await roleSeeder.SeedRolesAsync();
        }
        
        app.UseCors("AllowAll");
        
        // Migrate/Create the database
        using (var scope = app.Services.CreateScope())
        {
            // Get context from dependency injection
            // Type or throws: GetRequiredService<Type>();
            // Type or throws: GetRequiredService(Type);
            // Type or null: GetService<Type>();
            // Type or null: GetService(Type);
            var context = scope.ServiceProvider.GetRequiredService<Context>();
            var accountContext = scope.ServiceProvider.GetRequiredService<AccountContext>();

            context.Database.EnsureCreated();
            accountContext.Database.EnsureCreated();
            
            // Check if database supports migrations
            // if (context.Database.IsRelational())
            // {
            //     context.Database.Migrate();
            // }
            // else
            // {
            //     context.Database.EnsureCreated();
            // }
            
            // Check if database supports migrations
            // if (accountContext.Database.IsRelational())
            // {
            //     accountContext.Database.Migrate();
            // }
            // else
            // {
            //     accountContext.Database.EnsureCreated();
            // }
        }

        // Configure the HTTP request pipeline.
        // if (app.Environment.IsDevelopment())
        // {
        //     app.UseSwagger();
        //     app.UseSwaggerUI();
        // }
        
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.MapControllers();

        app.Run(); 
    }
}
using AccountData;
using EntityFramework;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Server;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Configuration.AddEnvironmentVariables();
        builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
        
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                policy => policy.WithOrigins("http://localhost:63343") // Change to your frontend URL
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()); // Required for cookies
        });
        
        // Retrieve the connection string from environment variables (if set)
        var genericDbConnectionString = builder.Configuration.GetConnectionString("SqliteDefaultConnectionString");
        
        builder.Services.AddDbContext<Context>(options =>
        {
            options.UseSqlite(genericDbConnectionString);
        });
        
        var accountDbConnectionString = builder.Configuration.GetConnectionString("SqliteAccountConnectionString");
        builder.Services.AddDbContext<AccountContext>(options =>
        {
            options.UseSqlite(accountDbConnectionString);
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
            if (accountContext.Database.IsRelational())
            {
                accountContext.Database.Migrate();
            }
            else
            {
                accountContext.Database.EnsureCreated();
            }
        }

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
    }
}
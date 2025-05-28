using AgriEnergyConnect.API.Data;
using AgriEnergyConnect.API.Models;
using AgriEnergyConnect.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace AgriEnergyConnect.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AgriEnergyDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlServerOptions =>
                    {
                        sqlServerOptions.EnableRetryOnFailure(
                            maxRetryCount: 15,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                        sqlServerOptions.CommandTimeout(120);  
                    });
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            });

            
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            })
            .AddEntityFrameworkStores<AgriEnergyDbContext>()
            .AddDefaultTokenProviders();

            // Configure JWT
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])),
                    ClockSkew = TimeSpan.Zero
                };
            });

           
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AgriEnergyConnect.API", Version = "v1" });

                
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Enter JWT token like this: Bearer {your token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
            
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IFarmerService, FarmerService>();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

           
            app.UseCors(policy => policy
                .WithOrigins(builder.Configuration.GetSection("ApiSettings:CorsOrigins").Get<string[]>())
                .AllowAnyMethod()
                .AllowAnyHeader());

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();  
            app.UseAuthorization();   


            
            app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));

            
            await InitializeDatabase(app);

            app.MapControllers();
            app.Run();

            async Task InitializeDatabase(WebApplication app)
            {
                using var scope = app.Services.CreateScope();
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                var config = services.GetRequiredService<IConfiguration>();

                var maxRetryAttempts = 15;  
                var pauseBetweenFailures = TimeSpan.FromSeconds(15);  

                for (int i = 0; i < maxRetryAttempts; i++)
                {
                    try
                    {
                        logger.LogInformation("Attempting to connect to database (Attempt {Attempt})...", i + 1);

                        var context = services.GetRequiredService<AgriEnergyDbContext>();

                        
                        if (await context.Database.CanConnectAsync())
                        {
                            logger.LogInformation("Database connection established");

                            
                            await context.Database.MigrateAsync();
                            logger.LogInformation("Migrations applied successfully");

                            
                            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                            string[] roles = { "Admin", "Farmer", "Employee", "HR" };

                            foreach (var role in roles)
                            {
                                if (!await roleManager.RoleExistsAsync(role))
                                {
                                    await roleManager.CreateAsync(new IdentityRole(role));
                                    logger.LogInformation("Created role: {Role}", role);
                                }
                            }

                            logger.LogInformation("Database initialization completed successfully");
                            return;
                        }
                    }
                    catch (SqlException sqlEx) when (sqlEx.Number == 18456 || sqlEx.Number == 4060)
                    {
                       
                        logger.LogError(sqlEx, "Database authentication failed (Attempt {Attempt})", i + 1);
                        await Task.Delay(pauseBetweenFailures);
                    }
                    catch (SqlException sqlEx)
                    {
                        logger.LogError(sqlEx, "Database connection failed (Attempt {Attempt})", i + 1);
                        await Task.Delay(pauseBetweenFailures);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred while initializing the database (Attempt {Attempt})", i + 1);
                        if (i == maxRetryAttempts - 1) throw;
                        await Task.Delay(pauseBetweenFailures);
                    }
                }

                throw new Exception("Database initialization failed after maximum attempts");
            }
        }
    }
}

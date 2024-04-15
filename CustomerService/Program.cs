using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using CustomerService.Data;
using CustomerService.Services;
using CustomerService.Services.Contracts;
using CustomerService.Validators.EntityValidators;
using CustomerService.Data.Base;
using CustomerService.Models.ModelDto.Mapping;
using System.Reflection;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure services and JWT authentication
        ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();

        // Configure middleware
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer Service Campaign v1"));
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Database Context
        services.AddDbContext<CustomerServiceDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("CustomerServiceConnection")));

        // JWT Authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
     .AddJwtBearer(options =>
     {
         options.TokenValidationParameters = new TokenValidationParameters
         {
             ValidateIssuerSigningKey = true,
             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY"))),
             ValidateIssuer = true,
             ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
             ValidateAudience = true,
             ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
             ValidateLifetime = true,
             ClockSkew = TimeSpan.Zero
         };
     });

        // Authorization
        services.AddAuthorization();

        // Controllers
        services.AddControllers();

        //Mappers
        services.AddAutoMapper(typeof(AutoMapperProfile));

        // Repositories and Services
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IAgentService, AgentService>();
        services.AddScoped<ICampaignService, CampaignService>();
        services.AddScoped<IPurchaseService, PurchaseService>();
        services.AddScoped<ICustomerService, CustomerPersonService>();
        services.AddScoped<ICampaignReportService, CampaignReportService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();


        // Validators
        services.AddScoped<PurchaseValidator>();

        // HTTP Client
        services.AddHttpClient();

        // Swagger Configuration
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Customer Service Campaign", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
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
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });

            //API Comments
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });
    }
}

using CustomerService.Data;
using CustomerService.Data.Base;
using CustomerService.Services;
using CustomerService.Services.Contracts;
using CustomerService.Validators.EntityValidators;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        var connectionString = configuration.GetConnectionString("CustomerServiceConnection");
        services.AddDbContext<CustomerServiceDbContext>(options => options.UseSqlServer(connectionString));

        // Add authorization services
        services.AddAuthorization();

        // Add controllers
        services.AddControllers();

        // Register repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Register services
        services.AddScoped<IAgentService, AgentService>();
        services.AddScoped<ICampaignService, CampaignService>();
        services.AddScoped<IPurchaseService, PurchaseServise>();
        services.AddScoped<ICustomerService, CustomerPersonService>();
        services.AddScoped<ICampaignReportService, CampaignReportService>();

        // Register validators
        services.AddScoped<PurchaseValidator>();

        // Register HTTP client
        services.AddHttpClient();
        services.AddHttpClient<ICustomerService, CustomerPersonService>();

        // Add Swagger services
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Customer Service Campaign", Version = "v1" });
        });
    }

}

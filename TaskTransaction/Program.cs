using Microsoft.EntityFrameworkCore;
using TaskTransaction.Models.DbContext;
using TaskTransaction.Models.Transaction.Repository;
using TaskTransaction.Models.User.Repository;
using TaskTransaction.Services;

namespace TaskTransaction;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerGen();
        ConfigureDatabaseContext(builder);
        builder.Services.AddAutoMapper(typeof(Program));
        RegisterServices(builder);
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }

    private static void ConfigureDatabaseContext(WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("TransactionContext");
        builder.Services.AddDbContext<TransactionContext>(options =>
            options.UseSqlite(connectionString));
    }

    private static void RegisterServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<TransactionService>();
    }
}
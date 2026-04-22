using AgendamentoVacinacao.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace AgendamentoVacinacao.WebApi.Configuration;

public static class DbConfiguration
{
    public static IServiceCollection AddDbConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        
        services.AddDbContext<AgendamentoVacinacaoContext>(options =>
            options.UseSqlServer(connectionString!));

        return services;
    }
}
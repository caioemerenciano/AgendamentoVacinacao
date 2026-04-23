using AgendamentoVacinacao.Business.Interface;
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using AgendamentoVacinacao.Repository.Repositories;
using AgendamentoVacinacao.Business;


namespace AgendamentoVacinacao.WebApi.Configuration;

public static class DependencyInjectionConfiguration
{
    public static IServiceCollection AddDependencyInjectionConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IPacienteRepository, PacienteRepository>();
        services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();

 
        services.AddScoped<IPacienteBusiness, PacienteBusiness>();
        services.AddScoped<IAgendamentoBusiness, AgendamentoBusiness>();

        return services;
    }
}
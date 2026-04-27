using AgendamentoVacinacao.Business.Interface;
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using AgendamentoVacinacao.Repository.Repositories;
using AgendamentoVacinacao.Business;
using AgendamentoVacinacao.Business.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using AgendamentoVacinacao.Validator.Validators;


namespace AgendamentoVacinacao.WebApi.Configuration;

public static class DependencyInjectionConfiguration
{
    public static IServiceCollection AddDependencyInjectionConfiguration(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<AgendamentoRequestValidator>();

        services.AddScoped<IPacienteRepository, PacienteRepository>();

        services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
        services.AddScoped<IAgendamentoBusiness, AgendamentoBusiness>();

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IAuthBusiness, AuthBusiness>();

        return services;
    }
}
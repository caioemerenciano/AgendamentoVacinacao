using AgendamentoVacinacao.Entity.Entities;
using AgendamentoVacinacao.Entity.Enums;
using AgendamentoVacinacao.Repository.Interface.IRepositories;

namespace AgendamentoVacinacao.Api.Configuration;

public static class DataSeeder
{
    public static async Task SeedEnfermeiroAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUsuarioRepository>();

        const string emailEnfermeiro = "enfermeiro@gmail.com";
        var usuarioExistente = await repository.ObterPorEmailAsync(emailEnfermeiro);

        if (usuarioExistente == null)
        {
            string senhaHash = BCrypt.Net.BCrypt.HashPassword("enfermeiro");
            
            var enfermeiro = new Usuario(
                nome: "Enfermeiro Padrão",
                email: emailEnfermeiro,
                senhaHash: senhaHash,
                perfil: PerfilUsuario.Enfermeiro
            );

            await repository.AddAsync(enfermeiro);
            await repository.SaveChangesAsync();
        }
    }
}

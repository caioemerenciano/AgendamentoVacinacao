using AgendamentoVacinacao.Business.Interface;
using AgendamentoVacinacao.Repository.Interface;
using AgendamentoVacinacao.Entity.DTOs.Request;
using AgendamentoVacinacao.Entity.DTOs.Response;
using AgendamentoVacinacao.Entity.Entities;

namespace AgendamentoVacinacao.Business.Services;

public class PacienteBusiness : IPacienteBusiness
{
    private readonly IPacienteRepository _repository;

    public PacienteBusiness(IPacienteRepository repository)
    {
        _repository = repository;
    }

    public async Task<PacienteResponse> CriarPacienteAsync(CriarPacienteRequest request)
    {
        bool pacienteExiste = await _repository.ExistePacienteAsync(request.Nome, request.DataNascimento);
        if (pacienteExiste)
        {
            throw new InvalidOperationException("Já existe um paciente cadastrado com este nome e data de nascimento.");
        }

        var novoPaciente = new Paciente(
            nome: request.Nome,
            dataNascimento: request.DataNascimento
        );

        await _repository.AdicionarAsync(novoPaciente);
        await _repository.SalvarAlteracoesAsync();

        return new PacienteResponse(
            novoPaciente.Id,
            novoPaciente.Nome!,
            novoPaciente.DataNascimento);
    }
}
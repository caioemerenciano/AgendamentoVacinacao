using AgendamentoVacinacao.Business.Interface;
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using AgendamentoVacinacao.Entity.DTOs.Request;
using AgendamentoVacinacao.Entity.DTOs.Response;
using AgendamentoVacinacao.Entity.Entities;

namespace AgendamentoVacinacao.Business;

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

        await _repository.AddAsync(novoPaciente);
        await _repository.SaveChangesAsync();

        return new PacienteResponse(
            novoPaciente.Id,
            novoPaciente.Nome!,
            novoPaciente.DataNascimento);
    }

    public async Task<IEnumerable<PacienteResponse>> ObterTodosAsync()
    {
        
        var pacientes = await _repository.GetAllAsync();

        return pacientes.Select(p => new PacienteResponse(
            Id: p.Id,
            Nome: p.Nome!,
            DataNascimento: p.DataNascimento
        ));
    }
    public async Task<PacienteResponse?> ObterPorIdAsync(int id)
    {
        
        var paciente = await _repository.GetByIdAsync(id);

        if (paciente == null)
            return null;

        return new PacienteResponse(
            Id: paciente.Id,
            Nome: paciente.Nome!,
            DataNascimento: paciente.DataNascimento
        );
    }

}
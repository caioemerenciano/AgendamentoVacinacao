using AgendamentoVacinacao.Business.Interface;
using AgendamentoVacinacao.Entity.DTOs.Request;
using AgendamentoVacinacao.Entity.DTOs.Response;
using AgendamentoVacinacao.Entity.Entities;
using AgendamentoVacinacao.Entity.Enums;
using AgendamentoVacinacao.Repository.Interface.IRepositories;


namespace AgendamentoVacinacao.Business;

public class AgendamentoBusiness : IAgendamentoBusiness
{
    private readonly IAgendamentoRepository _repository;
    public AgendamentoBusiness(IAgendamentoRepository repository)
    {
        _repository = repository;
    }

    public async Task<AgendamentoResponse> CriarAgendamentoAsync(CriarAgendamentoRequest request)
    {
        int agendamentosNoDia = await _repository.ContarAgendamentosPorDiaAsync(request.DataAgendamento);
        if (agendamentosNoDia > 20)
        {
            throw new InvalidOperationException("A capacidade máxima de 20 agendamentos para este dia foi atingida.");
        }

        int agendamentosNoHorario = await _repository.ContarAgendamentosPorHorarioAsync(request.DataAgendamento ,request.HoraAgendamento);
        if (agendamentosNoHorario >= 2)
        {
            throw new InvalidOperationException("A capacidade máxima de 2 agendamentos para este horário foi atingida");
        }

        var novoAgendamento = new AgendamentoVacinacao.Entity.Entities.Agendamento(
            id: 0,
            idPaciente: request.IdPaciente,
            dataAgendamento: request.DataAgendamento,
            horaAgendamento: request.HoraAgendamento,
            status: StatusAgendamento.Agendado
        );

        await _repository.AdicionarAsync(novoAgendamento);
        await _repository.SalvarAlteracoesAsync();

        return new AgendamentoResponse(
            Id: novoAgendamento.Id,
            IdPaciente: novoAgendamento.IdPaciente,
            NomePaciente: "Nome omitido (Requer include de IPacienteRepository)",
            DataAgendamento: novoAgendamento.DataAgendamento,
            HoraAgendamento: novoAgendamento.HoraAgendamento,
            Status: novoAgendamento.Status
        );
    }
    public async Task<IEnumerable<AgendamentoResponse>> ObterTodosAsync()
    {
        var agendamentos = await _repository.ObterTodosAsync();

        return agendamentos.Select(a => new AgendamentoResponse(
            Id: a.Id,
            IdPaciente: a.IdPaciente,
            NomePaciente: a.Paciente!.Nome!,
            DataAgendamento: a.DataAgendamento,
            HoraAgendamento: a.HoraAgendamento,
            Status: a.Status
        ));
    }



    public Task<AgendamentoResponse?> ObterPorIdAsync(int id)
    {
        throw new NotImplementedException("Método de busca a ser implementado futuramente.");
    }

}

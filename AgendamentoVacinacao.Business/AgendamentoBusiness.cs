using AgendamentoVacinacao.Business.Interface;
using AgendamentoVacinacao.Entity.DTOs.Request;
using AgendamentoVacinacao.Entity.DTOs.Response;
using AgendamentoVacinacao.Entity.Enums;
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using AgendamentoVacinacao.Entity.Extensions;
using AgendamentoVacinacao.Entity.Entities;


namespace AgendamentoVacinacao.Business;

public class AgendamentoBusiness : IAgendamentoBusiness
{
    private readonly IAgendamentoRepository _repository;
    private readonly IPacienteRepository _pacienteRepository;
    public AgendamentoBusiness(IAgendamentoRepository repository, IPacienteRepository pacienteRepository)
    {
        _repository = repository;
        _pacienteRepository = pacienteRepository;
    }

    public async Task<AgendamentoResponse> CriarAgendamentoAsync(CriarAgendamentoRequest request, int usuarioId)
    {
        var dataAgendamentoParsed = request.DataAgendamento;
        var dataNascimentoParsed = request.DataNascimento;
        var horaAgendamentoParsed = request.HoraAgendamento;

        int agendamentosNoDia = await _repository.ContarAgendamentosPorDiaAsync(dataAgendamentoParsed);
        if (agendamentosNoDia >= 20)
        {
            throw new InvalidOperationException("A capacidade máxima de 20 agendamentos para este dia foi atingida.");
        }

        int agendamentosNoHorario = await _repository.ContarAgendamentosPorHorarioAsync(dataAgendamentoParsed, horaAgendamentoParsed);
        if (agendamentosNoHorario >= 2)
        {
            throw new InvalidOperationException("A capacidade máxima de 2 agendamentos simultâneos ou com intervalo menor que 1 hora foi atingida para este slot.");
        }

        var paciente = await _pacienteRepository.GetByIdAsync(usuarioId);

        if (paciente == null)
        {
            paciente = new Paciente(request.Nome, dataNascimentoParsed);
            // Sincroniza o ID do paciente com o ID do usuário
            paciente.Id = usuarioId;
            await _pacienteRepository.AdicionarComIdForcadoAsync(paciente);
        }

        var novoAgendamento = new Agendamento(
            id: 0,
            idPaciente: paciente.Id,
            dataAgendamento: dataAgendamentoParsed,
            horaAgendamento: horaAgendamentoParsed,
            status: StatusAgendamento.Agendado
        );

        await _repository.AddAsync(novoAgendamento);
        await _repository.SaveChangesAsync();

        return new AgendamentoResponse(
            Id: novoAgendamento.Id,
            IdPaciente: novoAgendamento.IdPaciente,
            NomePaciente: paciente.Nome ?? string.Empty,
            DataAgendamento: novoAgendamento.DataAgendamento,
            HoraAgendamento: novoAgendamento.HoraAgendamento,
            Status: novoAgendamento.Status
        );
    }
    public async Task<IEnumerable<AgendamentoResponse>> ObterTodosAsync(int usuarioId, string perfil)
    {
        var agendamentos = await _repository.GetAllAsync();

        if (perfil.Equals("Paciente", StringComparison.OrdinalIgnoreCase))
        {
            agendamentos = agendamentos.Where(a => a.IdPaciente == usuarioId);
        }

        return agendamentos.Select(a => new AgendamentoResponse(
            Id: a.Id,
            IdPaciente: a.IdPaciente,
            NomePaciente: a.Paciente!.Nome!,
            DataAgendamento: a.DataAgendamento,
            HoraAgendamento: a.HoraAgendamento,
            Status: a.Status
        ));
    }
    public async Task<AgendamentoResponse> AtualizarAgendamentoAsync(int id, AtualizarAgendamentoRequest request, int usuarioId)
    {
        var agendamento = await _repository.GetByIdAsync(id);

        if (agendamento == null)
        {
            throw new InvalidOperationException("Agendamento não encontrado.");
        }

        if (agendamento.IdPaciente != usuarioId)
        {
            throw new UnauthorizedAccessException("Você não tem permissão para alterar este agendamento.");
        }

        if (agendamento.Status != StatusAgendamento.Agendado)
        {
            throw new InvalidOperationException("Apenas agendamentos com status 'Agendado' podem ser alterados.");
        }

        var dataHoraOriginal = agendamento.DataAgendamento.Date.Add(agendamento.HoraAgendamento);
        var limiteParaAlteracao = dataHoraOriginal.AddHours(-2);

        if (DateTime.Now > limiteParaAlteracao)
        {
            throw new InvalidOperationException("Alteração só podem ser feitas com no mínimo 2 horas de antecedência do horário agendado.");
        }

        bool mudouDataOuHora = agendamento.DataAgendamento != request.DataAgendamento ||
                               agendamento.HoraAgendamento != request.HoraAgendamento;

        if (mudouDataOuHora)
        {
            var horarioOcupado = await _repository.ExisteAgendamentoConflitanteAsync(request.DataAgendamento, request.HoraAgendamento, id);
            if (horarioOcupado)
            {
                throw new InvalidOperationException($"Já existe um paciente agendado para o dia {request.DataAgendamento:dd/MM/yyyy} às {request.HoraAgendamento:hh\\:mm}."); ;
            }

            if (agendamento.DataAgendamento != request.DataAgendamento)
            {
                const int LIMITE_VAGAS_POR_DIA = 20;
                var totalNoDia = await _repository.ContarAgendamentosPorDiaAsync(request.DataAgendamento);

                if (totalNoDia >= LIMITE_VAGAS_POR_DIA)
                {
                    throw new InvalidOperationException($"O limite máximo de {LIMITE_VAGAS_POR_DIA} vagas para o dia {request.DataAgendamento:dd/MM/yyyy} já foi atingido");
                }
            }
        }

        agendamento.DataAgendamento = request.DataAgendamento;
        agendamento.HoraAgendamento = request.HoraAgendamento;

        _repository.Update(agendamento);
        await _repository.SaveChangesAsync();
        
        return new AgendamentoResponse(
            Id: agendamento.Id,
            IdPaciente: agendamento.IdPaciente,
            NomePaciente: agendamento.Paciente?.Nome ?? string.Empty,
            DataAgendamento: agendamento.DataAgendamento,
            HoraAgendamento: agendamento.HoraAgendamento,
            Status: agendamento.Status
        );
    }


    public async Task<AgendamentoResponse?> ObterPorIdAsync(int id)
    {
        var agendamento = await _repository.GetByIdAsync(id);

        if (agendamento == null)
            return null!;

        return new AgendamentoResponse(
            Id: agendamento.Id,
            IdPaciente: agendamento.IdPaciente,
            NomePaciente: agendamento.Paciente?.Nome ?? string.Empty, 
            DataAgendamento: agendamento.DataAgendamento,
            HoraAgendamento: agendamento.HoraAgendamento,
            Status: agendamento.Status
        );
    }
    public async Task CancelarAgendamentoAsync(int id, int usuarioId, string perfil)
    {
        var agendamento = await _repository.GetByIdAsync(id);

        if (agendamento == null)
        {
            throw new InvalidOperationException("Agendamento não encontrado.");
        }

        bool Dono = agendamento.IdPaciente == usuarioId;
        bool Enfermeiro = perfil.Equals("Enfermeiro", StringComparison.OrdinalIgnoreCase);

        if (!Dono && !Enfermeiro)
        {
            throw new UnauthorizedAccessException("Você não tem permissão para cancelar este agendamento.");
        }

        if (agendamento.Status == StatusAgendamento.Cancelado)
        {
            throw new InvalidOperationException("Este agendamento já encontra-se cancelado.");
        }

        var dataHoraOriginal = agendamento.DataAgendamento.Date.Add(agendamento.HoraAgendamento);
        if (dataHoraOriginal < DateTime.Now)
        {
            throw new InvalidOperationException("Não é possível cancelar um agendamento cuja data/hora já passou.");
        }

        agendamento.Cancelar();

        _repository.Update(agendamento);
        await _repository.SaveChangesAsync();
    }

    public async Task AtualizarStatusAsync(int id, StatusAgendamento novoStatus)
    {
        var agendamento = await _repository.GetByIdAsync(id);

        if (agendamento == null)
        {
            throw new InvalidOperationException("Agendamento não encontrado.");
        }

        agendamento.Status = novoStatus;

        _repository.Update(agendamento);
        await _repository.SaveChangesAsync();
    }
}

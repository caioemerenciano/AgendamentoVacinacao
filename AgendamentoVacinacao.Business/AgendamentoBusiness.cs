using AgendamentoVacinacao.Business.Interface;
using AgendamentoVacinacao.Entity.DTOs.Request;
using AgendamentoVacinacao.Entity.DTOs.Response;
using AgendamentoVacinacao.Entity.Enums;
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using AgendamentoVacinacao.Entity.Extensions;


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

    public async Task<AgendamentoResponse> CriarAgendamentoAsync(CriarAgendamentoRequest request)
    {
        string[] formats = { "dd/MM/yyyy", "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ssZ", "yyyy-MM-ddTHH:mm:ss" };

        var dataAgendamentoParsed = DateTime.ParseExact(request.DataAgendamento, formats, null, System.Globalization.DateTimeStyles.None);
        var dataNascimentoParsed = DateTime.ParseExact(request.DataNascimento, formats, null, System.Globalization.DateTimeStyles.None);

        var horaAgendamentoParsed = TimeSpan.Parse(request.Horario);

        int agendamentosNoDia = await _repository.ContarAgendamentosPorDiaAsync(dataAgendamentoParsed);
        if (agendamentosNoDia > 20)
        {
            throw new InvalidOperationException("A capacidade máxima de 20 agendamentos para este dia foi atingida.");
        }

        int agendamentosNoHorario = await _repository.ContarAgendamentosPorHorarioAsync(dataAgendamentoParsed, horaAgendamentoParsed);
        if (agendamentosNoHorario >= 2)
        {
            throw new InvalidOperationException("A capacidade máxima de 2 agendamentos simultâneos ou com intervalo menor que 1 hora foi atingida para este slot.");
        }

        var paciente = await _pacienteRepository.ObterPorNomeEDataNascimentoAsync(request.Nome, dataNascimentoParsed);

        if (paciente == null)
        {
            paciente = new AgendamentoVacinacao.Entity.Entities.Paciente(request.Nome, dataNascimentoParsed);
            await _pacienteRepository.AdicionarAsync(paciente);
            await _pacienteRepository.SalvarAlteracoesAsync();
        }

        var novoAgendamento = new AgendamentoVacinacao.Entity.Entities.Agendamento(
            id: 0,
            idPaciente: paciente.Id,
            dataAgendamento: dataAgendamentoParsed,
            horaAgendamento: horaAgendamentoParsed,
            status: StatusAgendamento.Agendado
        );

        await _repository.AdicionarAsync(novoAgendamento);
        await _repository.SalvarAlteracoesAsync();

        return new AgendamentoResponse(
            Id: novoAgendamento.Id,
            IdPaciente: novoAgendamento.IdPaciente,
            NomePaciente: paciente.Nome ?? string.Empty,
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
    public async Task<AgendamentoResponse> AtualizarAgendamentoAsync(int id, AtualizarAgendamentoRequest request)
    {
        var agendamento = await _repository.ObterPorIdAsync(id);

        if (agendamento == null)
        {
            throw new InvalidOperationException("Agendamento não encontrado.");
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

        await _repository.AtualizarAsync(agendamento);
        
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
        var agendamento = await _repository.ObterPorIdAsync(id);

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
    public async Task CancelarAgendamentoAsync(int id)
    {
        var agendamento = await _repository.ObterPorIdAsync(id);

        if (agendamento == null)
        {
            throw new InvalidOperationException("Agendamento não encontrado.");
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

        await _repository.AtualizarAsync(agendamento);
    }

    public async Task AtualizarStatusAsync(int id, StatusAgendamento novoStatus)
    {
        var agendamento = await _repository.ObterPorIdAsync(id);

        if (agendamento == null)
        {
            throw new InvalidOperationException("Agendamento não encontrado.");
        }

        agendamento.Status = novoStatus;

        await _repository.AtualizarAsync(agendamento);
    }
}

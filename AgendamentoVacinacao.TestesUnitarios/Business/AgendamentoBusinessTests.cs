using AgendamentoVacinacao.Business;
using AgendamentoVacinacao.Entity.DTOs.Request;
using AgendamentoVacinacao.Entity.Entities;
using AgendamentoVacinacao.Entity.Enums;
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using Moq;
using Xunit;

namespace AgendamentoVacinacao.Tests.Business;

public class AgendamentoBusinessTests
{
    private readonly Mock<IAgendamentoRepository> _agendamentoRepositoryMock;
    private readonly Mock<IPacienteRepository> _pacienteRepositoryMock;
    private readonly AgendamentoBusiness _agendamentoBusiness;

    public AgendamentoBusinessTests()
    {
        _agendamentoRepositoryMock = new Mock<IAgendamentoRepository>();
        _pacienteRepositoryMock = new Mock<IPacienteRepository>();

        _agendamentoBusiness = new AgendamentoBusiness(_agendamentoRepositoryMock.Object, _pacienteRepositoryMock.Object);
    }

    //Testes de método: CancelarAgendamentosAsync (PATCH)

    [Fact]
    public async Task CancelarAgendamentosAsync_QuandoAgendamentoNaoExiste_DeveLancarExcecao()
    {
        int idInvalido = 9999;
        _agendamentoRepositoryMock.Setup(repo => repo.GetByIdAsync(idInvalido))
            .ReturnsAsync((Agendamento)null!);


        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _agendamentoBusiness.CancelarAgendamentoAsync(idInvalido, 1, "Paciente"));

        Assert.Equal("Agendamento não encontrado.", excecao.Message);
    }

    [Fact]
    public async Task CancelarAgendamentoAsync_QuandoJaEstaCancelado_DeveLancarExcecao()
    {
        int idValido = 1;
        var agendamentoSimulado = new Agendamento
        {
            IdPaciente = 1,
            Status = StatusAgendamento.Cancelado
        };

        _agendamentoRepositoryMock.Setup(repo => repo.GetByIdAsync(idValido))
            .ReturnsAsync(agendamentoSimulado);


        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _agendamentoBusiness.CancelarAgendamentoAsync(idValido, 1, "Paciente"));

        Assert.Equal("Este agendamento já encontra-se cancelado.", excecao.Message);
    }

    [Fact]
    public async Task CancelarAgendamentoAsync_QuandoValido_DeveAtualizarStatusNoRepositorio()
    {
        int idValido = 1;
        var agendamentoValido = new Agendamento
        {
            IdPaciente = 1,
            DataAgendamento = DateTime.Now.AddDays(1),
            HoraAgendamento = new TimeSpan(14, 0, 0),
            Status = StatusAgendamento.Agendado
        };

        _agendamentoRepositoryMock.Setup(repo => repo.GetByIdAsync(idValido))
            .ReturnsAsync(agendamentoValido);

        await _agendamentoBusiness.CancelarAgendamentoAsync(idValido, 1, "Paciente");

        Assert.Equal(StatusAgendamento.Cancelado, agendamentoValido.Status);

        _agendamentoRepositoryMock.Verify(repo => repo.Update(agendamentoValido), Times.Once);
        _agendamentoRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }


    // Testes de método: AtualizarAgendamentosAsync (PUT)

    [Fact]
    public async Task AtualizarAgendamentoAsync_QuandoFaltaMenosDeDuasHoras_DeveLancarExcecao()
    {
        int id = 1;
        var request = new AtualizarAgendamentoRequest
        {
            DataAgendamento = DateTime.Now.AddDays(2),
            HoraAgendamento = new TimeSpan(10, 0, 0)
        };

        var agendamentoExistente = new Agendamento
        {
            Id = id,
            IdPaciente = 1,
            DataAgendamento = DateTime.Now.Date,
            HoraAgendamento = DateTime.Now.AddHours(1).TimeOfDay,
            Status = StatusAgendamento.Agendado
        };

        _agendamentoRepositoryMock.Setup(repo => repo.GetByIdAsync(id))
            .ReturnsAsync(agendamentoExistente);

        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _agendamentoBusiness.AtualizarAgendamentoAsync(id, request, 1));

        Assert.Contains("2 horas de antecedência", excecao.Message);
    }

    [Fact]
    public async Task AtualizarAgendamentoAsync_QuandoHorarioEstaOcupado_DeveLancarExcecao()
    {
        int id = 1;
        var dataFutura = DateTime.Now.AddDays(3).Date;
        var horaFutura = new TimeSpan(14, 0, 0);

        var request = new AtualizarAgendamentoRequest
        {
            DataAgendamento = dataFutura,
            HoraAgendamento = horaFutura
        };

        var agendamentoExistente = new Agendamento
        {
            Id = id,
            IdPaciente = 1,
            DataAgendamento = DateTime.Now.AddDays(5).Date,
            HoraAgendamento = new TimeSpan(10, 0, 0),
            Status = StatusAgendamento.Agendado
        };

        _agendamentoRepositoryMock.Setup(repo => repo.GetByIdAsync(id))
            .ReturnsAsync(agendamentoExistente);

        _agendamentoRepositoryMock.Setup(repo => repo.ExisteAgendamentoConflitanteAsync(dataFutura, horaFutura, id))
            .ReturnsAsync(true);


        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _agendamentoBusiness.AtualizarAgendamentoAsync(id, request, 1));

        Assert.Contains("Já existe um paciente agendado", excecao.Message);
    }

    [Fact]
    public async Task AtualizarAgendamentoAsync_QuandoAtingeLimiteDiario_DeveLancarExcecao()
    {
        int id = 1;
        var dataFutura = DateTime.Now.AddDays(3).Date;
        var horaFutura = new TimeSpan(14, 0, 0);

        var request = new AtualizarAgendamentoRequest
        {
            DataAgendamento = dataFutura,
            HoraAgendamento = horaFutura
        };

        var agendamentoExistente = new Agendamento
        {
            Id = id,
            IdPaciente = 1,
            DataAgendamento = DateTime.Now.AddDays(5).Date,
            HoraAgendamento = new TimeSpan(10, 0, 0),
            Status = StatusAgendamento.Agendado
        };

        _agendamentoRepositoryMock.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(agendamentoExistente);

        _agendamentoRepositoryMock.Setup(repo => repo.ExisteAgendamentoConflitanteAsync(dataFutura, horaFutura, id)).ReturnsAsync(false);

        _agendamentoRepositoryMock.Setup(repo => repo.ContarAgendamentosPorDiaAsync(dataFutura)).ReturnsAsync(20);

        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _agendamentoBusiness.AtualizarAgendamentoAsync(id, request, 1));

        Assert.Contains("O limite máximo de 20 vagas", excecao.Message);
    }

    // Testes de método: CriarAgendamentoAsync (POST)

    [Fact]
    public async Task CriarAgendamentoAsync_ComDadosValidos_DeveSalvarNoRepositorio()
    {
        var dataAgendamento = DateTime.Now.AddDays(1).Date;
        var horaAgendamento = new TimeSpan(14, 0, 0);
        var dataNascimento = new DateTime(1990,5,15);
        var request = new CriarAgendamentoRequest(
            "João Silva",
            dataNascimento,
            dataAgendamento,
            horaAgendamento
        );

        _agendamentoRepositoryMock.Setup(repo => repo.ContarAgendamentosPorDiaAsync(dataAgendamento))
            .ReturnsAsync(0);
        _agendamentoRepositoryMock.Setup(repo => repo.ContarAgendamentosPorHorarioAsync(dataAgendamento, It.IsAny<TimeSpan>()))
            .ReturnsAsync(0);
        
        _pacienteRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(new Paciente { Id = 1, Nome = "João Silva" });

        var resultado = await _agendamentoBusiness.CriarAgendamentoAsync(request, 1);

        Assert.NotNull(resultado);
        _agendamentoRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Agendamento>()), Times.Once);
        _agendamentoRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CriarAgendamentoAsync_QuandoHoraEstaOcupadaComIntervaloMenorQueUmaHora_DeveLancarExcecao()
    {
        var dataAgendamento = DateTime.Now.AddDays(1).Date;
        var horaAgendamento = new TimeSpan(14, 30, 0);
        var dataNascimento = new DateTime(1990, 1, 1);
        var request = new CriarAgendamentoRequest(
            "Carlos Silva",
            dataNascimento,
            dataAgendamento,
            horaAgendamento
        );

        _agendamentoRepositoryMock.Setup(repo => repo.ContarAgendamentosPorDiaAsync(dataAgendamento))
            .ReturnsAsync(5); 
        _agendamentoRepositoryMock.Setup(repo => repo.ContarAgendamentosPorHorarioAsync(dataAgendamento, It.IsAny<TimeSpan>()))
            .ReturnsAsync(2); 
        
        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _agendamentoBusiness.CriarAgendamentoAsync(request, 1));

        Assert.Contains("capacidade máxima", excecao.Message);
        Assert.Contains("intervalo menor que 1 hora", excecao.Message);
    }

    [Fact]
    public async Task ObterTodosAsync_DeveRetornarListaDeAgendamentos()
    {
        var listaSimulada = new List<Agendamento>
        {
            new Agendamento { Id = 1, DataAgendamento = DateTime.Now, Paciente = new Paciente { Nome = "Maria" } },
            new Agendamento { Id = 2, DataAgendamento = DateTime.Now.AddDays(1), Paciente = new Paciente { Nome = "João" } }
        };

        _agendamentoRepositoryMock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(listaSimulada);

        var resultado = await _agendamentoBusiness.ObterTodosAsync(1, "Enfermeiro");

        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count());
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoExiste_DeveRetornarAgendamentoMapeado()
    {
        int idExistente = 1;
        var agendamentoSimulado = new Agendamento
        {
            Id = idExistente,
            DataAgendamento = DateTime.Now,
            Paciente = new Paciente { Nome = "João" }
        };

        _agendamentoRepositoryMock.Setup(repo => repo.GetByIdAsync(idExistente))
            .ReturnsAsync(agendamentoSimulado);

        var resultado = await _agendamentoBusiness.ObterPorIdAsync(idExistente);

        Assert.NotNull(resultado);
        Assert.Equal(idExistente, resultado.Id);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoNaoExiste_DeveRetornarNulo()
    {
        int idInexistente = 99;
        _agendamentoRepositoryMock.Setup(repo => repo.GetByIdAsync(idInexistente))
            .ReturnsAsync((Agendamento?)null);

        var resultado = await _agendamentoBusiness.ObterPorIdAsync(idInexistente);

        Assert.Null(resultado);
    }
}
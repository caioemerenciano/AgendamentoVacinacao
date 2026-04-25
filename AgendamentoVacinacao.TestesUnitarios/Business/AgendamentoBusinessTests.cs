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
        _agendamentoRepositoryMock.Setup(repo => repo.ObterPorIdAsync(idInvalido))
            .ReturnsAsync((Agendamento?)null);


        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _agendamentoBusiness.CancelarAgendamentoAsync(idInvalido));

        Assert.Equal("Agendamento não encontrado.", excecao.Message);
    }

    [Fact]
    public async Task CancelarAgendamentoAsync_QuandoJaEstaCancelado_DeveLancarExcecao()
    {

        int idValido = 1;
        var agendamentoCancelado = new Agendamento
        {
            Status = StatusAgendamento.Cancelado
        };

        _agendamentoRepositoryMock.Setup(repo => repo.ObterPorIdAsync(idValido))
            .ReturnsAsync(agendamentoCancelado);


        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _agendamentoBusiness.CancelarAgendamentoAsync(idValido));

        Assert.Equal("Este agendamento já encontra-se cancelado.", excecao.Message);
    }

    [Fact]
    public async Task CancelarAgendamentoAsync_QuandoValido_DeveAtualizarStatusNoRepositorio()
    {
        int idValido = 1;
        var agendamentoValido = new Agendamento
        {
            DataAgendamento = DateTime.Now.AddDays(1),
            HoraAgendamento = new TimeSpan(14, 0, 0),
            Status = StatusAgendamento.Agendado
        };

        _agendamentoRepositoryMock.Setup(repo => repo.ObterPorIdAsync(idValido))
            .ReturnsAsync(agendamentoValido);

        await _agendamentoBusiness.CancelarAgendamentoAsync(idValido);

        Assert.Equal(StatusAgendamento.Cancelado, agendamentoValido.Status);

        _agendamentoRepositoryMock.Verify(repo => repo.AtualizarAsync(agendamentoValido), Times.Once);
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
            DataAgendamento = DateTime.Now.Date,
            HoraAgendamento = DateTime.Now.AddHours(1).TimeOfDay
        };

        _agendamentoRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id))
            .ReturnsAsync(agendamentoExistente);

        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _agendamentoBusiness.AtualizarAgendamentoAsync(id, request));

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
            DataAgendamento = DateTime.Now.AddDays(5).Date,
            HoraAgendamento = new TimeSpan(10, 0, 0)
        };

        _agendamentoRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id))
            .ReturnsAsync(agendamentoExistente);

        _agendamentoRepositoryMock.Setup(repo => repo.ExisteAgendamentoConflitanteAsync(dataFutura, horaFutura, id))
            .ReturnsAsync(true);


        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _agendamentoBusiness.AtualizarAgendamentoAsync(id, request));

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
            DataAgendamento = DateTime.Now.AddDays(5).Date,
            HoraAgendamento = new TimeSpan(10, 0, 0)
        };

        _agendamentoRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(agendamentoExistente);

        _agendamentoRepositoryMock.Setup(repo => repo.ExisteAgendamentoConflitanteAsync(dataFutura, horaFutura, id)).ReturnsAsync(false);

        _agendamentoRepositoryMock.Setup(repo => repo.ContarAgendamentosPorDiaAsync(dataFutura)).ReturnsAsync(20);

        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _agendamentoBusiness.AtualizarAgendamentoAsync(id, request));

        Assert.Contains("O limite máximo de 20 vagas", excecao.Message);
    }

    // Testes de método: CriarAgendamentoAsync (POST)

    [Fact]
    public async Task CriarAgendamentoAsync_ComDadosValidos_DeveSalvarNoRepositorio()
    {
        var dataAgendamento = DateTime.Now.AddDays(1).Date;
        var request = new CriarAgendamentoRequest(
            "João Silva",
            "15/05/1990",
            dataAgendamento.ToString("dd/MM/yyyy"),
            "10:00"
        );

        _agendamentoRepositoryMock.Setup(repo => repo.ContarAgendamentosPorDiaAsync(dataAgendamento))
            .ReturnsAsync(0);
        _agendamentoRepositoryMock.Setup(repo => repo.ContarAgendamentosPorHorarioAsync(dataAgendamento, It.IsAny<TimeSpan>()))
            .ReturnsAsync(0);
        
        _pacienteRepositoryMock.Setup(repo => repo.ObterPorNomeEDataNascimentoAsync(request.Nome, It.IsAny<DateTime>()))
            .ReturnsAsync(new Paciente { Id = 1, Nome = "João Silva" });

        var resultado = await _agendamentoBusiness.CriarAgendamentoAsync(request);

        Assert.NotNull(resultado);
        _agendamentoRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Agendamento>()), Times.Once);
    }

    [Fact]
    public async Task CriarAgendamentoAsync_ComDadosValidos_ISOFormat_DeveSalvarNoRepositorio()
    {
        var dataAgendamento = DateTime.Now.AddDays(1).Date;
        var request = new CriarAgendamentoRequest(
            "Maria Oliveira",
            "1985-08-20",
            dataAgendamento.ToString("yyyy-MM-dd"),
            "14:00"
        );

        _agendamentoRepositoryMock.Setup(repo => repo.ContarAgendamentosPorDiaAsync(dataAgendamento))
            .ReturnsAsync(0);
        _agendamentoRepositoryMock.Setup(repo => repo.ContarAgendamentosPorHorarioAsync(dataAgendamento, It.IsAny<TimeSpan>()))
            .ReturnsAsync(0);
        
        _pacienteRepositoryMock.Setup(repo => repo.ObterPorNomeEDataNascimentoAsync(request.Nome, It.IsAny<DateTime>()))
            .ReturnsAsync(new Paciente { Id = 2, Nome = "Maria Oliveira" });

        var resultado = await _agendamentoBusiness.CriarAgendamentoAsync(request);

        Assert.NotNull(resultado);
        _agendamentoRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Agendamento>()), Times.Once);
    }

    [Fact]
    public async Task CriarAgendamentoAsync_QuandoHoraEstaOcupadaComIntervaloMenorQueUmaHora_DeveLancarExcecao()
    {
        var dataAgendamento = DateTime.Now.AddDays(1).Date;
        var request = new CriarAgendamentoRequest(
            "Carlos Silva",
            "1990-01-01",
            dataAgendamento.ToString("yyyy-MM-dd"),
            "14:30" // Tentando agendar 14:30, mas pode estar muito perto de outro
        );

        _agendamentoRepositoryMock.Setup(repo => repo.ContarAgendamentosPorDiaAsync(dataAgendamento))
            .ReturnsAsync(5); // Um número aceitável pro dia
        _agendamentoRepositoryMock.Setup(repo => repo.ContarAgendamentosPorHorarioAsync(dataAgendamento, It.IsAny<TimeSpan>()))
            .ReturnsAsync(2); // Retorna que o overlap é 2 (atingiu a margem de segurança de intervalo simultâneo)
        
        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _agendamentoBusiness.CriarAgendamentoAsync(request));

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

        _agendamentoRepositoryMock.Setup(repo => repo.ObterTodosAsync())
            .ReturnsAsync(listaSimulada);

        var resultado = await _agendamentoBusiness.ObterTodosAsync();

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

        _agendamentoRepositoryMock.Setup(repo => repo.ObterPorIdAsync(idExistente))
            .ReturnsAsync(agendamentoSimulado);

        var resultado = await _agendamentoBusiness.ObterPorIdAsync(idExistente);

        Assert.NotNull(resultado);
        Assert.Equal(idExistente, resultado.Id);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoNaoExiste_DeveRetornarNulo()
    {
        int idInexistente = 99;
        _agendamentoRepositoryMock.Setup(repo => repo.ObterPorIdAsync(idInexistente))
            .ReturnsAsync((Agendamento?)null);

        var resultado = await _agendamentoBusiness.ObterPorIdAsync(idInexistente);

        Assert.Null(resultado);
    }

    // Teste de método: ObterPorIdAsync (GET {id})

    [Fact]
    public async Task ObterPorIdAsync_QuandoOAgendamentoExiste_DeveRetornarResponseMapeado()
    {
        int idExistente = 10;
        var agendamentoNoBanco = new Agendamento
        {
            Id = idExistente,
            DataAgendamento = DateTime.Now.AddDays(5),
            HoraAgendamento = new TimeSpan(14, 30, 0),
            Paciente = new Paciente { Nome = "Usuário Teste" } 
        };

        _agendamentoRepositoryMock.Setup(repo => repo.ObterPorIdAsync(idExistente))
            .ReturnsAsync(agendamentoNoBanco);

        var resultado = await _agendamentoBusiness.ObterPorIdAsync(idExistente);

        Assert.NotNull(resultado);
        Assert.Equal(idExistente, resultado.Id);
        Assert.Equal("Usuário Teste", resultado.NomePaciente);

        _agendamentoRepositoryMock.Verify(repo => repo.ObterPorIdAsync(idExistente), Times.Once);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoOAgendamentoNaoExiste_DeveRetornarNulo()
    {
        int idInexistente = 999;

        _agendamentoRepositoryMock.Setup(repo => repo.ObterPorIdAsync(idInexistente))
            .ReturnsAsync((Agendamento?)null);

        var resultado = await _agendamentoBusiness.ObterPorIdAsync(idInexistente);

        Assert.Null(resultado);
        _agendamentoRepositoryMock.Verify(repo => repo.ObterPorIdAsync(idInexistente), Times.Once);
    }
}
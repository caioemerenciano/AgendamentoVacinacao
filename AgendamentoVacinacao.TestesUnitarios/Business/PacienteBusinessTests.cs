using AgendamentoVacinacao.Business; 
using AgendamentoVacinacao.Entity.DTOs.Request; 
using AgendamentoVacinacao.Entity.Entities;
using AgendamentoVacinacao.Repository.Interface.IRepositories; 
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AgendamentoVacinacao.Tests.Business;

public class PacienteBusinessTests
{
    private readonly Mock<IPacienteRepository> _pacienteRepositoryMock;
    private readonly PacienteBusiness _pacienteBusiness;

    public PacienteBusinessTests()
    {
        _pacienteRepositoryMock = new Mock<IPacienteRepository>();
        _pacienteBusiness = new PacienteBusiness(_pacienteRepositoryMock.Object);
    }

    //Testes de método: CriarPacienteAsync (POST)

    [Fact]
    public async Task CriarPacienteAsync_ComDadosValidos_DeveSalvarNoRepositorio()
    {
        var request = new CriarPacienteRequest(
            "Maria Silva",
            new DateTime(1990, 5, 20) 
        );

        var resultado = await _pacienteBusiness.CriarPacienteAsync(request);

        Assert.NotNull(resultado);

        _pacienteRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Paciente>()), Times.Once);
        _pacienteRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    //Testes de método: ObterTodosAsync (GET)

    [Fact]
    public async Task ObterTodosAsync_DeveRetornarListaDePacientesMapeados()
    {
        var listaSimulada = new List<Paciente>
        {
            new Paciente { Id = 1, Nome = "Carlos", DataNascimento = new DateTime(1985, 10, 10) },
            new Paciente { Id = 2, Nome = "Ana", DataNascimento = new DateTime(1995, 2, 15) }
        };

        _pacienteRepositoryMock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(listaSimulada);

        var resultado = await _pacienteBusiness.ObterTodosAsync();

        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count());

        Assert.Equal("Carlos", resultado.First().Nome);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoPacienteExiste_DeveRetornarResponseMapeado()
    {
        int idExistente = 5;
        var pacienteSimulado = new Paciente
        {
            Id = idExistente,
            Nome = "Lucas Mendes",
            DataNascimento = new DateTime(2000, 1, 1)
        };

        _pacienteRepositoryMock.Setup(repo => repo.GetByIdAsync(idExistente))
            .ReturnsAsync(pacienteSimulado);

        var resultado = await _pacienteBusiness.ObterPorIdAsync(idExistente);

        Assert.NotNull(resultado);
        Assert.Equal(idExistente, resultado.Id);
        Assert.Equal("Lucas Mendes", resultado.Nome);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoPacienteNaoExiste_DeveRetornarNulo()
    {
        int idInexistente = 9999;

        _pacienteRepositoryMock.Setup(repo => repo.GetByIdAsync(idInexistente))
            .ReturnsAsync((Paciente?)null);

        var resultado = await _pacienteBusiness.ObterPorIdAsync(idInexistente);

        Assert.Null(resultado);
    }
}

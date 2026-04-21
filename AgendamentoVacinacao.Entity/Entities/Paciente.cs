namespace AgendamentoVacinacao.Entity.Entities;

public class Paciente
{
    protected Paciente() { }

    public Paciente(Guid id, string nome, DateOnly dataNascimento)
    {
        Nome = nome;
        DataNascimento = dataNascimento;
        DataCriacao = DateTime.Now;
    }

    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public DateTime DataNascimento { get; private set; }
    public DateTime DataCriacao { get; private set; }

    public virtual ICollection<Agendamento> Agendamentos { get; private set; } = new List<Agendamento>();
}

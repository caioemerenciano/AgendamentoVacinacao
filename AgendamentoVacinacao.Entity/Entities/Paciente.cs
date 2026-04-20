namespace AgendamentoVacinacao.Entity.Entities;

public class Paciente
{
    protected Paciente() { }

    public Paciente(Guid id, string nome, DateOnly dataNascimento)
    {
        Id = id;
        Nome = nome;
        DataNascimento = dataNascimento;
    }

    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public DateOnly DataNascimento { get; private set; }

    public virtual ICollection<Agendamento> Agendamentos { get; private set; } = new List<Agendamento>();
}

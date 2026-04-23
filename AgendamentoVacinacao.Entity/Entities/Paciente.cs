namespace AgendamentoVacinacao.Entity.Entities;

public class Paciente
{
    public Paciente() { }

    public Paciente(string nome, DateTime dataNascimento)
    {
        Nome = nome;
        DataNascimento = dataNascimento;
        DataCriacao = DateTime.Now;
    }

    public int Id { get; set; }
    public string? Nome { get; set; }
    public DateTime DataNascimento { get; set; }
    public DateTime DataCriacao { get; set; }

    public virtual ICollection<Agendamento> Agendamentos { get; private set; } = new List<Agendamento>();
}

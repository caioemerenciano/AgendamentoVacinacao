using AgendamentoVacinacao.Entity.DTOs.Request;
using FluentValidation;

namespace AgendamentoVacinacao.Validator.Validators;

public class AgendamentoRequestValidator : AbstractValidator<CriarAgendamentoRequest>
{
    public AgendamentoRequestValidator()
    {
        RuleFor(x => x.IdPaciente)
            .GreaterThan(0).WithMessage("Id do paciente inválido.");

        RuleFor(x => x.DataAgendamento)
            .NotEmpty().WithMessage("A data do agendamento é obrigatória.")
            .Must(d => d.Date >= DateTime.Now.Date).WithMessage("Não é possível agendar para uma data passada.");

        RuleFor(x => x.HoraAgendamento)
            .Must(h => h.Minutes == 0 && h.Seconds == 0)
            .WithMessage("Os agendamentos devem ser feitos em intervalos exatos de 1 hora (ex: 08:00, 09:00).")
            .Must(h => h.Hours >= 8 && h.Hours <= 17)
            .WithMessage("O horário de vacinação é das 08:00 às 17:00.");
    }
}

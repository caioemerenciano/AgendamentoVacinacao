using AgendamentoVacinacao.Entity.DTOs.Request;
using FluentValidation;

namespace AgendamentoVacinacao.Validator.Validators
{
    public class AgendamentoRequestValidator : AbstractValidator<CriarAgendamentoRequest>
    {
        public AgendamentoRequestValidator()
        {
            RuleFor(x => x.Nome)
                .NotNull().WithMessage("O nome do paciente é obrigatório.")
                .NotEmpty().WithMessage("O nome do paciente não pode estar vazio.");

            RuleFor(x => x.DataNascimento)
                .NotEmpty().WithMessage("A data de nascimento é obrigatória.");

            RuleFor(x => x.DataAgendamento)
                .NotEmpty().WithMessage("A data do agendamento é obrigatória.")
                .Must(d => d.Date >= DateTime.Now.Date).WithMessage("Data de agendamento inválida ou no passado.");

            RuleFor(x => x.HoraAgendamento)
                .NotEmpty().WithMessage("O horário é obrigatório.")
                .Must(h => h.Hours >= 8 && h.Hours <= 17).WithMessage("O horário de vacinação é das 08:00 às 17:00.");

            RuleFor(x => x)
                .Must(x =>
                {
                    if (x.DataAgendamento.Date == DateTime.Now.Date)
                    {
                        return x.HoraAgendamento > DateTime.Now.TimeOfDay;
                    }
                    return true;
                }).WithMessage("Não é possível agendar um horário que já passou no dia de hoje.");
        }
    }
}
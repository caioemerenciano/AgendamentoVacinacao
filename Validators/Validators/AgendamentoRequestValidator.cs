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
                .NotEmpty().WithMessage("A data de nascimento é obrigatória.")
                .Must(d => DateTime.TryParse(d, out _)).WithMessage("Formato de data de nascimento inválido. Use DD/MM/YYYY ou YYYY-MM-DD.");

            RuleFor(x => x.DataAgendamento)
                .NotEmpty().WithMessage("A data do agendamento é obrigatória.")
                .Must(d => {
                    string[] formats = { "dd/MM/yyyy", "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ssZ", "yyyy-MM-ddTHH:mm:ss" };
                    if (DateTime.TryParseExact(d, formats, null, System.Globalization.DateTimeStyles.None, out var date))
                    {
                        return date.Date >= DateTime.Now.Date;
                    }
                    return false;
                }).WithMessage("Data de agendamento inválida ou no passado. Use DD/MM/YYYY ou YYYY-MM-DD.");

            RuleFor(x => x.Horario)
                .NotEmpty().WithMessage("O horário é obrigatório.")
                .Matches(@"^\d{2}:\d{2}$").WithMessage("O horário deve estar no formato HH:mm.")
                .Must(h => {
                    if (TimeSpan.TryParse(h, out var time))
                    {
                        return time.Hours >= 8 && time.Hours <= 17;
                    }
                    return false;
                }).WithMessage("O horário de vacinação é das 08:00 às 17:00.");

            RuleFor(x => x)
                .Must(x =>
                {
                    if (DateTime.TryParse(x.DataAgendamento, out var date) && TimeSpan.TryParse(x.Horario, out var time))
                    {
                        if (date.Date == DateTime.Now.Date)
                        {
                            return time > DateTime.Now.TimeOfDay;
                        }
                    }
                    return true;
                }).WithMessage("Não é possível agendar um horário que já passou no dia de hoje.");
        }
    }
}
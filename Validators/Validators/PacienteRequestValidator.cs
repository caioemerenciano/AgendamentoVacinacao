using AgendamentoVacinacao.Entity.DTOs.Request;
using FluentValidation;

namespace AgendamentoVacinacao.Validators.Validators;

public class PacienteRequestValidator : AbstractValidator<CriarPacienteRequest>
{
    public PacienteRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome do paciente é obrigatório.")
            .MinimumLength(3).WithMessage("O nome deve ter no mínimo 3 caracteres.")
            .MaximumLength(150).WithMessage("O nome deve ter no máximo 150 caracteres.");

        RuleFor(x => x.DataNascimento)
            .NotEmpty().WithMessage("A data de nascimento é obrigatória.")
            .LessThan(DateTime.Now).WithMessage("A data de nascimento deve ser no passado.");
    }
}
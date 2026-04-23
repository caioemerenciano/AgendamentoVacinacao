using AgendamentoVacinacao.Entity.Entities;
using AgendamentoVacinacao.Entity.Enums;

namespace AgendamentoVacinacao.Entity.Extensions;

public static class AgendamentoExtensions
{
    public static void Cancelar(this Agendamento agendamento)
    {
        if (agendamento == null) return;
        agendamento.Status = StatusAgendamento.Cancelado;
    }
}
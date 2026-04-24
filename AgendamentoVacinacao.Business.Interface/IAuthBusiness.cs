using AgendamentoVacinacao.Entity.DTOs.Request;
using AgendamentoVacinacao.Entity.DTOs.Response;

namespace AgendamentoVacinacao.Business.Interface;

public interface IAuthBusiness
{
    Task RegistrarAsync(RegistroRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request);
}

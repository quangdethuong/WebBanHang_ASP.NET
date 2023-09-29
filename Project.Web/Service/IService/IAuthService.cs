using Project.Web.Models;

namespace Project.Web.Service.IService
{
    public interface IAuthService
    {
        Task<ResponseDto?> LoginAsycn(LoginRequestDto loginRequestDto);
        Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registrationRequestDto);
        Task<ResponseDto?> AssignRoleAsycn(RegistrationRequestDto registrationRequestDto);
    }
}

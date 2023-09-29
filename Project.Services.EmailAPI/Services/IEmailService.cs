using Project.Services.EmailAPI.Models.Dto;

namespace Project.Services.EmailAPI.Services
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDto cartDto);
    }
}

using EasyTwoJuetengBackend.Dtos.AuthDto;
using EasyTwoJuetengBackend.Dtos.ValidatorDto;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Persistence.AuthRepositories
{
    public interface IAuthRepo
    {
        Task<LoginDetailsReadDto> LoginChecker(LoginCredentialsSaveDto model);
        Task<ErrorValidator> ChangePassword(int userId, ChangePasswordSaveDto resourceSave);
    }
}
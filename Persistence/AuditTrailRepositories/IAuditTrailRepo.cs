using EasyTwoJuetengBackend.Dtos.AuditTrailDto;
using EasyTwoJuetengBackend.Enumerations;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Persistence.AuditTrailRepositories
{
    public interface IAuditTrailRepo
    {
        Task SaveFailedTrail(AuditTrailFailedSaveDto model);
        Task SaveSuccessTrail(AuditTrailSuccessSaveDto model);
        void SaveSuccessTrails(List<AuditTrailSuccessSaveDto> models);
        SystemModule Module { get; set; }
        ClaimsPrincipal User { get; set; }
        Task SaveSuccessAuthTrail(AuditTrailAuthSuccessSaveDto model);
    }
}
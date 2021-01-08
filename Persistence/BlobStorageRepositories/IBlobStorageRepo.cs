using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Persistence.BlobStorageRepositories
{
    public interface IBlobStorageRepo
    {
        Task UploadFile(IFormFile file,string desiredFileName);
        Task<FileContentResult> DownloadFile(string fileName);
    }
}
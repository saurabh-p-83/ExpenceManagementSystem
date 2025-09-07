using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.Azure
{
    public interface IFileStorageService
    {
        Task<string> UploadFileOnBlob(IFormFile file);
    }
}

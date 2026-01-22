using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Service.Services.FileService
{
    public interface IFileService
    {
        Task<string> UploadImageAsync(string location, IFormFile file);
    }
}
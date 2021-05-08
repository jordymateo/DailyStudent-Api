using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.Services.Cloud
{
    public interface IGoogleCloudService
    {
        Task<string> SaveProfileImage(IFormFile image);
        Task<string> SaveProfileImage(int userId, IFormFile image);
        Task<string> SaveAttachment(IFormFile file);
        Task<string> SaveAttachment(IFormFile file, string fileName);
    }
}

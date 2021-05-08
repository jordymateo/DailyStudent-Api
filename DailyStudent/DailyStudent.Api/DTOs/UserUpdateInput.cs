using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.DTOs
{
    public class UserUpdateInput
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public IFormFile Image { get; set; }
    }
}

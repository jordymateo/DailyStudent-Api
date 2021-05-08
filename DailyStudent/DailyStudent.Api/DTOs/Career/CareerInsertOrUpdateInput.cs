using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.DTOs.Career
{
    public class CareerInsertOrUpdateInput
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int InstitutionId { get; set; }
        public bool IsPensumAvailable { get; set; }
        public IFormFile Pensum { get; set; }
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.DTOs.Institution
{
    public class InstitutionInsertOrUpdateInput
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Acronym { get; set; }
        public IFormFile Logo { get; set; }
        public string Website { get; set; }
        public short CountryId { get; set; }
        public bool IsAvailable { get; set; }
    }
}

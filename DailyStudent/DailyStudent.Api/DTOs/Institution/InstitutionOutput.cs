using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.DTOs.Institution
{
    public class InstitutionOutput
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Acronym { get; set; }
        public string LogoPath { get; set; }
        public string Website { get; set; }
        public string Country { get; set; }
        public int CountryId { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreationDate { get; set; }
        public string State { get; set; }
    }
}

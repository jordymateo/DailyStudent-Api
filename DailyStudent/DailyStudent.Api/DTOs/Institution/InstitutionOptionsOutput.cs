using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.DTOs.Institution
{
    public class InstitutionOptionsOutput
    {
        public int Id { get; set; }
        public string Acronym { get; set; }
        public string Name { get; set; }
        public string LogoPath { get; set; }
        public bool IsAvailable { get; set; }
    }
}

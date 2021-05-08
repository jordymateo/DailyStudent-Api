using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.DTOs.Career
{
    public class UserCareerInput
    {
        public int CareerId { get; set; }
        public int PensumId { get; set; }
        public int InstitutionId { get; set; }
    }
}

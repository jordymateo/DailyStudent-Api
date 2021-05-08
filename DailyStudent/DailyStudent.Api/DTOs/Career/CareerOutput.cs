using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.DTOs.Career
{
    public class CareerOutput
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int InstitutionId { get; set; }
        public DateTime CreationDate { get; set; }
        public string InstitutionName { get; set; }
        public bool IsPensumAvailable { get; set; }
        public string State { get; set; }
    }
}

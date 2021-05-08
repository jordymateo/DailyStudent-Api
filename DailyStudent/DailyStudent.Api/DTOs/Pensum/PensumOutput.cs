using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.DTOs.Pensum
{
    public class PensumOutput
    {
        public int  Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int CarrerId { get; set; }
        public string CareerName { get; set; }
        public string InstitutionName { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsApproved { get; set; }
        public bool IsDeleted { get; set; }
        public short CreditLimitPerPeriod { get; set; }
        public string State { get; set; }
    }
}

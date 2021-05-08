using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.DTOs.Pensum
{
    public class PensumInsertOrUpdateInput
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CareerId { get; set; }
        public short CreditLimitPerPeriod { get; set; }
        public IFormFile Pensum { get; set; }
    }
}

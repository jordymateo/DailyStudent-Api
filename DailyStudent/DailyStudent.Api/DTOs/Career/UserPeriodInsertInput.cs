using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.DTOs.Career
{
    public class SubjectInsert
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public string Teacher { get; set; }
    }

    public class UserPeriodInsertOrUpdateInput
    {
        public int Id { get; set; }
        public int UserCareerId { get; set; }
        public string Name { get; set; }
        public short Number { get; set; }
        public DateTime InitialDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<SubjectInsert> Subjects { get; set; }
    }
}

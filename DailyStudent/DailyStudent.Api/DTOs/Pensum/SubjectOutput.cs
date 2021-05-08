using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.DTOs.Pensum
{
    public class SubjectOutput
    {
        public int Id { get; set; }
        public int Pensumid { get; set; }
        public string Name { get; set; }
        public string Period { get; set; }
        public string Code { get; set; }
        public string Prerequisite { get; set; }
        public string Corequisite { get; set; }
        public short Credits { get; set; }
    }
}

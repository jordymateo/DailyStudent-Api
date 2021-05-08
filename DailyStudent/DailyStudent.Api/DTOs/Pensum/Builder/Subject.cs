using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.DTOs.Pensum.Builder
{
    public class Subject
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Period { get; set; }
        public bool IsCompleted { get; set; }
        public int Credits { get; set; }
        public List<Subject> Prerequisites { get; set; }
        public List<string> Prerequisites2 { get; set; }
        public List<Subject> Corequisites { get; set; }
        public List<string> Corequisites2 { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.DTOs.Pensum.Builder
{
    public class Period
    {
        public Period(int code)
        {
            Code = code;
            Credits = 0;
            Subjects = new List<Subject>();
        }

        public int Code { get; set; }
        public string Name { get; set; }
        public List<Subject> Subjects { get; set; }
        public int Credits { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.DTOs
{
    public class CourseOrCareerOutput
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CourseTypeId { get; set; }
        public DateTime CreationDate { get; set; }
        public string InstitutionName { get; set; }
        public string InstitutionAcronym { get; set; }
        public string InstitutionLogo { get; set; }
        public bool IsDeleted { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.DTOs.Course
{
    public class CourseOutput
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string TeacherFullName { get; set; }
        public int InstitutionUserId { get; set; }
        public string CourseTypeId { get; set; }
        public DateTime CreationDate { get; set; }
    }
}

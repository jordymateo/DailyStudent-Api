using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.DTOs.Course
{
    public class CourseInsertOrUpdateInput
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string TeacherFullName { get; set; }
        public int InstitutionUserId { get; set; }
        public string CourseTypeId { get; set; }
        public int InstitutionId { get; set; }
        public int AcademicPeriodId { get; set; }
        public int AcademicPeriodCourseId { get; set; }
    }
}

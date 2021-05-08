using System;
using System.Collections.Generic;

namespace DailyStudent.Api.DataAccess
{
    public partial class AcademicPeriodCourse
    {
        public int Id { get; set; }
        public int AcademicPeriodId { get; set; }
        public int CourseId { get; set; }

        public virtual AcademicPeriod AcademicPeriod { get; set; }
        public virtual Course Course { get; set; }
    }
}

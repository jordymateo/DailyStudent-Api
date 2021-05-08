using System;
using System.Collections.Generic;

namespace DailyStudent.Api.DataAccess
{
    public partial class AcademicPeriod
    {
        public AcademicPeriod()
        {
            AcademicPeriodCourse = new HashSet<AcademicPeriodCourse>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public short Number { get; set; }
        public DateTime InitialDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UserCareerId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? DeletionDate { get; set; }

        public virtual UserCareer UserCareer { get; set; }
        public virtual ICollection<AcademicPeriodCourse> AcademicPeriodCourse { get; set; }
    }
}

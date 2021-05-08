using System;
using System.Collections.Generic;

namespace DailyStudent.Api.DataAccess
{
    public partial class UserCareer
    {
        public UserCareer()
        {
            AcademicPeriod = new HashSet<AcademicPeriod>();
        }

        public int Id { get; set; }
        public int Careerid { get; set; }
        public int InstitutionUserId { get; set; }
        public bool IsDeleted { get; set; }
        public int PensumId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? DeletionDate { get; set; }

        public virtual Career Career { get; set; }
        public virtual InstitutionUser InstitutionUser { get; set; }

        public virtual Pensum Pensum { get; set; }
        public virtual ICollection<AcademicPeriod> AcademicPeriod { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace DailyStudent.Api.DataAccess
{
    public partial class Pensum
    {
        public Pensum()
        {
            Subject = new HashSet<Subject>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int CarrerId { get; set; }
        public bool IsApproved { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatorUserId { get; set; }
        public int? ApproverUserId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? DeletionDate { get; set; }
        public short CreditLimitPerPeriod { get; set; }

        public virtual Career Career { get; set; }
        public virtual User ApproverUser { get; set; }
        public virtual User CreatorUser { get; set; }
        public virtual ICollection<Subject> Subject { get; set; }

        public virtual ICollection<UserCareer> UserCareers { get; set; }
    }
}

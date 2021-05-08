using System;
using System.Collections.Generic;

namespace DailyStudent.Api.DataAccess
{
    public partial class Career
    {
        public Career()
        {
            UserCareer = new HashSet<UserCareer>();
            Pensums = new HashSet<Pensum>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int InstitutionId { get; set; }
        public bool IsPensumAvailable { get; set; }
        public int CreatorUserId { get; set; }
        public int? ApproverUserId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ApprovalDate { get; set; }

        public virtual User ApproverUser { get; set; }
        public virtual User CreatorUser { get; set; }
        public virtual Institution Institution { get; set; }
        public virtual ICollection<UserCareer> UserCareer { get; set; }
        public virtual ICollection<Pensum> Pensums { get; set; }
    }
}

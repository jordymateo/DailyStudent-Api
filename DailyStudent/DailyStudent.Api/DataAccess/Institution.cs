using System;
using System.Collections.Generic;

namespace DailyStudent.Api.DataAccess
{
    public partial class Institution
    {
        public Institution()
        {
            Career = new HashSet<Career>();
            InstitutionUser = new HashSet<InstitutionUser>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Acronym { get; set; }
        public string LogoPath { get; set; }
        public string Website { get; set; }
        public short CountryId { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatorUserId { get; set; }
        public int? ApproverUserId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ApprovalDate { get; set; }

        public virtual User ApproverUser { get; set; }
        public virtual Country Country { get; set; }
        public virtual User CreatorUser { get; set; }
        public virtual ICollection<Career> Career { get; set; }
        public virtual ICollection<InstitutionUser> InstitutionUser { get; set; }
    }
}

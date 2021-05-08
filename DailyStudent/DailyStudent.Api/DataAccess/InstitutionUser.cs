using System;
using System.Collections.Generic;

namespace DailyStudent.Api.DataAccess
{
    public partial class InstitutionUser
    {
        public InstitutionUser()
        {
            Course = new HashSet<Course>();
            UserCareer = new HashSet<UserCareer>();
        }

        public int Id { get; set; }
        public int InstitutionId { get; set; }
        public int UserId { get; set; }
        public bool Isdeleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? DeletionDate { get; set; }

        public virtual Institution Institution { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Course> Course { get; set; }
        public virtual ICollection<UserCareer> UserCareer { get; set; }
    }
}

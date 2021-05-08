using System;
using System.Collections.Generic;

namespace DailyStudent.Api.DataAccess
{
    public partial class User
    {
        public User()
        {
            CareerApproveruser = new HashSet<Career>();
            CareerCreatoruser = new HashSet<Career>();
            EventLog = new HashSet<EventLog>();
            InstitutionApproveruser = new HashSet<Institution>();
            InstitutionCreatoruser = new HashSet<Institution>();
            InstitutionUser = new HashSet<InstitutionUser>();
            PensumApproverUser = new HashSet<Pensum>();
            PensumCreatorUser = new HashSet<Pensum>();
            Userinfo = new HashSet<UserInfo>();
            Userlogindevice = new HashSet<UserLoginDevice>();
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
        public string PasswordSalt { get; set; }
        public bool IsBloqued { get; set; }
        public string UserRolId { get; set; }

        public virtual UserRole UserRol { get; set; }
        public virtual ICollection<Career> CareerApproveruser { get; set; }
        public virtual ICollection<Career> CareerCreatoruser { get; set; }
        public virtual ICollection<EventLog> EventLog { get; set; }
        public virtual ICollection<Institution> InstitutionApproveruser { get; set; }
        public virtual ICollection<Institution> InstitutionCreatoruser { get; set; }
        public virtual ICollection<InstitutionUser> InstitutionUser { get; set; }
        public virtual ICollection<Pensum> PensumApproverUser { get; set; }
        public virtual ICollection<Pensum> PensumCreatorUser { get; set; }
        public virtual ICollection<UserInfo> Userinfo { get; set; }
        public virtual ICollection<UserLoginDevice> Userlogindevice { get; set; }
    }
}

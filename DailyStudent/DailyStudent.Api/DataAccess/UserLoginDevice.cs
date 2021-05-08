using System;
using System.Collections.Generic;

namespace DailyStudent.Api.DataAccess
{
    public partial class UserLoginDevice
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Ip { get; set; }
        public string Location { get; set; }
        public DateTime CreationDate { get; set; }

        public virtual User User { get; set; }
    }
}

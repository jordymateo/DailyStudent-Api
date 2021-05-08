using System;
using System.Collections.Generic;

namespace DailyStudent.Api.DataAccess
{
    public partial class UserInfo
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UserId { get; set; }
        public DateTime CreationDate { get; set; }

        public virtual User User { get; set; }
    }
}

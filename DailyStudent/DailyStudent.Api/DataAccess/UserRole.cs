using System;
using System.Collections.Generic;

namespace DailyStudent.Api.DataAccess
{
    public partial class UserRole
    {
        public UserRole()
        {
            User = new HashSet<User>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<User> User { get; set; }
    }
}

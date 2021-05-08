using System;
using System.Collections.Generic;

namespace DailyStudent.Api.DataAccess
{
    public partial class EventLog
    {
        public int Id { get; set; }
        public string Level { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int CreatorUserId { get; set; }
        public DateTime CreationDate { get; set; }

        public virtual User CreatorUser { get; set; }
    }
}

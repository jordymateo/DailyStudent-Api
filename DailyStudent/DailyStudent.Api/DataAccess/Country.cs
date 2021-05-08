using System;
using System.Collections.Generic;

namespace DailyStudent.Api.DataAccess
{
    public partial class Country
    {
        public Country()
        {
            Institution = new HashSet<Institution>();
        }

        public short Id { get; set; }
        public string Iso { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Institution> Institution { get; set; }
    }
}

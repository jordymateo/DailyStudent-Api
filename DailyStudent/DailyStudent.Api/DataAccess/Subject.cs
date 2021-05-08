using System;
using System.Collections.Generic;

namespace DailyStudent.Api.DataAccess
{
    public partial class Subject
    {
        public int Id { get; set; }
        public int Pensumid { get; set; }
        public string Name { get; set; }
        public string Period { get; set; }
        public string Code { get; set; }
        public string Prerequisite { get; set; }
        public string Corequisite { get; set; }
        public short Credits { get; set; }

        public DateTime Creationdate { get; set; }

        public virtual Pensum Pensum { get; set; }
    }
}

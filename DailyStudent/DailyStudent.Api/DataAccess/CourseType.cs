using System;
using System.Collections.Generic;

namespace DailyStudent.Api.DataAccess
{
    public partial class CourseType
    {
        public CourseType()
        {
            Course = new HashSet<Course>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public int CreatorUserId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? ModifierUserId { get; set; }

        public virtual ICollection<Course> Course { get; set; }
    }
}

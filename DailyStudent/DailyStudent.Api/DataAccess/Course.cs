using System;
using System.Collections.Generic;

namespace DailyStudent.Api.DataAccess
{
    public partial class Course
    {
        public Course()
        {
            AcademicPeriodCourse = new HashSet<AcademicPeriodCourse>();
            Assignment = new HashSet<Assignment>();
            Note = new HashSet<Note>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string TeacherFullName { get; set; }
        public int InstitutionUserId { get; set; }
        public string CourseTypeId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? DeletionDate { get; set; }

        public virtual CourseType CourseType { get; set; }
        public virtual InstitutionUser InstitutionUser { get; set; }
        public virtual ICollection<AcademicPeriodCourse> AcademicPeriodCourse { get; set; }
        public virtual ICollection<Assignment> Assignment { get; set; }
        public virtual ICollection<Note> Note { get; set; }
    }
}

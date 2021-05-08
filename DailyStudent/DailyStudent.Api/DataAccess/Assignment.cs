using System;
using System.Collections.Generic;

namespace DailyStudent.Api.DataAccess
{
    public partial class Assignment
    {
        public Assignment()
        {
            AttachedDocument = new HashSet<AttachedDocument>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Descripcion { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsIndividual { get; set; }
        public bool IsCompleted { get; set; }
        public int CourseId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? DeletionDate { get; set; }

        public virtual Course Course { get; set; }
        public virtual ICollection<AttachedDocument> AttachedDocument { get; set; }
    }
}

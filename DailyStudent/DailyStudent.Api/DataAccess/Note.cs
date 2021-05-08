using System;
using System.Collections.Generic;

namespace DailyStudent.Api.DataAccess
{
    public partial class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Courseid { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? DeletionDate { get; set; }

        public virtual Course Course { get; set; }
    }
}

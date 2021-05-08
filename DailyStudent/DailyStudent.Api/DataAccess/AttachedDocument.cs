using System;
using System.Collections.Generic;

namespace DailyStudent.Api.DataAccess
{
    public partial class AttachedDocument
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int AssigmentId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? DeletionDate { get; set; }

        public virtual Assignment Assigment { get; set; }
    }
}

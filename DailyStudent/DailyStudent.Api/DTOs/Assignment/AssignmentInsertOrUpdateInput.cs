using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.DTOs.Assignment
{
    public class AssignmentInsertOrUpdateInput
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Descripcion { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsIndividual { get; set; }
        public bool IsCompleted { get; set; }
        public int CourseId { get; set; }
        public List<IFormFile> Files { get; set; }
    }
}

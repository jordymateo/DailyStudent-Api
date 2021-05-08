using DailyStudent.Api.DTOs.Assignment;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.Services.Assignment
{
    public interface IAssignmentService
    {
        public Task Delete(int assignmentId);
        public Task Complete(int assignmentId);
        public Task<AssignmentOutput> Get();
        public Task<List<AssignmentOutput>> GetAll();
        public Task<List<AssignmentOutput>> GetByCourse(int courseId);
        public Task<dynamic> GetToCalendar();
        public Task<dynamic> GetByCareer(int userCareerId);
        public Task<AssignmentOutput> Insert(AssignmentInsertOrUpdateInput input);
        public Task<AssignmentOutput> Update(AssignmentInsertOrUpdateInput input);
    }
}

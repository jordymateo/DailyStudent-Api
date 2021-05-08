using DailyStudent.Api.DTOs.Course;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.Services.Course
{
    public interface ICourseService
    {
        public Task<CourseOutput> Get(int courseId);
        public Task<List<CourseOutput>> GetAll();
        public Task<CourseOutput> Insert(CourseInsertOrUpdateInput input);
        public Task<CourseOutput> Update(CourseInsertOrUpdateInput input);
        public Task Delete(int courseId);
    }
}

using DailyStudent.Api.DTOs.Course;
using DailyStudent.Api.DTOs.Pensum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.Services.Pensums
{
    public interface IPensumsService
    {
        Task<PensumOutput> Get(int id);
        Task<List<PensumOutput>> GetAll();
        Task<List<PensumOutput>> GetByCareer(int careerId);
        Task<List<DTOs.Pensum.Builder.Period>> GenerateAcademicPlan(int pensumId, List<string> completedSubjects);
        //Task<dynamic> GetUserPensums();
        Task<PensumOutput> Insert(PensumInsertOrUpdateInput input);
        Task<PensumOutput> Update(PensumInsertOrUpdateInput input);
        Task ToggleState(int pensumId);
        Task Approve(int pensumId);

        //Subject
        Task<List<SubjectOutput>> GetSubjects(int pensumId);
        Task<List<CourseOutput>> GetUserSubjects(int userCareerId);
        Task InsertSubject(SubjectInsertOrUpdateInput input);
        Task UpdateSubject(SubjectInsertOrUpdateInput input);
        Task DeleteSubject(int subjectId);
    }
}

using DailyStudent.Api.DTOs.Career;
using DailyStudent.Api.DTOs.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.Services.Careers
{
    public interface ICareersService
    {
        Task<CareerOutput> Get(int id);
        Task<List<CareerOutput>> GetAll();
        Task<List<CareerOutput>> GetByInstitution(int institutionId);
        Task<dynamic> GetUserPeriods(int userCareerId);
        //Task<dynamic> GetUserCareers();
        Task<CareerOutput> Insert(CareerInsertOrUpdateInput input);
        Task<UserCareerOutput> InsertUserCareer(UserCareerInput input);
        Task InsertUserPeriod(UserPeriodInsertOrUpdateInput input);
        Task InsertPeriodSubject(CourseInsertOrUpdateInput input);
        Task<CareerOutput> Update(CareerInsertOrUpdateInput input);
        Task UpdateUserPeriod(UserPeriodInsertOrUpdateInput input);
        //Task ToggleState(int careerId);
        Task Approve(int careerId);
    }
}

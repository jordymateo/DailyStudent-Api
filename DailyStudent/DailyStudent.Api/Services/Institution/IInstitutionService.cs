using DailyStudent.Api.DTOs.Institution;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.Services.Institution
{
    public interface IInstitutionService
    {
        Task<InstitutionOutput> Get(int id);
        Task<List<InstitutionOutput>> GetAll();
        Task<List<InstitutionOutput>> GetAvailable();
        Task<dynamic> GetUserInstitutions();
        Task<dynamic> GetUserCareers();
        Task<dynamic> GetUserInstitutionsInLine();
        Task<InstitutionOutput> Insert(InstitutionInsertOrUpdateInput input);
        Task<InstitutionOutput> Update(InstitutionInsertOrUpdateInput input);
        Task ToggleState(int institutionId);
        Task ToggleUserCareerState(int userCareerId);
        Task ToggleUserCourseState(int userCourseId);
        Task Approve(int institutionId);
    }
}

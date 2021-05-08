using DailyStudent.Api.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.Services.Countries
{
    public interface ICountriesService
    {
        Task<List<Country>> Get();
    }
}

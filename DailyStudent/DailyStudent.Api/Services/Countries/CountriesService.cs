using DailyStudent.Api.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.Services.Countries
{
    public class CountriesService: ICountriesService
    {
        private readonly DailyStudentDbContext _context;
        public CountriesService(DailyStudentDbContext context)
        {
            _context = context;
        }

        public async Task<List<Country>> Get()
        {
            return await _context.Country
                .ToListAsync();
        }
    }
}

using DailyStudent.Api.DataAccess;
using DailyStudent.Api.Services.Security;
using DailyStudent.Api.Services.Security.UserContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace DailyStudent.Tests
{
    public abstract class TestBase : IDisposable
    {
        protected readonly DailyStudentDbContext _context;
        protected readonly IUserContext _dUserContext;
        protected TestBase()
        {
            _context = Setups.CreateDefaultDbContext();
            _dUserContext = Setups.CreateDefaultUserContext();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}

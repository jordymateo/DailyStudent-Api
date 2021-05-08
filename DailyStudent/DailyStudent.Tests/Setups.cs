using DailyStudent.Api.DataAccess;
using DailyStudent.Api.Services.Security;
using DailyStudent.Api.Services.Security.UserContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DailyStudent.Tests
{
    public class Setups
    {
        internal static DailyStudentDbContext CreateDefaultDbContext()
        {
            var options = new DbContextOptionsBuilder<DailyStudentDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

            var context = new DailyStudentDbContext(options);

            //Insert Mockup data
            context.Institution.AddRange(TestData.InstitutionsData());
            context.User.AddRange(TestData.UsersData());
            context.Country.AddRange(TestData.ContriesData());
            context.Course.AddRange(TestData.CoursesData());
            context.Assignment.AddRange(TestData.AssignmentsData());
            context.Note.AddRange(TestData.NotesData());


            context.SaveChanges();
            return context;
        }

        internal static IUserContext CreateDefaultUserContext()
        {
            var userContext = new Mock<IUserContext>();
            userContext.Setup(x => x.User).Returns(new SessionUser { Id = 1 });
            return userContext.Object;
        }
    }
}

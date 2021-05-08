using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyStudent.Api.DTOs.Course;
using DailyStudent.Api.Exceptions;
using DailyStudent.Api.Services.Course;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DailyStudent.Tests.Service.Course
{
    public class CourseServiceTest : TestBase
    {
        private readonly ICourseService _courseService;


        public CourseServiceTest()
        {
            _courseService = new CourseService(_context, _dUserContext);
        }
        //Methods to test: Insert, Update, Get, GetAll and Delete

        //[Fact]
        //public async void Insert_CompleteInformation_CourseInsert()
        //{
        //    var newCourse = new CourseInsertOrUpdateInput
        //    {
        //        Id = 1,
        //        Name = "Apps moviles",
        //        Color = "Blue",
        //        TeacherFullName = "Juan Ramírez",
        //        AcademicPeriodCourseId = 1,
        //        InstitutionId = 1,
        //        InstitutionUserId = 2

        //    };

        //    var actual = await _courseService.Insert(newCourse);

        //    Assert.NotNull(actual);
        //    Assert.IsType<CourseOutput>(actual);
        //}

        //[Theory]
        //[InlineData(null)]
        //[InlineData("")]
        //public void Insert_IsNullOrEmptyCourseName_CourseInsert(string courseName)
        //{
        //    // arrange
        //    var newCourse = new CourseInsertOrUpdateInput
        //    {
        //        Id = 1,
        //        Name = courseName,
        //        Color = "Blue",
        //        TeacherFullName = "Juan Ramírez",
        //        AcademicPeriodCourseId = 1,
        //        InstitutionId = 1,
        //        InstitutionUserId = 2

        //    };
        //    // act
        //    bool completed = _courseService.Insert(newCourse).IsCompletedSuccessfully;
        //    // assert
        //    Assert.True(completed);
        //}

        [Theory]
        [InlineData(true)]
        public  void Update_CompleteInformation_CourseModified(bool expected)
        {
            // arrange
            var newCourse = new CourseInsertOrUpdateInput
            {
                Id = 1,
                Name = "Apps moviles",
                Color = "Blue",
                TeacherFullName = "Pedro Ramírez",
            };

            bool courseUpdated = _courseService.Update(newCourse).IsCompleted;

            // assert
            Assert.Equal(expected, courseUpdated);
        }

        [Fact]
        public  void Update_InvalidCourseTypeId_Throw_Exception()
        {
            // arrange
            var newCourse = new CourseInsertOrUpdateInput
            {
                Id = 1,
                Name = "Apps moviles",
                Color = "Blue",
                TeacherFullName = "Pedro Ramírez",
                CourseTypeId = "23"
            };

            Func<Task> actual = () => _courseService.Update(newCourse);

            // assert
            Assert.ThrowsAsync<Exception>(actual);
        }

        [Theory]
        [InlineData(2, true)]
        [InlineData(3, true)]
        [InlineData(4, false)]
        [InlineData(-4, false)]
        [InlineData(1000, false)]
        public void Delete_ValidateIfCourseExist_CourseDeletedorNotDeleted(int courseId, bool expected)
        {
            var newCourse = new CourseInsertOrUpdateInput
            {
                Id = courseId,
                Name = "Apps moviles",
                Color = "Blue",
                TeacherFullName = "Juan Ramírez",
                AcademicPeriodCourseId = 1,
                InstitutionId = 1,
                InstitutionUserId = 2

            };
            bool actual = _courseService.Delete(newCourse.Id).IsCompletedSuccessfully;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, true)]
        public void Delete_DeleteCourseSuccessfully_CourseDeleted(int courseId, bool expected)
        {
            var newCourse = new CourseInsertOrUpdateInput
            {
                Id = courseId,
                Name = "Apps moviles",
                Color = "Blue",
                TeacherFullName = "Juan Ramírez",
                AcademicPeriodCourseId = 1,
                InstitutionId = 1,
                InstitutionUserId = 2

            };
            bool actual = _courseService.Delete(newCourse.Id).IsCompletedSuccessfully;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public  void Delete_NotExistCourse_Throw_Exception()
        {
            var newCourse = new CourseInsertOrUpdateInput
            {
                Id = 1111,
                Name = "Apps moviles",
                Color = "Blue",
                TeacherFullName = "Juan Ramírez",
                AcademicPeriodCourseId = 1,
                InstitutionId = 1,
                InstitutionUserId = 2

            };
            _courseService.Delete(newCourse.Id);
            Assert.ThrowsAsync<Exception> (() => _courseService.Delete(newCourse.Id));
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, true)]
        public void Get_ValidateIfCourseIdExists_CourseData(int courseId, bool expected) 
        {
            var newCourse = new CourseInsertOrUpdateInput
            {
                Id = courseId,
                Name = "Apps moviles",
                Color = "Blue",
                TeacherFullName = "Juan Ramírez",
                AcademicPeriodCourseId = 1,
                InstitutionId = 1,
                InstitutionUserId = 2

            };
            bool actual = _courseService.Get(courseId).IsCompletedSuccessfully; 

            Assert.Equal(expected,actual);
        }
    }
}

using DailyStudent.Api.DataAccess;
using DailyStudent.Api.DTOs.Course;
using DA = DailyStudent.Api.DataAccess;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DailyStudent.Api.Services.Security.UserContext;
using DailyStudent.Api.Exceptions;

namespace DailyStudent.Api.Services.Course
{
    public class CourseService : ICourseService
    {
        DailyStudentDbContext _context;
        IUserContext _userContext;

        public CourseService(DailyStudentDbContext context, IUserContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public async Task Delete(int courseId)
        {
            IDbContextTransaction transaction = null;

            try
            {
                var course = await _context.Course.SingleOrDefaultAsync(course => course.Id == courseId);
                
                if (course is null)
                    throw new Exception("El curso indicado no existe.");

                transaction = await _context.Database.BeginTransactionAsync();

                course.IsDeleted = true;
                course.DeletionDate = DateTime.UtcNow;

                await transaction.CommitAsync();

                // TODO Implementarlog
            }
            catch (Exception ex)
            {
                // TODO Implementar log

                if (transaction != null)
                    transaction.Rollback();
            }
            finally
            {
                transaction.Dispose();
            }
        }

        public async Task<CourseOutput> Get(int courseId)
        {
            var course = await _context.Course.SingleOrDefaultAsync(course => course.Id == courseId && !course.IsDeleted);

            return course == null ? null : Map(course);
        }

        public async Task<List<CourseOutput>> GetAll()
        {
            var result = new List<CourseOutput>();

            var courses = await _context.Course.Where(course => !course.IsDeleted).ToListAsync();

            courses.ForEach(course =>
            {
                result.Add(Map(course));
            });

            return result;
        }

        public async Task<CourseOutput> Insert(CourseInsertOrUpdateInput input)
        {
            IDbContextTransaction transaction = null;

            try
            {
                ValidateCourseInput(input);

                var currentSessionUser = _userContext.User;

                var currentUser = _context.User.SingleOrDefault(user => user.Id == currentSessionUser.Id);
                var academicPeriodCourse = _context.AcademicPeriodCourse.SingleOrDefault(period => period.Id == input.AcademicPeriodCourseId);

                var institutionUser = _context.InstitutionUser.SingleOrDefault(x => x.UserId == _userContext.User.Id && x.InstitutionId == input.InstitutionId);

                var courseType = _context.CourseType.SingleOrDefault(courseType => courseType.Id == input.CourseTypeId);
                if (courseType is null)
                    throw new MessageException(4, nameof(input.CourseTypeId), input.CourseTypeId.ToString());

                transaction = _context.Database.BeginTransaction();
                if (institutionUser is null)
                {
                    institutionUser = new InstitutionUser
                    {
                        InstitutionId = input.InstitutionId,
                        UserId = currentSessionUser.Id,
                        Isdeleted = false,
                        CreationDate = DateTime.UtcNow
                    };
                    _context.InstitutionUser.Add(institutionUser);
                    _context.SaveChanges();
                }

                var newCourse = new DA.Course()
                {
                    Name = input.Name,
                    Color = input.Color,
                    TeacherFullName = input.TeacherFullName,                    
                    CourseType = courseType,
                    InstitutionUser = institutionUser,
                    IsDeleted = false,
                    CreationDate = DateTime.UtcNow,
                };

                if (academicPeriodCourse != null)
                    newCourse.AcademicPeriodCourse.Add(academicPeriodCourse);

                _context.Course.Add(newCourse);

                await _context.SaveChangesAsync();

                transaction.Commit();

                // TODO Log

                return Map(newCourse);
            }
            catch (Exception ex)
            {
                // TODO Implementar log

                if (transaction != null)
                    transaction.Rollback();

                throw ex;
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();
            }
        }

        public async Task<CourseOutput> Update(CourseInsertOrUpdateInput input)
        {
            IDbContextTransaction transaction = null;

            try
            {
                ValidateCourseInput(input);

                var existingCourse = _context.Course.SingleOrDefault(course => course.Id == input.Id);

                if (existingCourse == null)
                    throw new MessageException(4, nameof(input.Id), input.Id.ToString());

                transaction = await _context.Database.BeginTransactionAsync();

                input.Color = (input.Color.StartsWith('#')) ? input.Color.Substring(1, input.Color.Length - 1) : input.Color;

                existingCourse.Name = input.Name == existingCourse.Name ? existingCourse.Name : input.Name;
                existingCourse.TeacherFullName = input.TeacherFullName == existingCourse.TeacherFullName 
                    ? existingCourse.TeacherFullName : input.TeacherFullName;
                existingCourse.Color = input.Color == existingCourse.Color ? existingCourse.Color : input.Color;


                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // TODO Implementar log

                return Map(existingCourse);
            }
            catch (Exception ex)
            {
                // TODO Implementar log

                if (transaction != null)
                    transaction.Rollback();

                throw ex;
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();
            }
        }

        private void ValidateCourseInput(CourseInsertOrUpdateInput input)
        {
            var message = string.Empty;

            var institution = _context.Institution.SingleOrDefault(inst => inst.Id == input.InstitutionId);

            if (string.IsNullOrEmpty(input.Name))
                message = nameof(input.Name);
            else if (string.IsNullOrEmpty(input.Color))
                message = nameof(input.Color);
            else if (string.IsNullOrEmpty(input.TeacherFullName))
                message = nameof(input.TeacherFullName);

            if (!string.IsNullOrEmpty(message))
                throw new MessageException(1, message);
        }

        private CourseOutput Map(DA.Course course)
        {
            return new CourseOutput()
            {
                Id = course.Id,
                Name = course.Name,
                Color = course.Color,
                TeacherFullName = course.TeacherFullName,
                CourseTypeId = course.CourseTypeId,
                InstitutionUserId = course.InstitutionUserId,
                CreationDate = course.CreationDate
            };
        }
    }
}

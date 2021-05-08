using DailyStudent.Api.DTOs.Assignment;
using DA = DailyStudent.Api.DataAccess;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DailyStudent.Api.DataAccess;
using DailyStudent.Api.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using DailyStudent.Api.Services.Cloud;
using DailyStudent.Api.Services.Security.UserContext;

namespace DailyStudent.Api.Services.Assignment
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IUserContext _userContext;
        private readonly DailyStudentDbContext _context;
        private readonly IGoogleCloudService _cloudService;
        public AssignmentService(DailyStudentDbContext context, IGoogleCloudService cloudService, IUserContext userContext)
        {
            _context = context;
            _userContext = userContext;
            _cloudService = cloudService;
        }

        public Task<AssignmentOutput> Get()
        {
            throw new NotImplementedException();
        }

        public async Task<List<AssignmentOutput>> GetByCourse(int courseId)
        {
            return await _context.Assignment
                .Include(x => x.AttachedDocument)
                .AsNoTracking()
                .Where(x => x.CourseId == courseId && !x.IsDeleted)
                .OrderByDescending(x => x.CreationDate)
                .Select(x => new AssignmentOutput
                {
                    Id = x.Id,
                    Title = x.Title,
                    Descripcion = x.Descripcion,
                    DueDate = x.DueDate,
                    CourseId = x.CourseId,
                    IsIndividual = x.IsIndividual,
                    IsCompleted = x.IsCompleted,
                    Files = x.AttachedDocument.ToList(),
                })
                .ToListAsync();
        }

        public async Task<dynamic> GetByCareer(int userCareerId)
        {
            var currentDate = DateTime.UtcNow.Date;
            var academicPeriod = _context.AcademicPeriods
                .Include(x => x.UserCareer)
                    .ThenInclude(x => x.Career)
                .Where(x => x.UserCareerId == userCareerId && x.InitialDate <= currentDate && x.EndDate >= currentDate)
                .FirstOrDefault();
            dynamic subjects = null;
            if (academicPeriod != null)
            {
                subjects = await _context.AcademicPeriodCourse
                    .Include(x => x.Course)
                        .ThenInclude(x => x.Assignment)
                            .ThenInclude(x => x.AttachedDocument)
                    .Where(x => x.AcademicPeriodId == academicPeriod.Id && !x.Course.IsDeleted)
                    .Select(x => new
                    {
                        Id = x.Course.Id,
                        Name = x.Course.Name,
                        Color = x.Course.Color,
                        TeacherFullName = x.Course.TeacherFullName,
                        Assignments = x.Course.Assignment
                                            .OrderByDescending(x => x.CreationDate)
                                            .Select(y => new 
                                            {
                                                Id = y.Id,
                                                Title = y.Title,
                                                Descripcion = y.Descripcion,
                                                DueDate = y.DueDate,
                                                CourseId = y.CourseId,
                                                IsIndividual = y.IsIndividual,
                                                IsCompleted = y.IsCompleted,
                                                Files = y.AttachedDocument.ToList(),
                                            })
                                            .ToList()
                    })
                    .ToListAsync();

                return new
                {
                    CareerName = academicPeriod.UserCareer.Career.Name,
                    Subjects = subjects
                };
            }
            return null;
            
        }
        public async Task<dynamic> GetToCalendar()
        {
            var courses = await _context.Course
                .Include(x => x.InstitutionUser)
                .Where(x => !x.InstitutionUser.Isdeleted && x.InstitutionUser.UserId == _userContext.User.Id && !x.IsDeleted)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Color
                })
                .ToListAsync();

            var assignments = await _context.Assignment
                .Include(x => x.Course)
                    .ThenInclude(x => x.InstitutionUser)
                        .ThenInclude(x => x.Institution)
                .Where(x => !x.Course.IsDeleted && !x.IsDeleted && !x.IsCompleted && x.Course.InstitutionUser.UserId == _userContext.User.Id)
                .Select(x => new 
                { 
                    InstitutionName = x.Course.InstitutionUser.Institution.Name,
                    InstitutionLogo = x.Course.InstitutionUser.Institution.LogoPath,
                    CourseName = x.Course.Name,
                    x.Id,
                    x.Title,
                    x.Descripcion,
                    x.DueDate,
                    x.CourseId,
                    x.IsIndividual,
                    Files = x.AttachedDocument.ToList()
                })
                .ToListAsync();

            return new
            {
                Courses = courses,
                Assignments = assignments
            };
        }
        public async Task<List<AssignmentOutput>> GetAll()
        {

            throw new NotImplementedException();
        }

        public async Task<AssignmentOutput> Insert(AssignmentInsertOrUpdateInput input)
        {
            IDbContextTransaction transaction = null;
            try
            {

                if (string.IsNullOrWhiteSpace(input.Title))
                    throw new MessageException(1, nameof(input.Title));
                if (string.IsNullOrWhiteSpace(input.Descripcion))
                    throw new MessageException(1, nameof(input.Descripcion));
                if (input.DueDate == null)
                    throw new MessageException(1, nameof(input.DueDate));

                var course = await _context.Course.AsNoTracking().SingleOrDefaultAsync(x => x.Id == input.CourseId);

                if (course == null)
                    throw new MessageException(4, nameof(input.CourseId), input.CourseId.ToString());

                transaction = _context.Database.BeginTransaction();

                var newAssigment = new DA.Assignment
                {
                    Title = input.Title,
                    Descripcion = input.Descripcion,
                    DueDate = input.DueDate,
                    CreationDate = DateTime.UtcNow,
                    CourseId = input.CourseId,
                    IsCompleted = false,
                    IsIndividual = input.IsIndividual
                };

                _context.Assignment.Add(newAssigment);

                await _context.SaveChangesAsync();

                if (input.Files != null)
                {
                    foreach (var file in input.Files)
                    {
                        var fileLink = await _cloudService.SaveAttachment(file);
                        var newAttach = new AttachedDocument
                        {
                            AssigmentId = newAssigment.Id,
                            Name = file.FileName,
                            CreationDate = DateTime.UtcNow,
                            Path = fileLink
                        };
                        _context.AttachedDocument.Add(newAttach);
                    }
                }

                await _context.SaveChangesAsync();

                transaction.Commit();

                return Map(newAssigment);
            }
            catch (Exception ex)
            {
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

        public async Task<AssignmentOutput> Update(AssignmentInsertOrUpdateInput input)
        {

            IDbContextTransaction transaction = null;
            try
            {

                if (input.Id == 0)
                    throw new MessageException(1, nameof(input.Id));
                if (string.IsNullOrWhiteSpace(input.Title))
                    throw new MessageException(1, nameof(input.Title));
                if (string.IsNullOrWhiteSpace(input.Descripcion))
                    throw new MessageException(1, nameof(input.Descripcion));
                if (input.DueDate == null)
                    throw new MessageException(1, nameof(input.DueDate));

                var course = await _context.Course.AsNoTracking().SingleOrDefaultAsync(x => x.Id == input.CourseId);

                if (course == null)
                    throw new MessageException(4, nameof(input.CourseId), input.CourseId.ToString());


                var assigment = await _context.Assignment.SingleOrDefaultAsync(x => x.Id == input.Id);

                if (assigment == null)
                    throw new MessageException(4, nameof(input.Id), input.Id.ToString());

                transaction = _context.Database.BeginTransaction();

                assigment.Title = input.Title;
                assigment.Descripcion = input.Descripcion;
                assigment.DueDate = input.DueDate;
                assigment.IsIndividual = input.IsIndividual;

                await _context.SaveChangesAsync();

                if (input.Files != null)
                {
                    foreach (var file in input.Files)
                    {
                        var fileLink = await _cloudService.SaveAttachment(file);
                        var newAttach = new AttachedDocument
                        {
                            AssigmentId = assigment.Id,
                            Name = file.FileName,
                            CreationDate = DateTime.UtcNow,
                            Path = fileLink
                        };
                        _context.AttachedDocument.Add(newAttach);
                    }
                }

                await _context.SaveChangesAsync();

                transaction.Commit();

                return Map(assigment);
            }
            catch (Exception ex)
            {
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


        public async Task Complete(int assignmentId)
        {
            if (assignmentId == 0)
                throw new MessageException(1, nameof(assignmentId));

            var assigment = await _context.Assignment.SingleOrDefaultAsync(x => x.Id == assignmentId);

            if (assigment == null)
                throw new MessageException(4, nameof(assignmentId), assignmentId.ToString());


            assigment.IsCompleted = true;

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int assignmentId)
        {
            if (assignmentId == 0)
                throw new MessageException(1, nameof(assignmentId));

            var assigment = await _context.Assignment.SingleOrDefaultAsync(x => x.Id == assignmentId);

            if (assigment == null)
                throw new MessageException(4, nameof(assignmentId), assignmentId.ToString());

            assigment.DeletionDate = DateTime.UtcNow;
            assigment.IsDeleted = true;

            await _context.SaveChangesAsync();
        }

        private AssignmentOutput Map(DA.Assignment input)
        {
            return new AssignmentOutput()
            {
                Id = input.Id,
                Title = input.Title,
                Descripcion = input.Descripcion,
                DueDate = input.DueDate,
                CourseId = input.CourseId,
                IsIndividual = input.IsIndividual,
                IsCompleted = input.IsCompleted,
                Files = input.AttachedDocument.ToList()
            };
        }
    }
}

using DailyStudent.Api.Constants;
using DailyStudent.Api.DataAccess;
using DailyStudent.Api.DTOs.Course;
using DailyStudent.Api.DTOs.Pensum;
using DailyStudent.Api.Exceptions;
using DailyStudent.Api.Services.Cloud;
using DailyStudent.Api.Services.Security.UserContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.Services.Pensums
{
    public class PensumsService: IPensumsService
    {
        private readonly DailyStudentDbContext _context;
        private readonly IUserContext _userContext;
        private readonly IGoogleCloudService _cloudService;

        public PensumsService(DailyStudentDbContext context, IUserContext userContext, IGoogleCloudService cloudService)
        {
            _context = context;
            _userContext = userContext;
            _cloudService = cloudService;
        }

        public async Task<PensumOutput> Insert(PensumInsertOrUpdateInput input)
        {
            IDbContextTransaction transaction = null;

            try
            {
                ValidatePensumInput(input);

                var currentSessionUser = _userContext.User;

                var currentUser = _context.User.SingleOrDefault(user => user.Id == currentSessionUser.Id);
                var career = _context.Career.SingleOrDefault(x => x.Id == input.CareerId);

                if (career is null)
                    throw new MessageException(4, nameof(input.CareerId), input.CareerId.ToString());

                if (currentUser is null)
                    throw new MessageException(2);

                var pensumPath = string.Empty;
                if (input.Pensum != null)
                    pensumPath = await _cloudService.SaveAttachment(input.Pensum);

                Pensum newPensum;
                if (currentUser.UserRolId == UserRoles.Administrator)
                {
                    newPensum = new Pensum
                    {
                        Name = input.Name,
                        Path = pensumPath,
                        IsApproved = true,
                        IsDeleted = false,
                        CreatorUser = currentUser,
                        ApproverUser = currentUser,
                        DeletionDate = null,
                        CarrerId = input.CareerId,
                        CreationDate = DateTime.UtcNow,
                        ApprovalDate = DateTime.UtcNow,
                        CreditLimitPerPeriod = input.CreditLimitPerPeriod
                    };
                }
                else
                {
                    newPensum = new Pensum
                    {
                        Name = input.Name,
                        Path = pensumPath,
                        IsApproved = false,
                        IsDeleted = false,
                        CarrerId = input.CareerId,
                        CreatorUser = currentUser,
                        DeletionDate = null,
                        CreationDate = DateTime.UtcNow,
                        CreditLimitPerPeriod = input.CreditLimitPerPeriod
                    };
                }


                transaction = _context.Database.BeginTransaction();

                _context.Pensum.Add(newPensum);

                if (!career.IsPensumAvailable)
                    career.IsPensumAvailable = true;

                await _context.SaveChangesAsync();

                transaction.Commit();


                return new PensumOutput
                {
                    Id = newPensum.Id,
                    Name = input.Name,
                    Path = newPensum.Path,
                    CreationDate = newPensum.CreationDate
                };
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

        public async Task ToggleState(int pensumId)
        {
            var pensum = await _context.Pensum.SingleOrDefaultAsync(x => x.Id == pensumId);

            if (pensum == null)
                throw new MessageException(4, nameof(pensumId), pensumId.ToString());

            pensum.IsDeleted = !pensum.IsDeleted;

            if (pensum.IsDeleted)
                pensum.DeletionDate = DateTime.UtcNow;
            else
                pensum.DeletionDate = null;

            await _context.SaveChangesAsync();
        }

        public async Task Approve(int pensumId)
        {
            var pensum = await _context.Pensum.SingleOrDefaultAsync(x => x.Id == pensumId);

            if (pensum == null)
                throw new MessageException(4, nameof(pensumId), pensumId.ToString());

            pensum.IsApproved = true;
            pensum.ApprovalDate = DateTime.UtcNow;
            pensum.ApproverUserId = _userContext.User.Id;

            await _context.SaveChangesAsync();
        }

        public async Task<PensumOutput> Get(int pensumId)
        {
            var pensum = await _context.Pensum
                .Include(x => x.Career)
                    .ThenInclude(x => x.Institution)
                .Select(x => new PensumOutput
                {
                    Id = x.Id,
                    Name = x.Name,
                    CarrerId = x.Career.Id,
                    CareerName = x.Career.Name,
                    InstitutionName = x.Career.Institution.Name,
                    CreationDate = x.CreationDate,
                    Path = x.Path
                })
                .SingleOrDefaultAsync(x => x.Id == pensumId);
            return pensum;
        }

        //public async Task<dynamic> GetUserPensums()
        //{
        //    var institution = await _context.InstitutionUser.Include(x => x.Institution)
        //                                .Where(x => x.UserId == _userContext.User.Id)
        //                                .Select(x => new
        //                                {
        //                                    x.Institution.Id,
        //                                    x.Institution.Name,
        //                                    x.Institution.LogoPath,
        //                                    Courses = _context.Course.Where(course => course.InstitutionUserId == x.Id && course.CourseTypeId == "course")
        //                                    .Select(y => new
        //                                    {
        //                                        y.Id,
        //                                        y.Name,
        //                                        y.CourseTypeId,
        //                                        InstitutionName = x.Institution.Name,
        //                                        InstitutionLogo = x.Institution.LogoPath
        //                                    })
        //                                    .ToList() //TODO: Colocar en constantes
        //                                })
        //                                .ToListAsync();

        //    return institution;
        //}

        public async Task<List<PensumOutput>> GetAll()
        {

            var pensums = await _context.Pensum
                .Include(x => x.Career)
                    .ThenInclude(x => x.Institution)
                .Select(x => new PensumOutput
                {
                    Id = x.Id,
                    Name = x.Name,
                    CarrerId = x.Career.Id,
                    CareerName = x.Career.Name,
                    InstitutionName = x.Career.Institution.Name,
                    CreationDate = x.CreationDate,
                    Path = x.Path,
                    IsApproved = x.IsApproved,
                    IsDeleted = x.IsDeleted,
                    CreditLimitPerPeriod = x.CreditLimitPerPeriod,
                    State = (!x.IsApproved) ? "Solicitado" : (x.IsDeleted) ? "Inactivo" : "Activo"
                })
                .ToListAsync();

            return pensums;
        }

        public async Task<List<PensumOutput>> GetByCareer(int careerId)
        {

            var pensums = await _context.Pensum
                .Include(x => x.Career)
                    .ThenInclude(x => x.Institution)
                .Where(x => x.CarrerId == careerId && x.IsApproved && !x.IsDeleted)
                .Select(x => new PensumOutput
                {
                    Id = x.Id,
                    Name = x.Name,
                    CarrerId = x.Career.Id,
                    CareerName = x.Career.Name,
                    InstitutionName = x.Career.Institution.Name,
                    CreationDate = x.CreationDate,
                    Path = x.Path,
                    CreditLimitPerPeriod = x.CreditLimitPerPeriod
                })
                .ToListAsync();

            return pensums;
        }

        public async Task<PensumOutput> Update(PensumInsertOrUpdateInput input)
        {
            IDbContextTransaction transaction = null;

            try
            {
                ValidatePensumInput(input);
                var currentSessionUser = _userContext.User;

                var currentUser = _context.User.SingleOrDefault(user => user.Id == currentSessionUser.Id);
                var career = _context.Career.SingleOrDefault(x => x.Id == input.CareerId);

                if (career is null)
                    throw new MessageException(4, nameof(input.CareerId), input.CareerId.ToString());

                if (currentUser is null)
                    throw new MessageException(2);

                var existingPensum = _context.Pensum.SingleOrDefault(x => x.Id == input.Id);

                if (existingPensum is null)
                    throw new MessageException(6, "pensum");

                transaction = _context.Database.BeginTransaction();

                var pensumPath = string.Empty;
                if (input.Pensum != null)
                    pensumPath = await _cloudService.SaveAttachment(input.Pensum);

                existingPensum.Name = input.Name;
                existingPensum.CarrerId = input.CareerId;
                existingPensum.CreditLimitPerPeriod = input.CreditLimitPerPeriod;

                if (!string.IsNullOrWhiteSpace(pensumPath))
                    existingPensum.Path = pensumPath;

                if (!career.IsPensumAvailable)
                    career.IsPensumAvailable = true;

                await _context.SaveChangesAsync();

                transaction.Commit();

                return new PensumOutput
                {
                    Id = existingPensum.Id,
                    Name = existingPensum.Name,
                    Path = existingPensum.Path,
                    CreationDate = existingPensum.CreationDate
                };
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

        private void ValidatePensumInput(PensumInsertOrUpdateInput input)
        {
            var message = string.Empty;

            if (string.IsNullOrEmpty(input.Name))
                message = nameof(input.Name);

            if (input.CareerId == 0)
                message = nameof(input.CareerId);

            if (input.CreditLimitPerPeriod <= 0)
                message = nameof(input.CreditLimitPerPeriod);

            if (!string.IsNullOrEmpty(message))
                throw new MessageException(1, message);
        }

        private PensumOutput Map(Pensum pensum)
        {
            return new PensumOutput
            {
                Id = pensum.Id,
                Name = pensum.Name,
                Path = pensum.Path,
                CarrerId = pensum.CarrerId,
                CreationDate = pensum.CreationDate
            };
        }

        public async Task<List<SubjectOutput>> GetSubjects(int pensumId)
        {

            var subjects = await _context.Subject
                .Where(x => x.Pensumid == pensumId)
                .Select(x => new SubjectOutput
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                    Corequisite = x.Corequisite,
                    Prerequisite = x.Prerequisite,
                    Period = x.Period,
                    Credits = x.Credits
                })
                .ToListAsync();

            return subjects;
        }

        public async Task<List<CourseOutput>> GetUserSubjects(int userCareerId)
        {
            var currentDate = DateTime.UtcNow.Date;
            var academicPeriod = _context.AcademicPeriods.Where(x => x.UserCareerId == userCareerId && x.InitialDate <= currentDate && x.EndDate >= currentDate).FirstOrDefault();
            List<CourseOutput> subjects = null;
            if (academicPeriod != null)
                subjects = await _context.AcademicPeriodCourse
                    .Include(x => x.Course)
                    .Where(x => x.AcademicPeriodId == academicPeriod.Id && !x.Course.IsDeleted)
                    .Select(x => new CourseOutput
                    {
                        Id = x.Course.Id,
                        Name = x.Course.Name,
                        Color = x.Course.Color,
                        TeacherFullName = x.Course.TeacherFullName
                    })
                    .ToListAsync();

            return subjects;
        }

        public async Task InsertSubject(SubjectInsertOrUpdateInput input)
        {
            IDbContextTransaction transaction = null;

            try
            {
                ValidateSubjectInput(input);

                var currentSessionUser = _userContext.User;

                var currentUser = _context.User.SingleOrDefault(user => user.Id == currentSessionUser.Id);
                var pensum = _context.Pensum.SingleOrDefault(x => x.Id == input.Pensumid);

                if (pensum is null)
                    throw new MessageException(4, nameof(input.Pensumid), input.Pensumid.ToString());

                if (currentUser is null)
                    throw new MessageException(2);

                transaction = _context.Database.BeginTransaction();

                _context.Subject.Add(new Subject 
                { 
                    Pensumid = input.Pensumid,
                    Code = input.Code,
                    Name = input.Name,
                    Period = input.Period,
                    Prerequisite = input.Prerequisite,
                    Corequisite = input.Corequisite,
                    Credits = input.Credits,
                    Creationdate = DateTime.UtcNow
                });


                await _context.SaveChangesAsync();

                transaction.Commit();

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

        public async Task UpdateSubject(SubjectInsertOrUpdateInput input)
        {
            IDbContextTransaction transaction = null;

            try
            {
                ValidateSubjectInput(input);

                var currentSessionUser = _userContext.User;

                var currentUser = await _context.User.SingleOrDefaultAsync(user => user.Id == currentSessionUser.Id);
                var pensum = await _context.Pensum.SingleOrDefaultAsync(x => x.Id == input.Pensumid);

                if (pensum is null)
                    throw new MessageException(4, nameof(input.Pensumid), input.Pensumid.ToString());

                if (currentUser is null)
                    throw new MessageException(2);


                var existingSubject= _context.Subject.SingleOrDefault(x => x.Id == input.Id);

                if (existingSubject is null)
                    throw new MessageException(6, "subject");

                transaction = _context.Database.BeginTransaction();


                existingSubject.Code = input.Code;
                existingSubject.Name = input.Name;
                existingSubject.Period = input.Period;
                existingSubject.Prerequisite = input.Prerequisite;
                existingSubject.Corequisite = input.Corequisite;
                existingSubject.Credits = input.Credits;

                await _context.SaveChangesAsync();

                transaction.Commit();

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

        public async Task<List<DTOs.Pensum.Builder.Period>> GenerateAcademicPlan(int pensumId, List<string> completedSubjects)
        {
            var pensumInDb = await _context.Pensum.AsNoTracking().SingleOrDefaultAsync(x => x.Id == pensumId);
            var subjectsInDb = await _context.Subject.AsNoTracking().Where(x => x.Pensumid == pensumId).ToListAsync();

            var builderData = subjectsInDb.Select(x => new DTOs.Pensum.Builder.Subject
            {
                Code = x.Code,
                Name = x.Name,
                Credits = x.Credits,
                IsCompleted = false,
                Period = x.Period,
                Prerequisites = new List<DTOs.Pensum.Builder.Subject>(),
                Corequisites = new List<DTOs.Pensum.Builder.Subject>(),
                Prerequisites2 = new List<string>(),
                Corequisites2 = new List<string>()
            }).ToList();

            foreach(var item in subjectsInDb)
            {
                if (!string.IsNullOrWhiteSpace(item.Prerequisite))
                {
                    var prerequisites = item.Prerequisite.Split(',').Select(x => x.Trim());
                    var target = builderData.SingleOrDefault(x => x.Code == item.Code);
                    var data = builderData.Where(x => prerequisites.Contains(x.Code)).ToList();

                    target.Prerequisites.AddRange(data);
                    target.Prerequisites2.AddRange(data.Select(x => x.Code).ToList());
                }

                if (!string.IsNullOrWhiteSpace(item.Corequisite))
                {
                    var corequisites = item.Corequisite.Split(',').Select(x => x.Trim());
                    var target = builderData.SingleOrDefault(x => x.Code == item.Code);

                    var data = builderData.Where(x => corequisites.Contains(x.Code)).ToList();

                    target.Corequisites.AddRange(data);
                    target.Corequisites2.AddRange(data.Select(x => x.Code).ToList());
                }
            }

            if (completedSubjects != null && completedSubjects.Count > 0)
            {
                var completed = builderData.Where(x => completedSubjects.Contains(x.Code)).ToList();
                foreach (var item in completed)
                    item.IsCompleted = true;
            }

            var builder = new PensumBuilder();
            var result = builder.Build(builderData, pensumInDb.CreditLimitPerPeriod);

            foreach(var period in result)
            {
                foreach (var subject in period.Subjects)
                {
                    subject.Corequisites.Clear();
                    subject.Prerequisites.Clear();
                }
            }

            return result;
        }

        public async Task DeleteSubject(int subjectId)
        {
            var existingSubject = await _context.Subject.SingleOrDefaultAsync(x => x.Id == subjectId);

            if (existingSubject is null)
                throw new MessageException(6, "subject");

            _context.Remove(existingSubject);

            await _context.SaveChangesAsync();
        }

        private void ValidateSubjectInput(SubjectInsertOrUpdateInput input)
        {
            var message = string.Empty;

            if (string.IsNullOrEmpty(input.Name))
                message = nameof(input.Name);
            else if (string.IsNullOrEmpty(input.Code))
                message = nameof(input.Code);
            else if (string.IsNullOrEmpty(input.Period))
                message = nameof(input.Period);
            else if (input.Pensumid == 0)
                message = nameof(input.Pensumid);
            else if (input.Credits < 0)
                message = nameof(input.Credits);

            if (!string.IsNullOrEmpty(message))
                throw new MessageException(1, message);
        }
    }
}

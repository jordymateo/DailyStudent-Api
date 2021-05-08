using DailyStudent.Api.Constants;
using DailyStudent.Api.DataAccess;
using DailyStudent.Api.DTOs.Career;
using DailyStudent.Api.DTOs.Course;
using DailyStudent.Api.Exceptions;
using DailyStudent.Api.Services.Cloud;
using DailyStudent.Api.Services.Security.UserContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.Services.Careers
{
    public class CareersService: ICareersService
    {

        private readonly DailyStudentDbContext _context;
        private readonly IUserContext _userContext;
        private readonly IGoogleCloudService _cloudService;

        public CareersService(DailyStudentDbContext context, IUserContext userContext, IGoogleCloudService cloudService)
        {
            _context = context;
            _userContext = userContext;
            _cloudService = cloudService;
        }

        public async Task<CareerOutput> Insert(CareerInsertOrUpdateInput input)
        {
            IDbContextTransaction transaction = null;

            try
            {
                ValidateCareerInput(input);

                var currentSessionUser = _userContext.User;

                var currentUser = _context.User.SingleOrDefault(user => user.Id == currentSessionUser.Id);
                var institution = _context.Institution.SingleOrDefault(x => x.Id == input.InstitutionId);

                if (institution is null)
                    throw new MessageException(4, nameof(input.InstitutionId), input.InstitutionId.ToString());

                if (currentUser is null)
                    throw new MessageException(2);

                transaction = _context.Database.BeginTransaction();

                Career newCareer;
                if (currentUser.UserRolId == UserRoles.Administrator)
                {
                    newCareer = new Career
                    {
                        Name = input.Name,
                        InstitutionId = input.InstitutionId,
                        CreatorUser = currentUser,
                        ApproverUser = currentUser,
                        CreationDate = DateTime.UtcNow,
                        ApprovalDate = DateTime.UtcNow
                    };


                    _context.Career.Add(newCareer);

                    if (input.Pensum != null)
                    {
                        var pensumPath = await _cloudService.SaveAttachment(input.Pensum);

                        await _context.SaveChangesAsync();

                        newCareer.IsPensumAvailable = true;
                        _context.Pensum.Add(new Pensum
                        {
                            Name = "Pensum " + input.Name,
                            Path = pensumPath,
                            CarrerId = newCareer.Id,
                            CreatorUser = currentUser,
                            ApproverUser = currentUser,
                            CreationDate = DateTime.UtcNow,
                            ApprovalDate = DateTime.UtcNow,
                            IsApproved = true,
                            IsDeleted = false,
                            DeletionDate = null
                        });
                    }
                }
                else
                {
                    newCareer = new Career
                    {
                        Name = input.Name,
                        InstitutionId = input.InstitutionId,
                        CreatorUser = currentUser,
                        CreationDate = DateTime.UtcNow
                    };

                    _context.Career.Add(newCareer);
                    if (input.Pensum != null)
                    {
                        var pensumPath = await _cloudService.SaveAttachment(input.Pensum);

                        await _context.SaveChangesAsync();

                        newCareer.IsPensumAvailable = true;
                        _context.Pensum.Add(new Pensum
                        {
                            Name = "Pensum " + input.Name,
                            Path = pensumPath,
                            CarrerId = newCareer.Id,
                            CreatorUser = currentUser,
                            CreationDate = DateTime.UtcNow,
                            IsApproved = false,
                            IsDeleted = false,
                            DeletionDate = null
                        });
                    }
                }

                await _context.SaveChangesAsync();

                transaction.Commit();


                return new CareerOutput()
                {
                    Id = newCareer.Id,
                    Name = newCareer.Name,
                    InstitutionId = newCareer.InstitutionId,
                    IsPensumAvailable = newCareer.IsPensumAvailable
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

        public async Task<UserCareerOutput> InsertUserCareer(UserCareerInput input)
        {
            IDbContextTransaction transaction = null;

            try
            {
                //ValidateCareerInput(input);

                var currentSessionUser = _userContext.User;

                var currentUser = _context.User.SingleOrDefault(user => user.Id == currentSessionUser.Id);

                if (currentUser is null)
                    throw new MessageException(2);


                var institution = _context.Institution.SingleOrDefault(x => x.Id == input.InstitutionId);

                if (institution is null)
                    throw new MessageException(4, nameof(input.InstitutionId), input.InstitutionId.ToString());

                var career = _context.Career.SingleOrDefault(x => x.Id == input.CareerId);

                if (career is null)
                    throw new MessageException(4, nameof(input.CareerId), input.CareerId.ToString());


                var pensum = _context.Pensum.SingleOrDefault(x => x.Id == input.PensumId);

                if (pensum is null)
                    throw new MessageException(4, nameof(input.PensumId), input.PensumId.ToString());


                transaction = _context.Database.BeginTransaction();


                var institutionUser = _context.InstitutionUser.SingleOrDefault(x => x.InstitutionId == input.InstitutionId && x.UserId == currentSessionUser.Id);

                if (institutionUser == null)
                {
                    institutionUser = new InstitutionUser
                    {
                        InstitutionId = input.InstitutionId,
                        UserId = currentSessionUser.Id,
                        DeletionDate = null,
                        CreationDate = DateTime.UtcNow,
                        Isdeleted = false
                    };

                    _context.InstitutionUser.Add(institutionUser);

                    await _context.SaveChangesAsync();
                }
                var newUserCareer = new UserCareer
                {
                    InstitutionUserId = institutionUser.Id,
                    PensumId = input.PensumId,
                    Careerid = input.CareerId,
                    CreationDate = DateTime.UtcNow,
                    IsDeleted = false,
                    DeletionDate = null
                };
                _context.UserCareer.Add(newUserCareer);

                await _context.SaveChangesAsync();
                transaction.Commit();

                return new UserCareerOutput
                {
                    Id = newUserCareer.Id,
                    PensumId = newUserCareer.PensumId
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

        public async Task InsertUserPeriod(UserPeriodInsertOrUpdateInput input)
        {
            IDbContextTransaction transaction = null;

            try
            {
                //ValidateCareerInput(input);

                var currentSessionUser = _userContext.User;

                var currentUser = _context.User.SingleOrDefault(user => user.Id == currentSessionUser.Id);

                if (currentUser is null)
                    throw new MessageException(2);


                var userCareer = _context.UserCareer.SingleOrDefault(x => x.Id == input.UserCareerId);

                if (userCareer is null)
                    throw new MessageException(4, nameof(input.UserCareerId), input.UserCareerId.ToString());

                input.EndDate = input.EndDate.Date;
                input.InitialDate = input.InitialDate.Date;

                if (_context.AcademicPeriods.Any(x => x.UserCareerId == input.UserCareerId && x.EndDate > input.InitialDate))
                    throw new MessageException(9, input.InitialDate.ToString("yyyy-MM-dd"));

                transaction = _context.Database.BeginTransaction();

                var newAcademicPeriod = new AcademicPeriod
                {
                    UserCareerId = input.UserCareerId,
                    Name = input.Name,
                    Number = input.Number,
                    InitialDate = input.InitialDate,
                    EndDate = input.EndDate,
                    DeletionDate = null,
                    IsDeleted = false,
                    CreationDate = DateTime.UtcNow
                };

                _context.AcademicPeriods.Add(newAcademicPeriod);
                await _context.SaveChangesAsync();

                foreach (var subject in input.Subjects)
                {
                    var newSubject = new DataAccess.Course
                    {
                        Color = (subject.Color.StartsWith('#')) ? subject.Color.Substring(1, subject.Color.Length - 1) : subject.Color,
                        Name = subject.Name,
                        TeacherFullName = subject.Teacher,
                        CreationDate = DateTime.UtcNow,
                        IsDeleted = false,
                        DeletionDate = null,
                        InstitutionUserId = userCareer.InstitutionUserId,
                        CourseTypeId = "career"
                    };
                    _context.Course.Add(newSubject);
                    await _context.SaveChangesAsync();

                    _context.AcademicPeriodCourse.Add(new AcademicPeriodCourse 
                    { 
                        AcademicPeriodId = newAcademicPeriod.Id,
                        CourseId = newSubject.Id
                    });
                }

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


        public async Task InsertPeriodSubject(CourseInsertOrUpdateInput input)
        {
            IDbContextTransaction transaction = null;

            try
            {
                ValidatePeriodSubjectInput(input);

                var currentSessionUser = _userContext.User;

                var currentUser = _context.User.SingleOrDefault(user => user.Id == currentSessionUser.Id);

                if (currentUser is null)
                    throw new MessageException(2);


                var academicPeriod = _context.AcademicPeriods
                    .Include(x => x.UserCareer)
                    .SingleOrDefault(x => x.Id == input.AcademicPeriodId);

                if (academicPeriod is null)
                    throw new MessageException(4, nameof(input.AcademicPeriodId), input.AcademicPeriodId.ToString());

                transaction = _context.Database.BeginTransaction();

                input.Color = (input.Color.StartsWith('#')) ? input.Color.Substring(1, input.Color.Length - 1) : input.Color;
                var newCourse = new DataAccess.Course
                {
                    Name = input.Name,
                    Color = input.Color,
                    TeacherFullName = input.TeacherFullName,
                    InstitutionUserId = academicPeriod.UserCareer.InstitutionUserId,
                    IsDeleted = false,
                    CourseTypeId = "career",
                    CreationDate = DateTime.UtcNow,
                };

                _context.Course.Add(newCourse);
                await _context.SaveChangesAsync();

                _context.AcademicPeriodCourse.Add(new AcademicPeriodCourse
                {
                    CourseId = newCourse.Id,
                    AcademicPeriodId = input.AcademicPeriodId
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



        public async Task UpdateUserPeriod(UserPeriodInsertOrUpdateInput input)
        {
            IDbContextTransaction transaction = null;

            try
            { 
                var currentSessionUser = _userContext.User;

                var currentUser = _context.User.SingleOrDefault(user => user.Id == currentSessionUser.Id);

                if (currentUser is null)
                    throw new MessageException(2);


                var existingPeriod = _context.AcademicPeriods.SingleOrDefault(x => x.Id == input.Id);

                if (existingPeriod is null)
                    throw new MessageException(6, "periodo");
                
                input.InitialDate= input.InitialDate.Date;
                input.EndDate = input.EndDate.Date;

                if (_context.AcademicPeriods.Any(x => x.Id != input.Id && x.UserCareerId == input.UserCareerId 
                && ((x.InitialDate < input.InitialDate && input.InitialDate < x.EndDate) || (x.InitialDate < input.EndDate && input.EndDate < x.EndDate)) ))
                    throw new MessageException(9, "seleccionada");

                transaction = _context.Database.BeginTransaction();

                existingPeriod.Name = input.Name;
                existingPeriod.Number = input.Number;
                existingPeriod.EndDate = input.EndDate.Date;
                existingPeriod.InitialDate = input.InitialDate.Date;

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

        public async Task Approve(int careerId)
        {
            var career = await _context.Career.SingleOrDefaultAsync(x => x.Id == careerId);

            if (career == null)
                throw new MessageException(4, nameof(careerId), careerId.ToString());

            career.ApprovalDate = DateTime.UtcNow;
            career.ApproverUserId = _userContext.User.Id;

            await _context.SaveChangesAsync();
        }

        public async Task<CareerOutput> Get(int careerId)
        {
            var career = await _context.Career.SingleOrDefaultAsync(x => x.Id == careerId);

            if (career is null)
                return null;

            return Map(career);
        }

        //public async Task<dynamic> GetUserCareers()
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

        public async Task<List<CareerOutput>> GetAll()
        {

            var institutions = await _context.Career
                .Include(x => x.Institution)
                .Select(x => new CareerOutput
                {
                    Id = x.Id,
                    Name = x.Name,
                    InstitutionId = x.InstitutionId,
                    InstitutionName = x.Institution.Name,
                    IsPensumAvailable = x.IsPensumAvailable,
                    CreationDate = x.CreationDate,
                    State = x.ApproverUserId != null ? "Aprobado" : "Solicitado"
                })
                .ToListAsync();

            return institutions;
        }
        public async Task<List<CareerOutput>> GetByInstitution(int institutionId)
        {

            var institutions = await _context.Career
                .Where(x => x.InstitutionId == institutionId && x.ApproverUserId != null)
                .Select(x => new CareerOutput
                {
                    Id = x.Id,
                    Name = x.Name,
                    InstitutionId = x.InstitutionId,
                    InstitutionName = x.Institution.Name,
                    IsPensumAvailable = x.IsPensumAvailable,
                    CreationDate = x.CreationDate
                })
                .ToListAsync();

            return institutions;
        }

        public async Task<dynamic> GetUserPeriods(int userCareerId)
        {

            var periods = await _context.AcademicPeriods
                .Include(x => x.AcademicPeriodCourse)
                    .ThenInclude(x => x.Course)
                .Where(x => x.UserCareerId == userCareerId && !x.IsDeleted)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Number,
                    x.InitialDate,
                    x.EndDate,
                    x.UserCareerId,
                    Subjects = x.AcademicPeriodCourse
                        .Select(y => new 
                        { 
                            y.Course.Id,
                            y.Course.Name,
                            y.Course.TeacherFullName,
                            y.Course.Color,
                            y.Course.IsDeleted
                        })
                        .ToList()
                })
                .ToListAsync();

            return periods;
        }


        public async Task<CareerOutput> Update(CareerInsertOrUpdateInput input)
        {
            IDbContextTransaction transaction = null;

            try
            {
                ValidateCareerInput(input);
                var currentSessionUser = _userContext.User;

                var currentUser = _context.User.SingleOrDefault(user => user.Id == currentSessionUser.Id);
                var institution = _context.Institution.SingleOrDefault(x => x.Id == input.InstitutionId);

                if (institution is null)
                    throw new MessageException(4, nameof(input.InstitutionId), input.InstitutionId.ToString());

                if (currentUser is null)
                    throw new MessageException(2);


                var existingCareer = _context.Career.SingleOrDefault(x => x.Id == input.Id);

                if (existingCareer is null)
                    throw new MessageException(6, "carrera");

                existingCareer.Name = input.Name;
                existingCareer.InstitutionId = input.InstitutionId;


                transaction = _context.Database.BeginTransaction();

                await _context.SaveChangesAsync();

                transaction.Commit();

                return Map(existingCareer);
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

        private void ValidateCareerInput(CareerInsertOrUpdateInput input)
        {
            var message = string.Empty;

            if (string.IsNullOrEmpty(input.Name))
                message = nameof(input.Name);
            else if (input.InstitutionId == 0)
                message = nameof(input.InstitutionId);

            if (!string.IsNullOrEmpty(message))
                throw new MessageException(1, message);
        }

        private void ValidatePeriodSubjectInput(CourseInsertOrUpdateInput input)
        {
            var message = string.Empty;

            if (string.IsNullOrEmpty(input.Name))
                message = nameof(input.Name);
            else if (input.AcademicPeriodId == 0)
                message = nameof(input.AcademicPeriodId);
            else if (string.IsNullOrEmpty(input.TeacherFullName))
                message = nameof(input.TeacherFullName);
            else if (string.IsNullOrEmpty(input.Color))
                message = nameof(input.Color);

            if (!string.IsNullOrEmpty(message))
                throw new MessageException(1, message);
        }

        private CareerOutput Map(Career career)
        {
            return new CareerOutput
            {
                Id = career.Id,
                Name = career.Name,
                InstitutionId = career.InstitutionId,
                IsPensumAvailable = career.IsPensumAvailable
            };
        }
    }
}

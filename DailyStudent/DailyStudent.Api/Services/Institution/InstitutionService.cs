using DailyStudent.Api.DataAccess;
using DailyStudent.Api.DTOs.Institution;
using DA = DailyStudent.Api.DataAccess;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DailyStudent.Api.Exceptions;
using DailyStudent.Api.Services.Security.UserContext;
using DailyStudent.Api.Constants;
using DailyStudent.Api.Services.Cloud;
using DailyStudent.Api.DTOs;

namespace DailyStudent.Api.Services.Institution
{
    public class InstitutionService : IInstitutionService
    {
        private readonly DailyStudentDbContext _context;
        private readonly IUserContext _userContext;
        private readonly IGoogleCloudService _cloudService;

        public InstitutionService(DailyStudentDbContext context, IUserContext userContext, IGoogleCloudService cloudService)
        {
            _context = context;
            _userContext = userContext;
            _cloudService = cloudService;
        }

        public async Task<InstitutionOutput> Insert(InstitutionInsertOrUpdateInput input)
        {
            IDbContextTransaction transaction = null;

            try
            {
                ValidateInstitutionInput(input);

                var currentSessionUser = _userContext.User;

                var currentUser = _context.User.SingleOrDefault(user => user.Id == currentSessionUser.Id);
                var country = _context.Country.SingleOrDefault(country => country.Id == input.CountryId);
                
                if (country is null)
                    throw new MessageException(3, input.CountryId.ToString());

                if (currentUser is null)
                    throw new MessageException(2);

                var logoPath = string.Empty;
                if (input.Logo != null)
                    logoPath = await _cloudService.SaveAttachment(input.Logo, input.Acronym.Trim());

                DA.Institution newInstitution;
                if (currentUser.UserRolId == UserRoles.Administrator)
                {
                    newInstitution = new DA.Institution()
                    {
                        Name = input.Name,
                        Acronym = input.Acronym,
                        LogoPath = logoPath,
                        Website = input.Website,
                        Country = country,
                        IsAvailable = true,
                        IsDeleted = false,
                        CreatorUser = currentUser,
                        ApproverUser = currentUser,
                        CreationDate = DateTime.UtcNow,
                        ApprovalDate = DateTime.UtcNow
                    };
                } else
                {
                    newInstitution = new DA.Institution()
                    {
                        Name = input.Name,
                        Acronym = input.Acronym,
                        LogoPath = logoPath,
                        Website = input.Website,
                        Country = country,
                        IsAvailable = false,
                        IsDeleted = false,
                        CreatorUser = currentUser,
                        CreationDate = DateTime.UtcNow
                    };
                }

                

                transaction = _context.Database.BeginTransaction();

                _context.Institution.Add(newInstitution);

                await _context.SaveChangesAsync();

                transaction.Commit();


                return new InstitutionOutput()
                {
                    Id = newInstitution.Id,
                    Name = input.Name,
                    Acronym = input.Acronym,
                    LogoPath = logoPath,
                    Country = country.Name,
                    IsAvailable = input.IsAvailable,
                    Website = input.Website
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

        public async Task ToggleState(int institutionId)
        {
            var institution = await _context.Institution.SingleOrDefaultAsync(x => x.Id == institutionId);

            if (institution == null)
                throw new MessageException(4, nameof(institutionId), institutionId.ToString());

            institution.IsDeleted = !institution.IsDeleted;

            await _context.SaveChangesAsync();
        }

        public async Task ToggleUserCareerState(int userCareerId)
        {
            var userCareer = await _context.UserCareer.SingleOrDefaultAsync(x => x.Id == userCareerId);

            if (userCareer == null)
                throw new MessageException(4, nameof(userCareerId), userCareerId.ToString());

            userCareer.IsDeleted = !userCareer.IsDeleted;

            if (userCareer.IsDeleted)
                userCareer.DeletionDate = DateTime.UtcNow;
            else
                userCareer.DeletionDate = null;

            await _context.SaveChangesAsync();
        }

        public async Task ToggleUserCourseState(int userCourseId)
        {
            var userCourse = await _context.Course.SingleOrDefaultAsync(x => x.Id == userCourseId);

            if (userCourse == null)
                throw new MessageException(4, nameof(userCourse), userCourse.ToString());

            userCourse.IsDeleted = !userCourse.IsDeleted;

            if (userCourse.IsDeleted)
                userCourse.DeletionDate = DateTime.UtcNow;
            else
                userCourse.DeletionDate = null;

            await _context.SaveChangesAsync();
        }

        public async Task Approve(int institutionId)
        {
            var institution = await _context.Institution.SingleOrDefaultAsync(x => x.Id == institutionId);

            if (institution == null)
                throw new MessageException(4, nameof(institutionId), institutionId.ToString());

            institution.IsAvailable = true;
            institution.ApprovalDate = DateTime.UtcNow;
            institution.ApproverUserId = _userContext.User.Id;

            await _context.SaveChangesAsync();
        }

        public async Task<InstitutionOutput> Get(int institutionId)
        {
            var institution = await _context.Institution.SingleOrDefaultAsync(institution => institution.Id == institutionId 
                && !institution.IsDeleted);

            if (institution is null)
                return null;

            return Map(institution);
        }

        public async Task<dynamic> GetUserInstitutions()
        {
            var institution = await _context.InstitutionUser.Include(x => x.Institution)
                                        .Where(x => x.UserId == _userContext.User.Id)
                                        .Select(x => new
                                        {
                                            UId = x.Institution + x.Institution.Name,
                                            x.Institution.Id,
                                            x.Institution.Name,
                                            x.Institution.LogoPath,
                                            Items = new [] 
                                            {
                                                new {
                                                    UId = Guid.NewGuid().ToString(),
                                                    Name = "Cursos",
                                                    IsGroup = true,
                                                    Items = _context.Course
                                                        .Where(course => course.InstitutionUserId == x.Id && !course.IsDeleted && course.CourseTypeId == "course")
                                                        .Select(y => new
                                                        {
                                                            UId = y.Id + y.Name,
                                                            y.Id,
                                                            y.Name,
                                                            y.CourseTypeId,
                                                            InstitutionName = x.Institution.Name,
                                                            InstitutionLogo = x.Institution.LogoPath
                                                        })
                                                        .ToList()
                                                }, new {
                                                    UId = Guid.NewGuid().ToString() + "1",
                                                    Name = "Carreras",
                                                    IsGroup = true,
                                                    Items = _context.UserCareer
                                                        .Include(x => x.Career)
                                                        .Where(course => course.InstitutionUserId == x.Id && !course.IsDeleted)
                                                        .Select(y => new
                                                        {
                                                            UId = y.Id + y.Career.Name,
                                                            y.Id,
                                                            y.Career.Name,
                                                            CourseTypeId = "career",
                                                            InstitutionName = x.Institution.Name,
                                                            InstitutionLogo = x.Institution.LogoPath
                                                        })
                                                        .ToList()
                                                } 
                                            }
                                        })
                                        .ToListAsync();

            return institution;
        }

        public async Task<dynamic> GetUserInstitutionsInLine()
        {
            var userId = _userContext.User.Id;

            List<CourseOrCareerOutput> data = await _context.Course
                .Include(x => x.InstitutionUser)
                    .ThenInclude(x => x.Institution)
                .Where(x => x.CourseTypeId == "course" && x.InstitutionUser.UserId == userId)
                .Select(y => new CourseOrCareerOutput
                {
                    Id = y.Id,
                    Name = y.Name,
                    CourseTypeId = y.CourseTypeId,
                    CreationDate = y.CreationDate,
                    InstitutionName = y.InstitutionUser.Institution.Name,
                    InstitutionAcronym = y.InstitutionUser.Institution.Acronym,
                    InstitutionLogo = y.InstitutionUser.Institution.LogoPath,
                    IsDeleted = y.IsDeleted
                })
                .ToListAsync();

            data.AddRange(_context.UserCareer
                                    .Include(x => x.InstitutionUser)
                                    .Include(x => x.Career)
                                        .ThenInclude(x => x.Institution)
                                    .Where(x => x.InstitutionUser.UserId == userId)
                                    .Select(y => new CourseOrCareerOutput
                                    {
                                        Id = y.Id,
                                        Name = y.Career.Name,
                                        CourseTypeId = "career",
                                        CreationDate = y.CreationDate,
                                        InstitutionName = y.Career.Institution.Name,
                                        InstitutionAcronym = y.Career.Institution.Acronym,
                                        InstitutionLogo = y.Career.Institution.LogoPath,
                                        IsDeleted = y.IsDeleted
                                    })
                                    .ToList());

            return data.OrderByDescending(x => x.CreationDate).ToList();
        }

        public async Task<dynamic> GetUserCareers()
        {
            var institution = await _context.InstitutionUser.Include(x => x.Institution)
                                        .Where(x => x.UserId == _userContext.User.Id)
                                        .Select(x => new
                                        {
                                            UId = x.Institution + x.Institution.Name,
                                            x.Institution.Id,
                                            x.Institution.Name,
                                            x.Institution.LogoPath,
                                            Items = _context.UserCareer
                                                        .Include(x => x.Career)
                                                        .Where(course => course.InstitutionUserId == x.Id && !x.Isdeleted)
                                                        .Select(y => new
                                                        {
                                                            UId = y.Id + y.Career.Name,
                                                            y.Id,
                                                            y.Career.Name,
                                                            CourseTypeId = "career",
                                                            y.PensumId,
                                                            InstitutionName = x.Institution.Name,
                                                            InstitutionLogo = x.Institution.LogoPath
                                                        })
                                                        .ToList()
                                        })
                                        .ToListAsync();

            return institution.Where(x => x.Items.Count > 0).ToList();
        }

        public async Task<List<InstitutionOutput>> GetAll()
        {

            var institutions = await _context.Institution
                .Include(institution => institution.Country)
                .Select(institution => new InstitutionOutput 
                {
                    Id = institution.Id,
                    Name = institution.Name,
                    Acronym = institution.Acronym,
                    Country = institution.Country.Name,
                    CountryId = institution.Country.Id,
                    IsAvailable = institution.IsAvailable,
                    IsDeleted = institution.IsDeleted,
                    LogoPath = institution.LogoPath,
                    Website = institution.Website,
                    State = (!institution.IsAvailable) ? "Solicitado" : (institution.IsDeleted) ? "Inactivo" : "Activo"
                })
                .ToListAsync();

            return institutions;
        }

        public async Task<List<InstitutionOutput>> GetAvailable()
        {

            var institutions = await _context.Institution
                .Include(institution => institution.Country)
                .Where(x => x.IsAvailable && !x.IsDeleted)
                .Select(institution => new InstitutionOutput
                {
                    Id = institution.Id,
                    Name = institution.Name,
                    Acronym = institution.Acronym,
                    Country = institution.Country.Name,
                    CountryId = institution.Country.Id,
                    IsAvailable = institution.IsAvailable,
                    IsDeleted = institution.IsDeleted,
                    LogoPath = institution.LogoPath,
                    Website = institution.Website
                })
                .ToListAsync();

            return institutions;
        }

        public async Task<InstitutionOutput> Update(InstitutionInsertOrUpdateInput input)
        {
            IDbContextTransaction transaction = null;

            try
            {
                ValidateInstitutionInput(input);
                var currentSessionUser = _userContext.User;

                var currentUser = _context.User.SingleOrDefault(user => user.Id == currentSessionUser.Id);
                var country = _context.Country.SingleOrDefault(country => country.Id == input.CountryId);

                if (country is null)
                    throw new MessageException(3, input.CountryId.ToString());

                if (currentUser is null)
                    throw new MessageException(2);


                var logoPath = string.Empty;
                if (input.Logo != null)
                    logoPath = await _cloudService.SaveAttachment(input.Logo, input.Acronym.Trim());

                var existingInstitution = _context.Institution.SingleOrDefault(institution => institution.Id == input.Id);

                if (existingInstitution is null)
                    throw new MessageException(6, "institución académica");

                existingInstitution.Name = input.Name;
                existingInstitution.Acronym = input.Acronym;

                if (!string.IsNullOrWhiteSpace(logoPath))
                    existingInstitution.LogoPath = logoPath;

                existingInstitution.Website = input.Website;
                existingInstitution.Country = country;

                transaction = _context.Database.BeginTransaction();

                await _context.SaveChangesAsync();

                transaction.Commit();

                return new InstitutionOutput()
                {
                    Id = existingInstitution.Id,
                    Name = input.Name,
                    Acronym = input.Acronym,
                    LogoPath = logoPath,
                    Country = country.Name,
                    IsAvailable = input.IsAvailable,
                    Website = input.Website
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

        public void ValidateInstitutionInput(InstitutionInsertOrUpdateInput input)
        {
            var message = string.Empty;

            if (string.IsNullOrEmpty(input.Name))
                message = "El nombre es requerido.";
            else if (string.IsNullOrEmpty(input.Acronym))
                message = "Las siglas son requeridas.";

            if(!string.IsNullOrEmpty(message))
                throw new Exception(message);
        }

        private InstitutionOutput Map(DA.Institution institution)
        {
            return new InstitutionOutput()
            {
                Id = institution.Id,
                Name = institution.Name,
                Acronym = institution.Acronym,
                Country = institution.Country.Name,
                IsAvailable = institution.IsAvailable,
                LogoPath = institution.LogoPath,
                Website = institution.Website
            };
        }
    }
}

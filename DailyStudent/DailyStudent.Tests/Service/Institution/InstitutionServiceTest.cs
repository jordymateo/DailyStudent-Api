using DailyStudent.Api.DTOs.Institution;
using DailyStudent.Api.Exceptions;
using DailyStudent.Api.Services.Cloud;
using DailyStudent.Api.Services.Institution;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DailyStudent.Tests.Service.Institution
{
    public class InstitutionServiceTest: TestBase
    {
        private readonly IInstitutionService _institutionService;
        private readonly IGoogleCloudService _cloudService;

        public InstitutionServiceTest()
        {
            _institutionService = new InstitutionService(_context, _dUserContext, _cloudService);
        }

        [Fact]
        public async void Insert_CompleteInformation_InstitutionCreated()
        {
            //arrange
            var institution = new InstitutionInsertOrUpdateInput 
            {
                Name = "Institución primogénita de Acción Pro Educación y Cultura",
                Acronym = "APEC",
                Website = "apec.edu.do",
                CountryId = 1,
                IsAvailable = true
            };

            //act
            var actual = await _institutionService.Insert(institution);
            
            //assert
            Assert.NotNull(actual);
            Assert.IsType<InstitutionOutput>(actual);
        }
        
        [Fact]
        public async void Insert_InputandOutput_NotSameType()
        {
            //arrange
            var institution = new InstitutionInsertOrUpdateInput
            {
                Name = "Institución primogénita de Acción Pro Educación y Cultura",
                Acronym = "APEC",
                Website = "apec.edu.do",
                CountryId = 1,
                IsAvailable = true
            };

            //act
            var actual = await _institutionService.Insert(institution);

            //assert
            Assert.IsNotType<InstitutionInsertOrUpdateInput>(actual);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Insert_EmptyOrNullName_Throws_Exception(string institutionName)
        {
            //arrange
            var institution = new InstitutionInsertOrUpdateInput
            {
                Name = institutionName,
                Acronym = "APEC",
                Website = "apec.edu.do",
                CountryId = 1,
                IsAvailable = true
            };

            //act
            Task actual() => _institutionService.Insert(institution);

            //assert
            Assert.ThrowsAsync<Exception>(actual);
        }
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Insert_EmptyOrNullInstitutionAcronym_Throws_Exception(string institutionAcronym)
        {
            //arrange
            var institution = new InstitutionInsertOrUpdateInput
            {
                Name = "Institución primogénita de Acción Pro Educación y Cultura",
                Acronym = institutionAcronym,
                Website = "apec.edu.do",
                CountryId = 1,
                IsAvailable = true
            };

            //act
            Task actual() => _institutionService.Insert(institution);

            //assert
            Assert.ThrowsAsync<Exception>(actual);
        }
        [Theory]
        [InlineData(9999)]
        [InlineData(-9)]
        [InlineData(0)]
        public  void Insert_InvalidCountryId_Throws_Exception(short institutionCountry)
        {
            //arrange
            var institution = new InstitutionInsertOrUpdateInput
            {
                Name = "Institución primogénita de Acción Pro Educación y Cultura",
                Acronym = "APEC",
                Website = "apec.edu.do",
                CountryId = institutionCountry,
                IsAvailable = true
            };

            //act
            Func<Task> actual = () => _institutionService.Insert(institution);
            //assert
            Assert.ThrowsAsync<MessageException>(actual);
        }

        [Theory]
        [InlineData(-9)]
        public void Insert_InvalidInstitutionId_Throws_Exception(short institutionId)
        {
            //arrange
            var institution = new InstitutionInsertOrUpdateInput
            {
                Id = institutionId,
                Name = "Institución primogénita de Acción Pro Educación y Cultura",
                Acronym = "APEC",
                Website = "apec.edu.do",
                CountryId = 1,
                IsAvailable = true
            };

            //act
            bool completed = _institutionService.Insert(institution).IsCompleted;
            //assert
            Assert.True(completed);
        }
        [Fact]
        public async void Insert_CorrectInstitutionId_InstitutionCreated()
        {
            //arrange
            var institution = new InstitutionInsertOrUpdateInput
            {
                Id = 3,
                Name = "Apec",
                Acronym = "Apec",
                Website = "apec.edu.do",
                CountryId = 1,
                IsAvailable = true,
            };
            //act
            var actual = await _institutionService.Insert(institution);

            //Assert
            Assert.Equal(institution.Id, actual.Id);
        }

        [Theory]
        [InlineData(2)]
        public async void Get_ExistingId_InstitutionData(int institutionId)
        {
            //arrange
            var institution = new InstitutionInsertOrUpdateInput
            {
                Id = institutionId,
                Name = "Instituto Tecnológico de Santo Domingo",
                Acronym = "Intec",
                Website = "intec.edu.do",
                CountryId = 1,
                IsAvailable = true
            };
            //act
            var actual = await _institutionService.Get(institutionId);
            //Assert
            Assert.Equal(institution.Id, actual.Id);
        }

        [Fact]
        public void Update_CompleteInformation_InstitutionModified()
        {
            //arrange
            var institution = new InstitutionInsertOrUpdateInput
            {
                   Name = "Instituto Tecnologico de Santo Domingo",
                   Acronym = "Intec",
                   Website = "intec.edu.do",
                   CountryId = 2,
                   IsAvailable = true,
            };
            //act
            bool UpdateCompleted = _institutionService.Update(institution).IsCompleted;
            //Assert
            Assert.True(UpdateCompleted);
        }

        [Fact]
        public void Update_ProcessCompleted_Thrown_Exception()
        {
            //arrange
            var institution = new InstitutionInsertOrUpdateInput
            {
                Id=2,
                Name = "Instituto Tecnologico de Santo Domingo",
                Acronym = "Intec",
                Website = "intec.edu.do",
                CountryId = 2,
                IsAvailable = true,
            };
            //act
            Task actual() => _institutionService.Update(institution);
            //Assert
            Assert.ThrowsAsync<Exception>(actual);
        }

        //[Theory]
        //[InlineData(1, true)]
        //[InlineData(2, true)]
        ////[InlineData(3, false)]
        ////[InlineData(-4, false)]
        ////[InlineData(1000, false)]
        //public async void Delete_ValidateIfInstitutionExists_InstitutionDeletedorNotDeleted(int institutionId, bool expected)
        //{
        //    //arrange
        //    var institution = new InstitutionInsertOrUpdateInput
        //    {
        //        Id = institutionId,
        //        Name = "Instituto Tecnologico de Santo Domingo",
        //        Acronym = "Intec",
        //        Website = "intec.edu.do",
        //        CountryId = 2,
        //        IsAvailable = true,
        //    };
        //    //actual
        //    await _institutionService.Delete(institution.Id);
        //    var actual = await _context.Institution.SingleOrDefaultAsync(institution => institution.Id == institutionId);
        //    //Assert
        //    Assert.True(actual.IsDeleted);
        //}

    }
}

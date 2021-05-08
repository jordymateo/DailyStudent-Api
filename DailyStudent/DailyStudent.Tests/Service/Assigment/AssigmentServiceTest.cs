using System;
using System.Collections.Generic;
using DailyStudent.Api.Services.Assignment;
using DailyStudent.Api.DTOs.Assignment;
using System.Text;
using DailyStudent.Api.Services.Cloud;
using Xunit;
using System.Threading.Tasks;
using DailyStudent.Api.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DailyStudent.Tests.Service.Assigment
{
    public class AssigmentServiceTest : TestBase
    {
        private readonly IAssignmentService _assigmentService;
        private readonly IGoogleCloudService _cloudService;

        public AssigmentServiceTest()
        {
            _assigmentService = new AssignmentService(_context,  _cloudService, _dUserContext);
        }
        //Methods to test: Insert(), Update(), Delete(), Get(), GetAll() and GetByCourse
        [Fact]
        public async void Insert_CompleteInformation_AssigmentCreated()
        {
            var assig = new AssignmentInsertOrUpdateInput
            {
                Id = 1,
                Title = "Crear grafico predictivo de los datos",
                Descripcion = "Mediante la herramienta SAS realizar un analisis predictivo y mostrar su gráfica",
                DueDate = DateTime.Parse("1-03-2021"),
                IsIndividual = true,
                IsCompleted = false,
                CourseId = 1
            };

            var actual = await _assigmentService.Insert(assig);

            //Assert.ThrowsAsync<Exception>(actual);
            Assert.NotNull(actual);
            Assert.IsType<AssignmentOutput>(actual); // Assigment created
        }
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Insert_EmptyOrNullName_ThrowsException(string _title)
        {
            var assig = new AssignmentInsertOrUpdateInput
            {
                Id = 1,
                Title = _title,
                Descripcion = "Mediante la herramienta SAS realizar un analisis predictivo y mostrar su gráfica",
                DueDate = DateTime.Parse("1-03-2021"),
                IsIndividual = true,
                IsCompleted = false
            };

            Task actual() => _assigmentService.Insert(assig);
            Assert.ThrowsAsync<MessageException>(actual); // Result.Message	"Title requiere un valor."	string
        }
        [Fact]
        public void Insert_ProcessIncomplete_NotImplementedException()
        {

            var assig = new AssignmentInsertOrUpdateInput
            {
                Id = 1,
                Title = "Analisis predictivo",
                Descripcion = "Mediante la herramienta SAS realizar un analisis predictivo y mostrar su gráfica",
                DueDate = DateTime.Parse("1-03-2021"),
                IsIndividual = true,
                IsCompleted = false,

            };
            Task actual() => _assigmentService.Insert(assig);
            Assert.ThrowsAsync<NotImplementedException>(actual);
        }
        [Fact]
        public void Update_CompleteInformation_AssigmentModified()
        {
            var assig = new AssignmentInsertOrUpdateInput
            {
                Id = 1,
                Title = "Analisis predictivo",
                Descripcion = "Mediante la herramienta SAS realizar un analisis predictivo y mostrar su gráfica",
                DueDate = DateTime.Parse("1-03-2021"),
                IsIndividual = true,
                IsCompleted = false,

            };
            bool UpdateCompleted = _assigmentService.Update(assig).IsCompleted;
            Assert.True(UpdateCompleted); // Assigment modified
        }
        [Fact]
        public void Update_ProcessIncomplete_NotImplementedException()
        {
            var assig = new AssignmentInsertOrUpdateInput
            {
                Id = 1,
                Title = "Analisis predictivo",
                Descripcion = "Mediante la herramienta SAS realizar un analisis predictivo y mostrar su gráfica",
                DueDate = DateTime.Parse("1-03-2021"),
                IsIndividual = true,
                IsCompleted = false,

            };
            Task actual() => _assigmentService.Update(assig);
            Assert.ThrowsAsync<NotImplementedException>(actual);
        }
        [Fact]
        public void Delete_ProcessIncomplete_NotImplementedException()
        {
            var assig = new AssignmentInsertOrUpdateInput
            {
                Id = 1,
                Title = "Analisis predictivo",
                Descripcion = "Mediante la herramienta SAS realizar un analisis predictivo y mostrar su gráfica",
                DueDate = DateTime.Parse("1-03-2021"),
                IsIndividual = true,
                IsCompleted = false,

            };
            Task actual() => _assigmentService.Delete(assig.Id);
            Assert.ThrowsAsync<NotImplementedException>(actual);
        }
        //Doesnt work
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async void Delete_ValidateIfAssigmentExists_AssigmentDeleted(int assignmentId)
        {
            var assig = new AssignmentInsertOrUpdateInput
            {
                Id = assignmentId,

            };
            await _assigmentService.Delete(assig.Id);
            var actual = await _context.Assignment.SingleOrDefaultAsync(x => x.Id == assignmentId);
            Assert.True(actual.IsDeleted);
        }
        // Method not implemented
        //[Theory]
        //[InlineData(1, true)]
        //[InlineData(2, true)]
        //[InlineData(3, true)]
        //public void Get_ValidateIfAssigmentIdExists_AssigmentData(int courseId, bool expected)
        //{
        //    Assert.True(true);
        //}

        // Method not implemented
        //[Fact]
        //public void GetAll_some_something()
        //{
        //    Assert.True(true);
        //}

        [Theory]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, true)]
        //[InlineData(-99, false)]
        //[InlineData(1000, false)]
        public void GetByCourse_AssigmentFound(int courseId, bool expected)
        {
            var assig = new AssignmentInsertOrUpdateInput
            {
                Title = "Analisis predictivo",
                Descripcion = "Mediante la herramienta SAS realizar un analisis predictivo y mostrar su gráfica",
                DueDate = DateTime.Parse("1-03-2021"),
                IsIndividual = true,
                IsCompleted = false,
                CourseId = courseId
            };
            _assigmentService.GetByCourse(assig.CourseId);
            bool actual = _assigmentService.GetByCourse(assig.CourseId).IsCompletedSuccessfully;
            Assert.Equal(expected, actual);
        }
    }
}
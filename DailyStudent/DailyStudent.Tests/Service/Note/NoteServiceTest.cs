using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DailyStudent.Api.DTOs.Note;
using DailyStudent.Api.Exceptions;
using DailyStudent.Api.Services.Note;
using Xunit;

namespace DailyStudent.Tests.Service.Note
{
    public class NoteServiceTest: TestBase
    {
        private readonly INoteService _noteService;
        public NoteServiceTest()
        {
            _noteService = new NoteService(_context);
        }
        //Methods to test: Insert, Update, Delete, GetByCourse

        [Theory]
        [InlineData(1, "Notas clase Web Development martes 09/02", "Entregar Echo Server para el jueves 11/02 !Importante", 1)]
        [InlineData(2, "Notas clase Web Development martes 11/02", "Title Description1", 1)]
        [InlineData(3, "Notas clase Web Development martes 16/02", "Title Description2", 1)]
        [InlineData(4, "Notas clase Web Development martes 18/02", "Title Description3", 1)]

        public async void Insert_CompleteInformationnOTECreated(int noteId, string noteTitle, string noteDesc, int courseId)
        {
            var note = new NoteInsertOrUpdateInput
            {
                Id = noteId,
                Title = noteTitle,
                Description = noteDesc,
                CourseId = courseId
            };
            var actual = await _noteService.Insert(note);
            Assert.NotNull(actual);
            Assert.IsType<NoteOutput>(actual);
        }
        [Theory]
        [InlineData(3, "Notas clase Web Development martes 16/02", "Title Description2", 1)]
        [InlineData(4, "Notas clase Web Development martes 18/02", "Title Description3", 1)]
        public async void Insert_InputandOutput_NotSameType(int noteId, string noteTitle, string noteDesc, int courseId)
        {
            var note = new NoteInsertOrUpdateInput
            {
                Id = noteId,
                Title = noteTitle,
                Description = noteDesc,
                CourseId = courseId
            };
            var actual = await _noteService.Insert(note);
            Assert.IsNotType<NoteInsertOrUpdateInput>(actual);
        }
        [Theory]
        [InlineData(1, "", "Title Description2", 1)]
        [InlineData(2, null, "Title Description3", 1)]
        public void Insert_EmptyOrNullNoteTitle_Throws_Exception(int noteId, string noteTitle, string noteDesc, int courseId)
        {
            var note = new NoteInsertOrUpdateInput
            {
                Id = noteId,
                Title = noteTitle,
                Description = noteDesc,
                CourseId = courseId
            };
            Task actual() => _noteService.Insert(note);

            Assert.ThrowsAsync<MessageException>(actual); // Result "Title requiere valor" string
        }
        //[Theory]
        //[InlineData(9999)]
        //[InlineData(-9)]
        //[InlineData(0)]
        //public void Insert_InvalidCourseId_Throws_Exception(short institutionCountry)
        //{
        //    var note = new NoteInsertOrUpdateInput
        //    {
        //        Id = noteId,
        //        Title = noteTitle,
        //        Description = noteDesc,
        //        CourseId = courseId
        //    };

        //    //act
        //    Func<Task> actual = () => _institutionService.Insert(institution);
        //    //assert
        //    Assert.ThrowsAsync<MessageException>(actual);
        //}
        //Doesnt work
        //[Theory]
        //[InlineData(1, true)]
        //[InlineData(2, true)]
        //[InlineData(3, true)]
        //[InlineData(-99, false)]
        //[InlineData(1000, false)]
        //public void GetByCourse_NoteFound(int courseId, bool expected)
        //{
        //    var note = new NoteInsertOrUpdateInput
        //    {
        //        Title = "Analisis predictivo",
        //        CourseId = courseId
        //    };
        //    bool actual = _noteService.GetByCourse(note.CourseId).IsCompletedSuccessfully;
        //    Assert.Equal(expected, actual);
        //}
    }
}

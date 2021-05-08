using DailyStudent.Api.DTOs.Note;
using DailyStudent.Api.Services.Note;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NotesController : ControllerBase
    {
        private readonly INoteService _noteService;

        public NotesController(
            INoteService noteService
        )
        {
            _noteService = noteService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NoteInsertOrUpdateInput note)
        {
            var output = await _noteService.Insert(note);
            return Ok(output);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] NoteInsertOrUpdateInput note)
        {
            var output = await _noteService.Update(note);
            return Ok(output);
        }

        [HttpDelete("{noteId}")]
        public async Task<IActionResult> Delete(int noteId)
        {
            await _noteService.Delete(noteId);
            return Ok(true);
        }


        [HttpGet("{courseId}/ByCourse")]
        public async Task<IActionResult> GetByCourse(int courseId)
        {
            var output = await _noteService.GetByCourse(courseId);
            return Ok(output);
        }
    }
}

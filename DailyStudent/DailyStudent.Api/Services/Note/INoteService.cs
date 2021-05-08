using DailyStudent.Api.DTOs.Note;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.Services.Note
{
    public interface INoteService
    {
        Task Delete(int noteId);
        Task<List<NoteOutput>> GetByCourse(int courseId);
        Task<NoteOutput> Insert(NoteInsertOrUpdateInput input);
        Task<NoteOutput> Update(NoteInsertOrUpdateInput input);

    }
}

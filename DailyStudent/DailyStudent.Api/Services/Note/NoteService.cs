using DailyStudent.Api.DataAccess;
using DailyStudent.Api.DTOs.Note;
using DailyStudent.Api.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.Services.Note
{
    public class NoteService: INoteService
    {
        private readonly DailyStudentDbContext _context;
        public NoteService(DailyStudentDbContext context)
        {
            _context = context;
        }

        public async Task<List<NoteOutput>> GetByCourse(int courseId)
        {
            return await _context.Note
                .AsNoTracking()
                .Where(x => x.Courseid == courseId && !x.IsDeleted)
                .OrderByDescending(x => x.CreationDate)
                .Select(x => new NoteOutput
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    CreationDate = x.CreationDate,
                    CourseId = x.Courseid
                })
                .ToListAsync();
        }

        public async Task<NoteOutput> Insert(NoteInsertOrUpdateInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Title))
                throw new MessageException(1, nameof(input.Title));
            if (string.IsNullOrWhiteSpace(input.Description))
                throw new MessageException(1, nameof(input.Description));

            var course = await _context.Course.AsNoTracking().SingleOrDefaultAsync(x => x.Id == input.CourseId);

            if (course == null)
                throw new MessageException(4, nameof(input.CourseId), input.CourseId.ToString());

            var newNote = new DataAccess.Note
            {
                Title = input.Title,
                Description = input.Description,
                CreationDate = DateTime.UtcNow,
                Courseid = input.CourseId,
                IsDeleted = false,
                DeletionDate = null
            };

            _context.Note.Add(newNote);

            await _context.SaveChangesAsync();

            return Map(newNote);
        }

        public async Task<NoteOutput> Update(NoteInsertOrUpdateInput input)
        {
            if (input.Id == 0)
                throw new MessageException(1, nameof(input.Id));
            if (string.IsNullOrWhiteSpace(input.Title))
                throw new MessageException(1, nameof(input.Title));
            if (string.IsNullOrWhiteSpace(input.Description))
                throw new MessageException(1, nameof(input.Description));

            var course = await _context.Course.AsNoTracking().SingleOrDefaultAsync(x => x.Id == input.CourseId);

            if (course == null)
                throw new MessageException(4, nameof(input.CourseId), input.CourseId.ToString());

            var note = await _context.Note.SingleOrDefaultAsync(x => x.Id == input.Id);

            if (note == null)
                throw new MessageException(4, nameof(input.Id), input.Id.ToString());

            note.Title = input.Title;
            note.Description = input.Description;

            await _context.SaveChangesAsync();

            return Map(note);
        }

        public async Task Delete(int noteId)
        {
            if (noteId == 0)
                throw new MessageException(1, nameof(noteId));

            var note = await _context.Note.SingleOrDefaultAsync(x => x.Id == noteId);

            if (note == null)
                throw new MessageException(4, nameof(noteId), noteId.ToString());

            note.DeletionDate = DateTime.UtcNow;
            note.IsDeleted = true;

            await _context.SaveChangesAsync();
        }

        private NoteOutput Map(DataAccess.Note input)
        {
            return new NoteOutput
            {
                Id = input.Id,
                Title = input.Title,
                Description = input.Description,
                CreationDate = input.CreationDate,
                CourseId = input.Courseid
            };
        }
    }
}

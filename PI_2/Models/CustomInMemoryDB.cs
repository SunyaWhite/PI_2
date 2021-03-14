using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace PI_2.Models
{
    /// <summary>
    /// Кастомая БД. очень просто
    /// </summary>
    public class CustomInMemoryDB : INoteRepository
    {
        /// <summary>
        /// Услованая база
        /// </summary>
        private readonly ConcurrentDictionary<int, Note> _db;
        /// <summary>
        /// Число эелементов в базе
        /// </summary>
        private int _counter; 
        
        public CustomInMemoryDB()
        {
            _db = new ConcurrentDictionary<int, Note>();
            _counter = 0;
        }

        public Task<IList<Note>> GetAllAsync() => Task
            .FromResult<IList<Note>>(_db.Values.ToList());

        public Task<Note> GetNoteByIdAsync(int id)
        {
            if (_db.TryGetValue(id, out var result))
            {
                return Task.FromResult<Note>(result);
            }

            throw new DataException("Failed to retrieve exception");
        }

        public Task<Note> CreateNewNoteAsync(Note note)
        {
            note.Id = _counter;
            _counter += 1;
            // Workaround
            _db.AddOrUpdate(note.Id, note, (_, note1) => note1);

            return Task.FromResult(note);
        }
        
        public async Task<Note> UpdateNoteAsync(int id, PreNote preNote)
        {
            var note = await this.GetNoteByIdAsync(id);

            if (note == null)
            {
                throw new DataException("No such note");
            }
            
            // Workaround
            return _db.AddOrUpdate(note.Id, note, (_, note1) =>
            {
                note1.Content = preNote.Content;
                note1.Title = preNote.Title;
                return note1;
            });
        }

        public Task<IList<Note>> SearchQueryAsync(string search)
        {
            var result = new List<Note>();
            
            foreach (var key in _db.Keys)
            {
                if (_db.TryGetValue(key, out var note))
                {
                    if (note.Content.Contains(search, StringComparison.OrdinalIgnoreCase)
                        || note.Title.Contains(search, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add(note);
                    }
                }
            }
            
            return Task.FromResult<IList<Note>>(result);
        }

        public Task<bool> DeleteByIdAsync(int id)
        {
            if (_db.TryRemove(id, out var _))
            {
                return Task.FromResult<bool>(true);
            }

            return Task.FromResult<bool>(false);
        }
    }
}
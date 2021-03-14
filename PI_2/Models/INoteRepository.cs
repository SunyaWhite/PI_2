using System.Collections.Generic;
using System.Threading.Tasks;

namespace PI_2.Models
{
    /// <summary>
    /// Интерфейс для работы с БД
    /// </summary>
    public interface INoteRepository
    {
        Task<IList<Note>> GetAllAsync();

        Task<Note> GetNoteByIdAsync(int id);

        Task<Note> CreateNewNoteAsync(Note note);
        
        Task<Note> UpdateNoteAsync(int id, PreNote note);

        Task<IList<Note>> SearchQueryAsync(string search);

        Task<bool> DeleteByIdAsync(int id);
    }
}
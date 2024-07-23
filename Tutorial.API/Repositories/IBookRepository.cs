using Tutorial.API.Models;

namespace Tutorial.API.Repositories
{
    public interface IBookRepository
    {
        public Task<List<BookModel>> GetAllBookAsync();
        public Task<BookModel> GetBookAsync(int Id);
        public Task<int> AddBookAsync(BookModel model);
        public Task UpdateBookAsync(int Id, BookModel model);
        public Task DeleteBookAsync(int Id);
    }
}

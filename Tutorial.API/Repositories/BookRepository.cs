using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Tutorial.API.Data;
using Tutorial.API.Models;

namespace Tutorial.API.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly BookStoreContext _context;
        private readonly IMapper _mapper;

        public BookRepository(BookStoreContext context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<int> AddBookAsync(BookModel model)
        {
            var book = _mapper.Map<Book>(model);
            _context.Books!.Add(book);
            await _context.SaveChangesAsync();
            return book.Id;
        }

        public async Task DeleteBookAsync(int Id)
        {
            var book = _context.Books!.SingleOrDefault(b => b.Id == Id);
            if (book != null) 
            {
                _context.Books!.Remove(book);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<BookModel>> GetAllBookAsync()
        {
            var books = await _context.Books!.ToListAsync();
            return _mapper.Map<List<BookModel>>(books);
        }

        public async Task<BookModel> GetBookAsync(int Id)
        {
            var book = await _context.Books!.FindAsync(Id);
            return _mapper.Map<BookModel>(book);
        }

        public async Task UpdateBookAsync(int Id, BookModel model)
        {
            if (Id == model.Id) 
            {
                var book = _mapper.Map<Book>(model);
                _context.Books!.Update(book);
                await _context.SaveChangesAsync();

            }
        }
    }
}

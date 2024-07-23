using AutoMapper;
using Tutorial.API.Data;
using Tutorial.API.Models;

namespace Tutorial.API.Helper
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper() 
        {
            CreateMap<Book, BookModel>().ReverseMap();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using Tutorial.API.Data;
using Tutorial.API.Helper;
using Tutorial.API.Models;
using Tutorial.API.Repositories;

namespace Tutorial.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookStoreContext _context;
        private readonly IBookRepository _repository;

        public BooksController(BookStoreContext context, IBookRepository repository)
        {
            _context = context;
            _repository = repository;
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddBook(BookModel model)
        {
            try
            {
                var BookId = await _repository.AddBookAsync(model);
                return CreatedAtAction(nameof(GetBookById), BookId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetAllBook()
        {
            try
            {
                return Ok(await _repository.GetAllBookAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetBookById(int id)
        {
            try
            {
                var book = await _repository.GetBookAsync(id);
                return book == null ? NotFound() : Ok(book);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPut("id")]
        [Authorize]
        public async Task<IActionResult> UpdateBook(int id, BookModel model)
        {
            try
            {
                await _repository.UpdateBookAsync(id, model);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("id")]
        [Authorize]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                await _repository.DeleteBookAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

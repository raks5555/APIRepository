using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookStore_API.Controllers
{
    /// <summary>
    /// Interacts with the Books Table
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        public readonly iLoggerService _logger;
        private readonly IMapper _mapper;

        public BooksController(IBookRepository bookRepository, iLoggerService logger, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _logger = logger;
            _mapper = mapper;
        }
        /// <summary>
        /// Gets All Books
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]                         //What are these?
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBooks()                             //What does this line means?
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Attempted Call" );
                var books = await _bookRepository.FindAll();
                var response = _mapper.Map <IList<BookDTO>>(books);
                _logger.LogInfo($"{location}: Successful");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalError($"{location} : {ex.Message} - {ex.InnerException}");
            }
        }

        /// <summary>
        /// Get book by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]                         
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBook(int id)                             
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Attempted Call for id: {id}");
                var book = await _bookRepository.FindById(id) ;
                if(book == null)
                {
                    _logger.LogWarn($"{location}: Failed to retrieve record with id: {id}");
                    return NotFound();
                }
                var response = _mapper.Map<BookDTO>(book);
                _logger.LogInfo($"{location}: Successfully got record with id : {id}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalError($"{location} : {ex.Message} - {ex.InnerException}");
            }
        }

        /// <summary>
        /// Creates a new book
        /// </summary>
        /// <param name="bookDTO"></param>
        /// <returns>Book object</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] BookCreateDTO bookDTO)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}:Create Attempted.");
                if(bookDTO == null)
                {
                    _logger.LogWarn($"{location}: Empty Request was submitted");
                    return BadRequest(ModelState);                              //What is ModelState?
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{location}: Data was incomplete");
                    return BadRequest(ModelState);
                }
                var book = _mapper.Map<Book>(bookDTO);
                var isSuccess = await _bookRepository.Create(book);
                if (!isSuccess)
                {
                    return InternalError($"{location} : Create failed!");
                }

                _logger.LogInfo($"{location}:Creation was Successful.");
                _logger.LogInfo($"{location}:{book}");

                return Created("Create", new {book});
            }
            catch (Exception ex)
            {
                return InternalError($"{location} : {ex.Message} - {ex.InnerException}");
            }
        }

        /// <summary>
        /// Updates a Book
        /// </summary>
        /// <param name="id"></param>
        /// <param name="authorDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] BookUpdateDTO bookDTO)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"Update Attempted on record with id: {id}");
                if (id < 1 || bookDTO == null || id != bookDTO.Id)
                {
                    _logger.LogWarn($"{location} : Update failed with bad data id: {id}");
                    return BadRequest();
                }

                var isExists = await _bookRepository.IsExists(id);
                if (!isExists)
                {
                    _logger.LogWarn($"{location} : Failed to retrieve the record with id: {id}");
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{location} : Data was Incomplete");
                    return BadRequest(ModelState);
                }
                var book = _mapper.Map<Book>(bookDTO);
                var isSuccess = await _bookRepository.Update(book);

                if (!isSuccess)
                {
                    return InternalError($"{location} : Update failed for the record with id: {id}");
                }
                _logger.LogInfo("Book with id: {id} successfully updated");
                return NoContent();
            }
            catch (Exception ex)
            {
                return InternalError($"{ex.Message} - {ex.InnerException}");
            }
        }

        /// <summary>
        /// Removes a book by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location} :Book with id: {id} Delete Attemted");
                if (id < 1)
                {
                    _logger.LogWarn($"{location} :Book Delete failed with bad data");
                    return BadRequest();
                }
                var isExists = await _bookRepository.IsExists(id);
                if (!isExists)
                {
                    _logger.LogWarn($"{location} :Book with id:{id} was not found.");
                    return NotFound();
                }

                var book = await _bookRepository.FindById(id);
                var isSuccess = await _bookRepository.Delete(book);

                if (!isSuccess)
                {
                    return InternalError($"{location} :Book Delete failed!");
                }
                _logger.LogInfo($"{location} :Book with id:{id} was deleted successfully.");
                return NoContent();
            }
            catch (Exception ex)
            {
                return InternalError($"{ex.Message} - {ex.InnerException}");
            }
        }

        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }

        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Something went wrong. Please Contact the Adminitrator.");
        }

    }
}

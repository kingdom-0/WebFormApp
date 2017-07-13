using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BooksAPI.Models;
using System.Linq.Expressions;
using BooksAPI.DTOs;

namespace BooksAPI.Controllers
{
    [RoutePrefix("api/books")]
    public class BooksController : ApiController
    {
        private BooksAPIContext db = new BooksAPIContext();
        private static readonly Expression<Func<Book, BookDto>> AsBookDto =
            b => new BookDto
            {
                Title = b.Title,
                Author = b.Author.Name,
                Genre = b.Genre
            };

        // GET: api/Books
        [Route("")]
        public IQueryable<BookDto> GetBooks()
        {
            return db.Books.Include(b=>b.Author).Select(AsBookDto);
        }

        // GET: api/Books/5
        [ResponseType(typeof(BookDto))]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetBook(int id)
        {
            BookDto book = await db.Books.Include(b=>b.Author)
                .Where(b=>b.BookId == id)
                .Select(AsBookDto)
                .FirstOrDefaultAsync();
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [ResponseType(typeof(BookDetailDto))]
        [Route("{id:int}/details")]
        public async Task<IHttpActionResult> GetBookDetails(int id)
        {
            var book = await db.Books.Include(b => b.Author)
                .Where(x => x.BookId == id)
                .Select(x => new BookDetailDto
                {
                    Title = x.Title,
                    Genre = x.Genre,
                    PublishDate = x.PublishDate,
                    Description = x.Description,
                    Price = x.Price,
                    Author = x.Author.Name
                })
                .FirstOrDefaultAsync();

            if(book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [ResponseType(typeof(BookDetailDto))]
        [Route("{genre}")]
        public IQueryable<BookDto> GetBookByGenre(string genre)
        {
            return db.Books.Where(b => string.Equals(b.Genre, genre))
                .Select(AsBookDto);
        }

        [ResponseType(typeof(Author))]
        [Route("~/api/authors")]
        public IQueryable<Author> GetAuthors()
        {
            return db.Authors;
        }

        [ResponseType(typeof(BookDto))]
        [Route("~/api/authors/{authorId:int}/books")]
        public IQueryable<BookDto> GetBooksByAuthorId(int authorId)
        {
            return db.Books.Where(b => b.AuthorId == authorId)
                .Select(AsBookDto);
        }

        [ResponseType(typeof(BookDto))]
        [Route("date/{publishedDate:datetime:regex(\\d{4}-\\d{2}-\\d{2})}")]
        public IQueryable<BookDto> GetBooks(DateTime publishedDate)
        {
            return db.Books.Where(b=>DbFunctions.TruncateTime(b.PublishDate) == DbFunctions.TruncateTime(publishedDate)).Select(AsBookDto);
        }

        // PUT: api/Books/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutBook(int id, Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != book.BookId)
            {
                return BadRequest();
            }

            db.Entry(book).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Books
        [ResponseType(typeof(Book))]
        public async Task<IHttpActionResult> PostBook(Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Books.Add(book);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = book.BookId }, book);
        }

        // DELETE: api/Books/5
        [ResponseType(typeof(Book))]
        public async Task<IHttpActionResult> DeleteBook(int id)
        {
            Book book = await db.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            db.Books.Remove(book);
            await db.SaveChangesAsync();

            return Ok(book);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BookExists(int id)
        {
            return db.Books.Count(e => e.BookId == id) > 0;
        }
    }
}
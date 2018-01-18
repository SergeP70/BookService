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
using BookService.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace BookService.Controllers
{
    public class BooksController : ApiController
    {
        private BookServiceContext db = new BookServiceContext();

        // GET: api/Books
        public IQueryable<BookDTO> GetBooks()
        {
            //1. return db.Books.Include(b => b.Author);
            //2. var books = from b in db.Books
            //            select new BookDTO()
            //            {
            //                Id = b.Id,
            //                Title = b.Title,
            //                AuthorName = b.Author.Name
            //            };

            // De methode ProjectTo zet een object van de bronklasse Book nu om in een object van de doelklasse BookDTO
            var books = db.Books.ProjectTo<BookDTO>();
            return books;
        }

        // GET: api/Books/5
        [ResponseType(typeof(BookDetailDTO))]
        public async Task<IHttpActionResult> GetBook(int id)
        {
            //1. Book book = await db.Books.FindAsync(id);
            //2. var book = await db.Books.Include(b => b.Author).Select(b =>
            //    new BookDetailDTO()
            //    {
            //        Id=b.Id,
            //        Title = b.Title,
            //        Year = b.Year,
            //        Price = b.Price,
            //        AuthorId = b.Author.Id,
            //        AuthorName = b.Author.Name,
            //        Genre = b.Genre
            //    }).SingleOrDefaultAsync(b => b.Id==id);


            var book = await db.Books.ProjectTo<BookDetailDTO>().SingleOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        // PUT: api/Books/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutBook(int id, BookDetailDTO bookDetailDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Book book = Mapper.Map<Book>(bookDetailDTO);
            db.Set<Book>().Attach(book); // voo rupdate
            db.Entry(book).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return Ok(bookDetailDTO);

            //if (id != book.Id)
            //{
            //    return BadRequest();
            //}

            //db.Entry(book).State = EntityState.Modified;

            //try
            //{
            //    await db.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!BookExists(id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}
            //   return StatusCode(HttpStatusCode.NoContent);


        }

        // POST: api/Books
        [ResponseType(typeof(BookDTO))]
        public async Task<IHttpActionResult> PostBook(BookDetailDTO bookDetailDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //var book = new Book
            //{
            //    Id = bookDetailDTO.Id,
            //    Title = bookDetailDTO.Title,
            //    Year = bookDetailDTO.Year,
            //    Price = bookDetailDTO.Price,
            //    Genre = bookDetailDTO.Genre,
            //    AuthorId = bookDetailDTO.AuthorId
            //};

            Book book = Mapper.Map<Book>(bookDetailDTO);

            db.Books.Add(book);
            await db.SaveChangesAsync();

            //Id naar DTO wegschrijven
            bookDetailDTO.Id = book.Id;

            return Ok(bookDetailDTO);
            //return CreatedAtRoute("DefaultApi", new { id = book.Id }, book);
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
            return db.Books.Count(e => e.Id == id) > 0;
        }
    }
}
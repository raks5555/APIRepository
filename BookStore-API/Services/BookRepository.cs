using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookStore_API.Contracts;
using BookStore_API.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStore_API.Services
{
    public class BookRepository : IBookRepository
    {
        public BookRepository()                                                 //What is IBookRepository?
        {
        }

        private readonly ApplicationDbContext _db;

        public BookRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Create(Book entity)
        {
            await _db.Books.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(Book entity)
        {
            _db.Books.Remove(entity);
            return await Save();
        }

        public async Task<IList<Book>> FindAll()
        {
            var Books =  await _db.Books.ToListAsync();
            return Books;
        }

        public async Task<Book> FindById(int id)
        {
            var Book = await _db.Books.FindAsync(id) ;
            return Book;
        }

        public async Task<bool> IsExists(int id)
        {
            var isExists = await _db.Books.AnyAsync (q => q.Id == id );         //What is lambda expression and how it works?
            return isExists;                                                    //How this scans the entire database for books?
        }

        public async Task<bool> Save()
        {
            var changes = await _db.SaveChangesAsync();                         //What is asynchronus saving?
            return changes > 0;
        }

        public async Task<bool> Update(Book entity)
        {
            _db.Books.Update(entity);
            return await Save();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Interface;
using DataLayer.DataContext;
using DataLayer.CustomModels;
using DataLayer.DataModels;

namespace BusinessLayer.Repositories
{
    public class RecordsRepo : IRecords
    {
        private readonly ApplicationDbContext _context;

        public RecordsRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public Recordscm GetRecords()
        {
            var records = from r in _context.Books
                          join rw in _context.Borrowers
                          on r.BorrowerId equals rw.Id
                          select (new Recordscm.Records()
                          {
                              Id = r.Id,
                              BookName=r.BookName,
                              Author=r.Author,
                              BorrowerName=r.BorrowerName,
                              DateOfIssue=r.DateOfIssue,
                              City=rw.City,
                              Genre=r.Genre,
                              BorrowerId=rw.Id,
                          });

            var recordsList = new Recordscm()
            {
                records = records.ToList(),
            };

            return recordsList;
        }


       



        public Recordscm GetBookDetails(int BorrowerId)
        {
            if(BorrowerId != null)
            {
                var book=_context.Books.FirstOrDefault(x=>x.BorrowerId == BorrowerId);

                if(book != null)
                {
                    Recordscm bookDetails = new Recordscm()
                    {
                        BookName = book.BookName,
                        Author = book.Author,
                        BorrowerName = book.BorrowerName,
                        DateOfIssue = book.DateOfIssue,
                        City = book.City,
                        Genre = book.Genre,
                    };

                    return bookDetails;
                }

                return null;
            }

          
            return null;
        }


        public bool SaveBookForm(Recordscm cm)
        {
            if(cm.BorrowerId == null) 
            {
                Borrower borrower = new Borrower()
                {
                    City = cm.City,
                };

                _context.Borrowers.Add(borrower);
                _context.SaveChanges();


                Book book = new Book()
                {
                    BookName = cm.BookName,
                    Author = cm.Author,
                    BorrowerId = borrower.Id,
                    BorrowerName = cm.BorrowerName,
                    DateOfIssue = cm.DateOfIssue,
                    City = borrower.City,
                    Genre = cm.Genre,
                };

                _context.Books.Add(book);
                _context.SaveChanges();

                return true;
            }

            if(cm.BorrowerId != null)
            {
                var borrower = _context.Borrowers.FirstOrDefault(x=>x.Id== cm.BorrowerId);

                var book = _context.Books.FirstOrDefault(x=>x.BorrowerId== cm.BorrowerId);

                if(borrower != null)
                {
                    borrower.City= cm.City;

                    _context.SaveChanges();

                    book.BookName= cm.BookName;
                    book.Author= cm.Author;
                    book.BorrowerName= cm.BorrowerName;
                    book.DateOfIssue= cm.DateOfIssue;
                    book.City= borrower.City;
                    book.Genre= cm.Genre;

                    _context.SaveChanges();

                    return true;
                }
            }

            return false;
        }


        public bool DeleteBookRecord(int BorrowerId)
        {
            var borrower= _context.Borrowers.FirstOrDefault(x=>x.Id== BorrowerId);

            var book=_context.Books.FirstOrDefault(x=>x.BorrowerId == BorrowerId);

            if(borrower != null)
            {

                _context.Books.Remove(book);
                _context.SaveChanges();

                _context.Borrowers.Remove(borrower);
                _context.SaveChanges();


                return true;
            }

            return false;
        }
    }
}

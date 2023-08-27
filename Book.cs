using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ex3
{
    public class Book
    {
        //attri
        protected string _book_id;
        protected string _book_name;
        protected string _author;
        protected string _genre;

        //c-tor
        public Book(string bookId, string book_name, string author, string genre)
        {
            _book_id = bookId;
            _book_name = book_name;
            _author = author;
            _genre = genre;
        }
        //empty c-tor
        public Book() { }

        //methods
        public void setBookID(string bookId)
        {
            _book_id = bookId;
        }
        public string getBookId()
        {
            return _book_id;
        }
        public void setBookName(string book_name)
        {
            this._book_name = book_name;
        }
        public string getBookName()
        {
            return _book_name;
        }
        public void setAuthor(string author)
        {
            this._author = author;
        }
        public string getAuthor()
        {
            return _author;
        }
        public void setGenre(string genre)
        {
            this._genre = genre;
        }
        public string getGenre()
        {
            return _genre;
        }

    }
}
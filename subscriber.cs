using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ex3
{
    public class subscriber
    {
        //attri
        protected string _name;
        private int _count_books = 0;
        private string _subscriberId;
        private List<Book> _subscriberBooks = new List<Book>(3);

        //c-tor
        public subscriber(string subscriberId, string name, int CountBooks)
        {
            _subscriberId = subscriberId;
            _name = name;
            _count_books = CountBooks;
        }

        //methods
        public List<Book> getSubscriberBooks()
        {
            return _subscriberBooks;
        }
        public void addBook(Book book)
        {
            _subscriberBooks.Add(book);
        }
        public int getCountBooks()
        {
            return _subscriberBooks.Count;
        }
        public void rmBookFromSubBooks(Book book)
        {
            _subscriberBooks.Remove(book);
        }

        public void setName(string name)
        {
            _name = name;
        }
        public string getName()
        {
            return this._name;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ex3
{
    public class PaperBook : Book
    {
        //attri
        public int _copeis = 1;

        //c-tor
        public PaperBook(string bookId, string book_name, string author, string genre, int copies) : base(bookId, book_name, author, genre)
        {
            _copeis = copies;
        }
        //methods
        public void AddCopy()
        {
            _copeis++;
        }
        public void RmCopy()
        {
            _copeis--;
        }
        public int getCopy()
        {
            return _copeis;
        }

    }
}


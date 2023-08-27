using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace ex3
{
    public class Library
    {
        //atrri
        Dictionary<string, Book> _books = new Dictionary<string, Book>();
        Dictionary<string, subscriber> _subscribers = new Dictionary<string, subscriber>();

        //c-tor
        //default empty one


        //************************************************ library management methods *************************************

        //this method use to add initail books & subscribers to library
        public void initialDataInLibrary()
        {
            //add books
            PaperBook p1 = new PaperBook("111", "pbook", "galila", "action", 1);
            PaperBook p2 = new PaperBook("11", "pbook a", "gila", "action", 1);
            digitalBook d1 = new digitalBook("00", "dbook a", "yona", "drama");
            digitalBook d2 = new digitalBook("000", "dbook", "yossi", "drama");
            digitalBook d3 = new digitalBook("222", "dbook b", "eli", "drama");
            _books.Add("111", p1);
            _books.Add("11", p2);
            _books.Add("00", d1);
            _books.Add("000", d2);
            _books.Add("222", d3);
            //add subscribers
            subscriber s1 = new subscriber("01", "avi revach", 3);
            subscriber s2 = new subscriber("12", "yuval", 3);
            subscriber s3 = new subscriber("23", "ori", 3);
            subscriber s4 = new subscriber("45", "almog", 3);
            subscriber s5 = new subscriber("89", "amit", 2);
            _subscribers.Add("01", s1);
            _subscribers.Add("12", s2);
            _subscribers.Add("23", s3);
            _subscribers.Add("45", s4);
            _subscribers.Add("89", s5);
            //add books to subscribers list
            s1.addBook(d3);
            s2.addBook(d3);
            s3.addBook(d3);
            s4.addBook(d3);
            s5.addBook(d3);
            s1.addBook(d2);
            s2.addBook(d2);
            s3.addBook(d2);
            s4.addBook(d2);
            s5.addBook(d2);

            Console.WriteLine("initail books & subscribers details Successfully added to library!");
        }
        //methods
        public void addBook()
        {
            Console.WriteLine("please enter:\nbook id\nbook name (without any numbers)\ntype of book (enter 0 for digital OR 1 for paper)\nauthor\ngenre\neach value at a separate line\n");
            string book_id = Console.ReadLine();
            if (IsValiBookdId(book_id) == false) { return; }
            string book_name = Console.ReadLine();
            if (!IsValidString(book_name))
            {
                Console.WriteLine("wrong input, please enter valid name, without any numbers, special characters or unnecessary whitespaces");
                return;
            }
            if (!int.TryParse(Console.ReadLine(), out int type) || type < 0 || type > 1)
            {
                Console.WriteLine("wrong input, enter 0 for digital OR 1 for paper");
                return;
            }
            string author = Console.ReadLine();
            if (!IsValidString(author))
            {
                Console.WriteLine("wrong input, please enter valid author");
                return;
            }
            string genre = Console.ReadLine();
            if (!IsValidString(genre))
            {
                Console.WriteLine("wrong input, please enter valid genre");
                return;
            }
            //case digital:
            if (type == 0)
            {
                if (_books.ContainsKey(book_id)) { Console.WriteLine("book id is already exist, use different id "); return; }
                Book newDiBook = new digitalBook(book_id, book_name, author, genre);
                _books.Add(book_id, newDiBook);
                string bookType = "digital"; //use for the addBookToDb() method
                //sql
                insertDigiBooksToDb(book_id, book_name, author, genre, bookType);
                Console.WriteLine("Created new digital book!");
                Console.WriteLine("New digital book added to library database!");
                return;
            }
            if (type == 1) //case paper
            {
                //A copy can only be added if all(!) properties are the same, otherwise not.
                if ((_books.ContainsKey(book_id)) && book_name == _books[book_id].getBookName() && author == _books[book_id].getAuthor() && genre == _books[book_id].getGenre())
                {
                    if (_books[book_id] is PaperBook)
                    {
                        Console.WriteLine("book id is already exist, adding copy");
                        PaperBook pBook = (PaperBook)_books[book_id]; //casting for use getCopy() method
                        pBook.AddCopy();
                        Console.WriteLine($"current copies: {pBook.getCopy()}");
                        int pbookCopies = pBook.getCopy();
                        //sql
                        updateCopies(book_id, pbookCopies);
                        return;
                    }
                    else
                    {
                        { Console.WriteLine("digital book id is already exist"); return; }
                    }
                }
                else if (_books.ContainsKey(book_id))
                {
                    Console.WriteLine("book id is already exist. in order to add a book copy, please provide similar details other than an id");
                    return;
                }
                //otherwise -> create new paperbook with 1 copy
                Book newPaBook = new PaperBook(book_id, book_name, author, genre, 1);
                _books.Add(book_id, newPaBook);
                string bookType = "paper"; //use for the addBookToDb() method, for "paperOrDidital" column
                PaperBook paperbook = (PaperBook)newPaBook;
                int copeis = paperbook.getCopy(); //use for the addBookToDb() method, for "copies" column
                //calling sql method
                insertPaperBooksToDb(book_id, book_name, author, genre, bookType, copeis);
                Console.WriteLine("Created new paper book!");
                Console.WriteLine("New paper book added to library database!");
                return;
            }
        }
        // this method insert paper book to books table
        public void insertPaperBooksToDb(string book_id, string book_name, string author, string genre, string bookType, int copies)
        {
            SqlConnectionStringBuilder conn_str = new SqlConnectionStringBuilder();
            conn_str.ConnectionString = @"Server=localhost\SQLEXPRESS;Database=Avi_DB;Trusted_Connection=True;";

            SqlConnection sqlConnection = new SqlConnection(conn_str.ConnectionString);
            sqlConnection.Open();
            // create command to run the query
            using SqlCommand cmd = new SqlCommand
            {
                Connection = sqlConnection,
                CommandType = System.Data.CommandType.Text,
            };
            try
            {
                cmd.CommandText = "INSERT INTO books(book_id, bookName, author, genre, paperOrDidital, copies) VALUES('" + book_id + "' , '" + book_name + "', '" + author + "', '" + genre + "', '" + bookType + "', '" + copies + "');";
                cmd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        //update copies of paperbook in books table
        public void updateCopies(string book_id, int copies)
        {
            SqlConnectionStringBuilder conn_str = new SqlConnectionStringBuilder();
            conn_str.ConnectionString = @"Server=localhost\SQLEXPRESS;Database=Avi_DB;Trusted_Connection=True;";

            SqlConnection sqlConnection = new SqlConnection(conn_str.ConnectionString);
            sqlConnection.Open();
            // create command to run the query
            using SqlCommand cmd = new SqlCommand
            {
                Connection = sqlConnection,
                CommandType = System.Data.CommandType.Text,
            };
            try
            {
                cmd.CommandText = "UPDATE books SET copies = '" + copies + "' WHERE book_id = '" + book_id + "' ";
                cmd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { sqlConnection.Close(); }

        }
        // this method insert digital book to books table
        public void insertDigiBooksToDb(string book_id, string book_name, string author, string genre, string bookType)
        {
            SqlConnectionStringBuilder conn_str = new SqlConnectionStringBuilder();
            conn_str.ConnectionString = @"Server=localhost\SQLEXPRESS;Database=Avi_DB;Trusted_Connection=True;";

            SqlConnection sqlConnection = new SqlConnection(conn_str.ConnectionString);
            sqlConnection.Open();
            // create command to run the query
            using SqlCommand cmd = new SqlCommand
            {
                Connection = sqlConnection,
                CommandType = System.Data.CommandType.Text,
            };
            try
            {
                cmd.CommandText = "INSERT INTO books(book_id, bookName, author, genre, paperOrDidital, copies) VALUES('" + book_id + "' , '" + book_name + "', '" + author + "', '" + genre + "', '" + bookType + "', NULL);";
                cmd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        // add Subscriber to Db & dictionary
        public void addSubscriber() 
        {
            Console.WriteLine("please enter:\nSubscriber's id\nSubscriber name\neach value at a separate line");
            string Subscriber_id = Console.ReadLine();
            if (!(IsValiSubscriberdId(Subscriber_id))) { return; }
            //check if subscriber already exist
            if (_subscribers.ContainsKey(Subscriber_id))
            {
                Console.WriteLine("Subscriber id is already exist, use different id");
                return;
            }
            string Subscriber_name = Console.ReadLine();
            if (!IsValidString(Subscriber_name))
            {
                Console.WriteLine("wrong input, please enter valid name");
                return;
            }
            //create new subscriber and add it to dictionary
            subscriber newsubscriber = new subscriber(Subscriber_id, Subscriber_name, 0);
            _subscribers.Add(Subscriber_id, newsubscriber);
            //sql
            insertSubscriberToDb(Subscriber_id, Subscriber_name);
            Console.WriteLine($"success. The Subscriber: {Subscriber_name}, with id: {Subscriber_id} has been added to the library database!");
            return;
        }
        //insert Subscriber to Subscribers table in db
        public void insertSubscriberToDb(string subscriberId, string Subscriber_name)
        {
            SqlConnectionStringBuilder conn_str = new SqlConnectionStringBuilder();
            conn_str.ConnectionString = @"Server=localhost\SQLEXPRESS;Database=Avi_DB;Trusted_Connection=True;";

            SqlConnection sqlConnection = new SqlConnection(conn_str.ConnectionString);
            sqlConnection.Open();
            // create command to run the query
            using SqlCommand cmd = new SqlCommand
            {
                Connection = sqlConnection,
                CommandType = System.Data.CommandType.Text,
            };
            try
            {
                cmd.CommandText = "INSERT INTO subscribers(Subscriber_id, fullName, book_id1, book_id2, book_id3) VALUES('" + subscriberId + "' , '" + Subscriber_name + "', NULL, NULL, NULL);";
                cmd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        //*********************************** End of library management methods *************************************


        //*********************************** Regex methods: ********************************************

        //method for verify string input 
        //this method gets srting ang verify all char in str is char 
        public bool IsValidString(string str)
        {
            if (str == "" || str == null)
            {
                //Console.WriteLine("you have not enter any value");
                return false;
            }
            string pattern = @"^\s+"; //match whitespace at the beginning of a string
            string pattern1 = @"\s+$"; //match whitespace at the end of a string
            bool match = Regex.IsMatch(str, pattern);
            bool match1 = Regex.IsMatch(str, pattern1);
            if (match == true || match1 == true) { return false; } //if user input match whitespaces at the beginning or end of a string -> false and out 
            foreach (char c in str)
            {
                //if string contains digits OR special character like: 5, 0, 2, /, %, etc. -> false and out the loop
                if (!(char.IsLetter(c) || char.IsWhiteSpace(c)))
                {
                    return false;
                }
            }
            return true;
        }


        //This method defines a regular expression pattern that matches a comma & whitespace at the end of the string,
        //and replaces it with dot, using the Regex.Replace method.
        public string RemoveCommaAtEnd(string input)
        {
            string pattern = @",\s$"; // matches a comma at the end of the string
            string replacement = ".";
            return Regex.Replace(input, pattern, replacement);
        }

        // this method verify if input string from the user is book id in range 0-5 digits and contains only numbers
        public bool IsValiBookdId(string input)
        {
            string pattern = @"^\d{1,5}$";
            if (Regex.IsMatch(input, pattern)) { return true; }
            else
            {
                Console.WriteLine("wrong input, enter id up to 5 digits in the range 0-9");
                return false;
            }
        }
        // this method verify if input string from the user is Subscriber id in range 0-9 digits and contains only numbers
        public bool IsValiSubscriberdId(string input)
        {
            string pattern = @"^\d{1,9}$";
            if (Regex.IsMatch(input, pattern)) { return true; }
            else { Console.WriteLine("wrong input, enter id up to 9 digits in the range 0-9"); return false; }
        }

        //*********************************** End of Regex methods ********************************************



        //*********************************** Print Details Methods ********************************************
        public void printBookDetails()
        {
            Console.WriteLine("please enter book id");
            string book_id = Console.ReadLine();
            if (IsValiBookdId(book_id) == false) { return; }
            if (_books.Count == 0) { Console.WriteLine("there are no books in the library, please add some books :) "); return; }
            if (_books.ContainsKey(book_id) == false) { Console.WriteLine("book id does not exist"); return; }
            if (_books[book_id] is digitalBook) //case digital
            {
                digitalBook Dbook = (digitalBook)_books[book_id]; //casting for use digital-book methods
                Console.WriteLine($"book name: {Dbook.getBookName()}, digital-book, book author: {Dbook.getAuthor()}, genre: {Dbook.getGenre()}");
                return;
            }
            else if (_books[book_id] is PaperBook)//case paper
            {
                PaperBook Pbook = (PaperBook)_books[book_id]; //casting for use paperbook methods
                Console.WriteLine($"book name: {Pbook.getBookName()}, paper-book, book author: {Pbook.getAuthor()}, genre: {Pbook.getGenre()}, number of copies: {Pbook.getCopy()}");
                return;
            }
        }
        //print by genre method
        public void printBooksByGenre()
        {

            Console.WriteLine("please enter genre");
            string genre = Console.ReadLine();
            if (!IsValidString(genre)) { Console.WriteLine("wrong input, please enter valid genre"); return; }
            bool GenreExist = false; //use to out the foreach loop
            string print = ""; //it use for concatenation
            if (_books.Count == 0) { Console.WriteLine("there are no books in the library, please add some books :) "); return; }
            foreach (KeyValuePair<string, Book> element in _books)
            {
                string genre_from_dictionary = element.Value.getGenre();
                if (genre_from_dictionary == genre)
                {
                    print += $"book id: {element.Key} and it book name: {element.Value.getBookName()}, ";
                    GenreExist = true;
                }
            }
            if (!GenreExist) { Console.WriteLine("no such genre as you enter"); return; }
            string NewPrint = RemoveCommaAtEnd(print); //remove comma when finish loop
            Console.WriteLine($"books by genre '{genre}':");
            Console.Write(NewPrint);
        }

        public void printSubscriberBooks()
        {
            Console.WriteLine("please enter: Subscriber id");
            string Subscriber_id = Console.ReadLine();
            //checking valid Subscriber id and if subscriber is Exist
            if (!(IsValiSubscriberdId(Subscriber_id))) { return; }
            if (_subscribers.ContainsKey(Subscriber_id) == false) { Console.WriteLine("Subscriber id does not exist"); return; }
            subscriber subscriber = _subscribers[Subscriber_id]; //create subscriber obj, by Accessing value using key
            if (subscriber.getCountBooks()== 0) { Console.WriteLine("subscriber have 0 books"); return; }
            Console.WriteLine("subscriber books:");
            foreach (var subscriberBook in subscriber.getSubscriberBooks())
            {
                Console.WriteLine($"book id: {subscriberBook.getBookId()}, book name: {subscriberBook.getBookName()}, author: {subscriberBook.getAuthor()}, genre: {subscriberBook.getGenre()}.");
            }
        }

        //*********************************** End Of Print Details Methods ********************************************


        //*********************************** Returning & Borrow Book Methods *****************************************

        // ReturningBook method
        public void ReturningBook()
        {
            Console.WriteLine("please enter:\nSubscriber's id\nbook id\neach value at a separate line\n");
            string Subscriber_id = Console.ReadLine();
            //checking valid Subscriber id and if subscriber is Exist
            if (!(IsValiSubscriberdId(Subscriber_id))) { return; }
            if (_subscribers.ContainsKey(Subscriber_id) == false) { Console.WriteLine("Subscriber id does not exist"); return; }
            if (_subscribers.Count == 0) { Console.WriteLine("there are no Subscribers in the library, please add some Subscribers :)"); return; }
            subscriber subscriber = _subscribers[Subscriber_id];//create subscriber obj by Accessing to value using the key
            if (subscriber.getCountBooks() == 0) { Console.WriteLine("subscriber have 0 books"); return; }
            string book_id = Console.ReadLine();
            //checking book id
            if (IsValiBookdId(book_id) == false) { return; }
            if (_books.Count == 0) { Console.WriteLine("there are no books in the library, please add some books :) "); return; }
            if (!_books.ContainsKey(book_id)) { Console.WriteLine("no book id OR book name as you enter"); return; }
            Book return_book_in_dic = _books[book_id]; //create obj by Accessing to value using the key
            //string book_id_in_dic = return_book_in_dic.getBookId();
            if (!subscriber.getSubscriberBooks().Contains(return_book_in_dic)) { Console.WriteLine("subscriber does not have this book"); return; }
            Console.WriteLine("subscriber have this book!");
            //case paper
            if (return_book_in_dic is PaperBook) // check if paper type
            {
                PaperBook pBook = (PaperBook)return_book_in_dic;
                subscriber.rmBookFromSubBooks(pBook); // Removing value from the list Using Remove() method
                pBook.AddCopy();
                Console.WriteLine("current number copies of the book id '" + pBook.getBookId() + "' are: " + pBook.getCopy());
                Console.WriteLine($"success, returning paper book with id: {pBook.getBookId()}");
                Console.WriteLine("current books of subscriber: " + subscriber.getCountBooks());
                string bookId = pBook.getBookId();
                int copies = pBook.getCopy();
                //calling sql method
                addCopyAndRmBookInDB(Subscriber_id, book_id, copies);
                return;
            }
            if (return_book_in_dic is digitalBook) //check if digital type
            {
                digitalBook digitalBook = (digitalBook)return_book_in_dic;
                subscriber.rmBookFromSubBooks(digitalBook);
                Console.WriteLine($"success, returning digital book with id: {digitalBook.getBookId()}");
                Console.WriteLine("current books of subscriber: " + subscriber.getCountBooks());
                //calling sql method
                rmDigiBookIdFromSub(Subscriber_id, book_id);
                return;
            }
        }

        //this method remove digital book id from subscriber
        public void rmDigiBookIdFromSub(string subscriber_id, string return_book_id)
        {
            SqlConnectionStringBuilder conn_str = new SqlConnectionStringBuilder();
            conn_str.ConnectionString = @"Server=localhost\SQLEXPRESS;Database=Avi_DB;Trusted_Connection=True;";

            SqlConnection sqlConnection = new SqlConnection(conn_str.ConnectionString);
            sqlConnection.Open();
            // create command to run the query
            using SqlCommand cmd = new SqlCommand
            {
                Connection = sqlConnection,
                CommandType = System.Data.CommandType.Text,
            };
            try
            {
                //remove book id from Subscriber part
                cmd.CommandText = "SELECT COUNT(*) FROM subscribers WHERE subscriber_id = '" + subscriber_id + "' AND (book_id1 = '" + return_book_id + "')";
                int count = (int)cmd.ExecuteScalar();
                if (count > 0) 
                {
                    cmd.CommandText = "UPDATE subscribers SET book_id1 = NULL WHERE subscriber_id = '" + subscriber_id + "' AND book_id1 = '" + return_book_id + "'";
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("book_id1 column successfully updated"); //validation check
                }
                cmd.CommandText = "SELECT COUNT(*) FROM subscribers WHERE subscriber_id = '" + subscriber_id + "' AND (book_id2 = '" + return_book_id + "')";
                count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    cmd.CommandText = "UPDATE subscribers SET book_id2 = NULL WHERE subscriber_id = '" + subscriber_id + "' AND book_id2 = '" + return_book_id + "'";
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("book_id2 column successfully updated"); //validation check
                }
                cmd.CommandText = "SELECT COUNT(*) FROM subscribers WHERE subscriber_id = '" + subscriber_id + "' AND (book_id3 = '" + return_book_id + "')";
                count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    cmd.CommandText = "UPDATE subscribers SET book_id3 = NULL WHERE subscriber_id = '" + subscriber_id + "' AND book_id3 = '" + return_book_id + "'";
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("book_id3 column successfully updated"); //validation check
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { sqlConnection.Close(); }

        }

        //this method remove book id from subscriber and add copy to papaerbook
        public void addCopyAndRmBookInDB(string subscriber_id, string return_book_id, int copies)
        {
            SqlConnectionStringBuilder conn_str = new SqlConnectionStringBuilder();
            conn_str.ConnectionString = @"Server=localhost\SQLEXPRESS;Database=Avi_DB;Trusted_Connection=True;";

            SqlConnection sqlConnection = new SqlConnection(conn_str.ConnectionString);
            sqlConnection.Open();
            // create command to run the query
            using SqlCommand cmd = new SqlCommand
            {
                Connection = sqlConnection,
                CommandType = System.Data.CommandType.Text,
            };
            try
            {
                //add Copy to papaerbook part
                cmd.CommandText = "UPDATE books SET copies = '" + copies + "' WHERE book_id = '" + return_book_id + "' ";
                cmd.ExecuteNonQuery();
                Console.WriteLine($"added 1 copy for {return_book_id} id in the database");
                //remove book id from Subscriber part
                cmd.CommandText = "SELECT COUNT(*) FROM subscribers WHERE subscriber_id = '" + subscriber_id + "' AND (book_id1 = '" + return_book_id + "')";
                int count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    cmd.CommandText = "UPDATE subscribers SET book_id1 = NULL WHERE subscriber_id = '" + subscriber_id + "' AND book_id1 = '" + return_book_id + "'";
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("book_id1 column successfully updated"); //validation check
                }
                cmd.CommandText = "SELECT COUNT(*) FROM subscribers WHERE subscriber_id = '" + subscriber_id + "' AND (book_id2 = '" + return_book_id + "')";
                count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    cmd.CommandText = "UPDATE subscribers SET book_id2 = NULL WHERE subscriber_id = '" + subscriber_id + "' AND book_id2 = '" + return_book_id + "'";
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("book_id2 column successfully updated"); //validation check
                }
                cmd.CommandText = "SELECT COUNT(*) FROM subscribers WHERE subscriber_id = '" + subscriber_id + "' AND (book_id3 = '" + return_book_id + "')";
                count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    cmd.CommandText = "UPDATE subscribers SET book_id3 = NULL WHERE subscriber_id = '" + subscriber_id + "' AND book_id3 = '" + return_book_id + "'";
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("book_id3 column successfully updated"); //validation check
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { sqlConnection.Close(); }
        }


        //borrowBook method
        public void borrowBook()
        {
            bool isExistBook = false;
            Console.WriteLine("please enter:\nSubscriber's id\nbook id OR book name\neach value at a separate line\n");
            string Subscriber_id = Console.ReadLine();
            //checking valid Subscriber id and if subscriber is Exist
            if (!(IsValiSubscriberdId(Subscriber_id))) { return; }
            if (_subscribers.Count == 0) { Console.WriteLine("there are no Subscribers in the library, please add some Subscribers :)"); return; }
            if (_subscribers.ContainsKey(Subscriber_id) == false) { Console.WriteLine("Subscriber id does not exist"); return; }
            if (_subscribers[Subscriber_id].getCountBooks() == 3) { Console.WriteLine("subscriber has maximum number of allowed books on loan"); return; }
            subscriber subscriber = _subscribers[Subscriber_id]; //create subscriber obj by Accessing to value using the key
            string input_book = Console.ReadLine(); //.ToUpper();
            if (_books.Count == 0) { Console.WriteLine("there are no books in the library, please add some books :) "); return; }
            if (!(IsValidString(input_book) || IsValiBookdId(input_book))) { Console.WriteLine("wrong input, enter valid book id OR book name"); return; }
            if (_books.ContainsKey(input_book)) { borrowisPaperOrDidital(_books[input_book], subscriber, Subscriber_id); return; }// if user enter book_id and we found it
                                                                                                                                  // -> activate borrow and out
            Dictionary<string, Book> books_match_dic = new Dictionary<string, Book>();
            foreach (KeyValuePair<string, Book> element in _books)
            {
                string book_name_from_dic = element.Value.getBookName(); //get book name from books Dictionary
                string book_id_from_dic = element.Key; //get book id from books Dictionary
                if (book_name_from_dic == input_book) //case user enter book name 
                {
                    books_match_dic.Add(book_id_from_dic, element.Value);
                    isExistBook = true;
                }
            }
            if (isExistBook == false) { Console.WriteLine("no book id OR book name as you enter"); return; }
            //case user enter book name:
            Console.WriteLine("book name exist!");
            if (books_match_dic.Count == 1) //case 1 match only
            {
                foreach (var book in books_match_dic)
                {
                    borrowisPaperOrDidital(book.Value, subscriber, Subscriber_id);
                    // we call to sql methods inside borrowisPaperOrDidital() method
                }
            }
            else if (books_match_dic.Count > 1)
            {
                //in case we have more than 1 match, displaying list of books matches to the user
                Console.WriteLine("please choose book from the next list: (choose by book id)");
                foreach (var item in books_match_dic)
                {
                    Console.WriteLine($"book id: {item.Key}");
                }
                string user_book_id = Console.ReadLine();
                bool matchBookExist = false;
                while (!matchBookExist) //end loop if matchBookExist==true
                {
                    if (IsValiBookdId(user_book_id) == false) { Console.WriteLine("try again"); user_book_id = Console.ReadLine(); continue; }
                    else if (!_books.ContainsKey(user_book_id)) { Console.WriteLine("book id does not exist in library, please choose book id from the list"); user_book_id = Console.ReadLine(); continue; }
                    else if (!books_match_dic.ContainsKey(user_book_id)) { Console.WriteLine("you have not choose book id from the list, try again"); user_book_id = Console.ReadLine(); continue; }
                    else
                        matchBookExist = true;
                }
                if (user_book_id == null) { Console.WriteLine("out the program to menu"); return; }
                Book bookmatch = _books[user_book_id]; //bookmatch is the obj who has key in place user_book_id, it could be paper or digital
                Console.WriteLine("book id exist!");
                borrowisPaperOrDidital(bookmatch, subscriber, Subscriber_id);
                //we call to sql methods inside borrowisPaperOrDidital() method
            }
        }
        //this method add book id to subscriber and remove copy from papaerbook
        public void rmCopyAndAddBookInDB(string book_id, int copies, string subscriber_id, string book_id1, string book_id2, string book_id3)
        {
            SqlConnectionStringBuilder conn_str = new SqlConnectionStringBuilder();
            conn_str.ConnectionString = @"Server=localhost\SQLEXPRESS;Database=Avi_DB;Trusted_Connection=True;";

            SqlConnection sqlConnection = new SqlConnection(conn_str.ConnectionString);
            sqlConnection.Open();
            // create command to run the query
            using SqlCommand cmd = new SqlCommand
            {
                Connection = sqlConnection,
                CommandType = System.Data.CommandType.Text,
            };
            try
            {
                //rmCopy part
                cmd.CommandText = "UPDATE books SET copies = '" + copies + "' WHERE book_id = '" + book_id + "' ";
                cmd.ExecuteNonQuery();
                Console.WriteLine($"remove 1 copy of {book_id} id in the database");
                //add book id to Subscriber part
                if (book_id1 != null)
                {
                    cmd.CommandText = "UPDATE subscribers SET book_id1 = '" + book_id1 + "' WHERE Subscriber_id = '" + subscriber_id + "' ";
                    cmd.ExecuteNonQuery();
                }
                if (book_id2 != null)
                {
                    cmd.CommandText = "UPDATE subscribers SET book_id2 = '" + book_id2 + "' WHERE Subscriber_id = '" + subscriber_id + "' ";
                    cmd.ExecuteNonQuery();
                }
                if (book_id3 != null)
                {
                    cmd.CommandText = "UPDATE subscribers SET book_id3 = '" + book_id3 + "' WHERE Subscriber_id = '" + subscriber_id + "' ";
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine($"subscriber's book list of subscriber: '{subscriber_id}' Successfully updated in database");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { sqlConnection.Close(); }

        }
        //this method use to check if book is paper or digital and adds it to the list of subscriber books accordingly
        public void borrowisPaperOrDidital(Book book, subscriber subscriber, string Subscriber_id)
        {
            //case paper
            if (book is PaperBook)
            {
                PaperBook pbook = (PaperBook)book;
                if (pbook.getCopy() > 0)
                {
                    subscriber.addBook(pbook);
                    Console.WriteLine("success, you boorow paper book");
                    Console.WriteLine($"current books of subscriber {Subscriber_id}: {subscriber.getCountBooks()}");
                    pbook.RmCopy();
                    Console.WriteLine("current number copies of the book id '" + pbook.getBookId() + "' are: " + pbook.getCopy()); //check for me
                    string bookId = pbook.getBookId();
                    int copies = pbook.getCopy();
                    //check if subscriberBooks list is bigger than 1 or 2
                    string book_id1 = subscriber.getSubscriberBooks()[0].getBookId();
                    string book_id2 = null;
                    string book_id3 = null;
                    if (subscriber.getCountBooks() > 1)
                    {
                        book_id2 = subscriber.getSubscriberBooks()[1].getBookId();
                    }
                    if (subscriber.getCountBooks() > 2)
                    {
                        book_id3 = subscriber.getSubscriberBooks()[2].getBookId();
                    }
                    //calling sql method
                    rmCopyAndAddBookInDB(bookId, copies, Subscriber_id, book_id1, book_id2, book_id3);
                    return;
                }
                else
                {
                    Console.WriteLine("but all copies of the book are already taken, try another book");
                    return;
                }
            }
            //case digital
            if (book is digitalBook)
            {
                subscriber.addBook(book);
                Console.WriteLine("success, you boorow digital book");
                Console.WriteLine($"current books of subscriber {Subscriber_id}: {subscriber.getCountBooks()}");
                //check if subscriberBooks list is bigger than 1 or 2
                string book_id1 = subscriber.getSubscriberBooks()[0].getBookId();
                string book_id2 = null;
                string book_id3 = null;
                if (subscriber.getCountBooks() > 1)
                {
                    book_id2 = subscriber.getSubscriberBooks()[1].getBookId();
                }
                if (subscriber.getCountBooks() > 2)
                {
                    book_id3 = subscriber.getSubscriberBooks()[2].getBookId();
                }
                //calling sql method
                addBookIdToSub(Subscriber_id, book_id1, book_id2, book_id3);
                return;
            }
        }
        //this method add book id to subscribers table
        public void addBookIdToSub(string subscriber_id, string book_id1, string book_id2, string book_id3)
        {
            SqlConnectionStringBuilder conn_str = new SqlConnectionStringBuilder();
            conn_str.ConnectionString = @"Server=localhost\SQLEXPRESS;Database=Avi_DB;Trusted_Connection=True;";

            SqlConnection sqlConnection = new SqlConnection(conn_str.ConnectionString);
            sqlConnection.Open();
            // create command to run the query
            using SqlCommand cmd = new SqlCommand
            {
                Connection = sqlConnection,
                CommandType = System.Data.CommandType.Text,
            };
            try
            {
                if (book_id1 != null)
                {
                    cmd.CommandText = "UPDATE subscribers SET book_id1 = '" + book_id1 + "' WHERE Subscriber_id = '" + subscriber_id + "' ";
                    cmd.ExecuteNonQuery();
                }
                if (book_id2 != null)
                {
                    cmd.CommandText = "UPDATE subscribers SET book_id2 = '" + book_id2 + "' WHERE Subscriber_id = '" + subscriber_id + "' ";
                    cmd.ExecuteNonQuery();
                }
                if (book_id3 != null)
                {
                    cmd.CommandText = "UPDATE subscribers SET book_id3 = '" + book_id3 + "' WHERE Subscriber_id = '" + subscriber_id + "' ";
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine($"subscriber's book list of subscriber: '{subscriber_id}' Successfully updated in database");

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { sqlConnection.Close(); }
        }

        //*********************************** End Of Returning & Borrow Book Methods *****************************************
    }

}

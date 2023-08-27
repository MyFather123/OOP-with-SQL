//main
using ex3;

Library l1 = new Library();
//sql
Db_work db_Work = new Db_work();
db_Work.create_Avi_DB_and_tables();

//Add initail books & subscribers to library
l1.initialDataInLibrary();

bool flag_done = true;
while (flag_done)
{
    Console.WriteLine("\nenter number between 1-8");
    Console.WriteLine("1. Adding a new book to the library database");
    Console.WriteLine("2. Adding a new Subscriber to the library\n3. Borrow a book by Subscriber");
    Console.WriteLine("4. Returning a book by subscriber\n5. Printing book details");
    Console.WriteLine("6. Printing books belonging to a specific genre\n7. Print subscriber books\n8. Ending program\n");
    if (!Enum.TryParse(Console.ReadLine(), out actions enum_action))
    {
        Console.WriteLine("wrong input, try again to enter number between 1-8\n");
        continue; //if user does not enter number between 1-8
    }
    switch (enum_action)
    {
        case actions.AddingBook: l1.addBook(); break; //case 1
        case actions.AddingSubscriber: l1.addSubscriber(); break; //case 2
        case actions.borrowBook: l1.borrowBook(); break; //case 3
        case actions.Returning_Book_By_Subscriber: l1.ReturningBook(); break; //case 4
        case actions.Printing_book_details: l1.printBookDetails(); break; //case 5
        case actions.Printing_books_by_genre: l1.printBooksByGenre(); break; //case 6
        case actions.Printing_books_of_subscriber: l1.printSubscriberBooks(); break; //case 7
        case (actions)8: flag_done = false; break; //case 8
        default: Console.WriteLine("you have not enter number between 1-8, try again"); break; //verify input again
    }
}
Console.WriteLine("good bye!");

enum actions
{
    AddingBook = 1,
    AddingSubscriber = 2,
    borrowBook = 3,
    Returning_Book_By_Subscriber = 4,
    Printing_book_details = 5,
    Printing_books_by_genre = 6,
    Printing_books_of_subscriber = 7,
};


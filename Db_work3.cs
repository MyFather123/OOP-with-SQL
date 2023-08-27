using System.Data.SqlClient;

namespace ex3;

class Db_work
{


    //create initial DB via master connection
    public void create_db(string db_name)
    {
        SqlConnectionStringBuilder conn_str = new SqlConnectionStringBuilder();
        conn_str.ConnectionString = @"Server=localhost\SQLEXPRESS;Database=master;Trusted_Connection=True;";

        SqlConnection sqlConnection = new SqlConnection(conn_str.ConnectionString);
        String str = @"CREATE DATABASE " + db_name;

        SqlCommand myCommand = new SqlCommand(str, sqlConnection);
        try
        {
            // drop Avi_DB if already exist
            myCommand.CommandText = "DROP DATABASE IF EXISTS Avi_DB";
            myCommand.CommandType = System.Data.CommandType.Text;

            sqlConnection.Open();
            myCommand.ExecuteNonQuery();

            myCommand.CommandText = str;
            myCommand.ExecuteNonQuery();
            Console.WriteLine("DataBase deleted and recreated Successfully");
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Continuing regardless of try catch");
        }
        finally
        {
            if (sqlConnection.State == System.Data.ConnectionState.Open)
            {
                sqlConnection.Close();
            }
        }

    }

    // create Avi DB and tables
    public void create_Avi_DB_and_tables()
    {
        create_db("Avi_DB"); 
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

            cmd.CommandText = "DROP TABLE IF EXISTS subscribers";
            cmd.ExecuteNonQuery();


            cmd.CommandText = "DROP TABLE IF EXISTS books";
            cmd.ExecuteNonQuery();


            string query =
         @"CREATE TABLE dbo.books
                    (                    
                        book_id NVARCHAR(5) NOT NULL PRIMARY KEY,
                        bookName NVARCHAR(255) NOT NULL,
                        author NVARCHAR(255) NOT NULL,
                        genre NVARCHAR(255) NOT NULL,
                        paperOrDidital NVARCHAR(7) NOT NULL,
                        copies INT
                    );";


            cmd.CommandText = query;

            cmd.ExecuteNonQuery();
            cmd.CommandText = "INSERT INTO books(book_id, bookName, author, genre, paperOrDidital, copies) VALUES('111','pbook', 'galila', 'action', 'paper', 1);";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "INSERT INTO books(book_id, bookName, author, genre, paperOrDidital, copies) VALUES('11','pbook a', 'gila', 'action', 'paper', 1)";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "INSERT INTO books(book_id, bookName, author, genre, paperOrDidital, copies) VALUES('000','dbook', 'yossi', 'drama', 'digital', NULL)";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "INSERT INTO books(book_id, bookName, author, genre, paperOrDidital, copies) VALUES('00','dbook a', 'yona', 'drama', 'digital', NULL)";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "INSERT INTO books(book_id, bookName, author, genre, paperOrDidital, copies) VALUES('222','dbook b', 'eli', 'drama', 'digital', NULL)";
            cmd.ExecuteNonQuery();

            query =
            @"CREATE TABLE dbo.subscribers
                    (                    
                        Subscriber_id NVARCHAR(9) NOT NULL PRIMARY KEY,
                        fullName NVARCHAR(255) NOT NULL,
                        book_id1 NVARCHAR(5) FOREIGN KEY REFERENCES books(book_id),
                        book_id2 NVARCHAR(5) FOREIGN KEY REFERENCES books(book_id),
                        book_id3 NVARCHAR(5) FOREIGN KEY REFERENCES books(book_id)
                    );";

            cmd.CommandText = query;

            // execute
            int results = cmd.ExecuteNonQuery();



            cmd.CommandText = "INSERT INTO subscribers(Subscriber_id, fullName, book_id1, book_id2, book_id3) VALUES('01','avi revach', '222', '000', NULL)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO subscribers(Subscriber_id, fullName, book_id1, book_id2, book_id3) VALUES('12','yuval', '222', '000', NULL)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO subscribers(Subscriber_id, fullName, book_id1, book_id2, book_id3) VALUES('23','ori', '222', '000', NULL)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO subscribers(Subscriber_id, fullName, book_id1, book_id2, book_id3) VALUES('45','almog', '222', '000', NULL)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO subscribers(Subscriber_id, fullName, book_id1, book_id2, book_id3) VALUES('89','amit', '222', '000', NULL)";
            cmd.ExecuteNonQuery();
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Continuing regardless of try catch");
        }
        finally
        {
            sqlConnection.Close();
        }
    }
}

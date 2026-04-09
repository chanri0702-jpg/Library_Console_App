using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Threading;
using static PRG281_Milestone2.ManualEmail;
using System.IO;
using System.CodeDom;

namespace PRG281_Milestone2
{
    class Program
    {
        enum LibraryMenu
        {
            LoanBook = 1,
            RegisterBook,
            RegisterStudent,
            ReturnBook,
            EmailStudent,
            OverdueBooks,
            ReturnedBooks,
            FindBook,
            FindStudent,
            Loanedbooks,
            Close
        }

        static List<Loan> loans = new List<Loan>();
        static List<Book> books = new List<Book>();
        static List<Student> students = new List<Student>();
         
        static void Main(string[] args)
        {
            
            Textfile textfile = new Textfile();
            //use an anonymous delegate as the methods have return values
            Thread getStudents = new Thread(delegate ()
            { 
                students = textfile.GenerateStudents(); 
            });
            Thread getBooks = new Thread(delegate ()
            {
                books = textfile.GenerateBooks();
            });
            Thread getLoans = new Thread(delegate ()
            {
                loans = textfile.GenerateLoans();
            });
            //start thread
            getStudents.Start();
            getBooks.Start();
            getLoans.Start();
            //make main thread wait
            getStudents.Join();
            getBooks.Join();
            getLoans.Join();

            //loans.Add(new BookLoan("601635", "17", DateTime.Now.AddMonths(-3), DateTime.Now.AddMonths(-2), DateTime.Now.AddMonths(-3), false, 300));//To add data

            //get the last time an email was sent
            DateTime emailsSent = textfile.EmailSentTime();
            //DateTime emailsSent = DateTime.Parse("08/18/2025");


            DateTime now = DateTime.Now.Date.AddHours(8);
            DateTime nextRunTime = now.AddDays(1);
            TimeSpan firstRunDelay;
            //its earlier than 8 and emails haven't been sent today
            if (now.TimeOfDay > DateTime.Now.TimeOfDay && emailsSent.Date != DateTime.Now.Date) { 
                
                firstRunDelay = now - DateTime.Now;
                
            }
            //it's later than 8 and emails haven't been sent today
            else if(now.TimeOfDay < DateTime.Now.TimeOfDay && emailsSent.Date!=DateTime.Now.Date)
            {
                firstRunDelay = TimeSpan.FromMilliseconds(1);
            }
            // emails have been sent today
            else
            {
                firstRunDelay = nextRunTime - DateTime.Now;
                nextRunTime = now.AddDays(2);
            }


            int firstRunMS = (int)firstRunDelay.TotalMilliseconds;


            //get the milliseconds for the time span to pass it to the timer
            TimeSpan nextrun = nextRunTime - DateTime.Now;
            int nextRunTimeMS = (int)nextrun.TotalMilliseconds;

            AutoEmail emails= new AutoEmail();
            
            //generate timer. A timer uses threading to run in background
            Timer timer = new Timer(del=> { emails.GenerateEmails(loans, books, students, emails.SendEmail); }, null, firstRunMS, nextRunTimeMS);

            //clear text files
            //textfile.deleteFiles();

            ManualEmail mailer = new ManualEmail();//subscribe email event
            bool running = true;

            //Loop for the enum
            while (running)
            {
                Console.WriteLine("\n===== LIBRARY MENU =====");
                Console.WriteLine("1. Loan a Book");
                Console.WriteLine("2. Register a Book");
                Console.WriteLine("3. Register Student");
                Console.WriteLine("4. Return a Book");
                Console.WriteLine("5. Email a Student");
                Console.WriteLine("6. Show all Overdue Books");
                Console.WriteLine("7. Show Returned Books");
                Console.WriteLine("8. Find a Book");
                Console.WriteLine("9. Find a Student");
                Console.WriteLine("10. Show all Loaned Books");
                Console.WriteLine("11. Close");
                Console.Write("Enter choice: ");

                //store enum choice
                string input = Console.ReadLine();
                if (int.TryParse(input, out int option) && Enum.IsDefined(typeof(LibraryMenu), option)) //make sure enum option was chosen
                {
                    LibraryMenu selectedOption = (LibraryMenu)option;
                    switch (selectedOption)
                    {
                        case LibraryMenu.LoanBook:
                            try
                            {
                                Console.Write("\nEnter Student ID: ");
                                string studentId = Console.ReadLine().Trim();
                                if (studentId.Length != 6 && int.TryParse(studentId, out int studentid))
                                {
                                    throw new MyException("Invalid student number. StudentID must be in the format of 012345 with no spaces.");
                                }
                                Console.Write("Enter Book ID: ");
                                string bookId = Console.ReadLine().Trim();
                                if ( !int.TryParse(bookId, out int bookid))
                                {
                                    throw new MyException("Invalid book ID number.");
                                }
                                //use LINQ to find if the book is currently being loaned
                                var findloan = from loan in loans where (loan.BookID == bookId) && (loan.IsReturned==false)
                                                      select new {loan.BookID };
                                if (findloan.Any() )
                                {
                                    throw new MyException("Book is currently loaned by another.");
                                }
                                Console.WriteLine("How much is the fine?");
                                if (double.TryParse(Console.ReadLine().Trim(), out double fine))
                                {
                                    Console.WriteLine("Is book a textbook? Y/N");
                                    string answer = Console.ReadLine().Trim();
                                    if (answer == "Y")
                                    {
                                        //textbook is being loaned
                                        Console.WriteLine("Enter the cost of the book");
                                        if (double.TryParse(Console.ReadLine().Trim(), out double cost))
                                        {
                                            //textbooks allow custom due dates
                                            Console.WriteLine("Enter the date the book must be returned in the format mm/dd/yyyy");
                                            DateTime dueDate = DateTime.Parse(Console.ReadLine());
                                            if (dueDate.Date<DateTime.Now.Date)
                                            {
                                                throw new Exception("Due date can only be today or in the future.");
                                            }
                                            else
                                            {
                                                LoanTextbook(studentId, bookId,dueDate, fine, cost);
                                            }
                                            
                                        }
                                        else
                                        {
                                            throw new Exception("Invalid Textbook cost. Only enter a number.");
                                        }
                                    }
                                    //normal book is being loaned
                                    else if (answer == "N")
                                    {
                                        LoanBook(studentId, bookId, fine);
                                    }
                                    else
                                    {
                                        throw new MyException("Invalid input. Only type in Y or N");
                                    }
                                }
                                else
                                {
                                    throw new MyException("Invalid fine. Only enter a number.");
                                }
                            }
                            //display exception that was caught in red
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n" + ex.Message + "\n");
                                Console.ResetColor();
                            }

                            break;
                        case LibraryMenu.RegisterBook:
                            try
                            {
                                Console.Write("\nEnter Book's ID number without spaces: ");
                                string serialNum = Console.ReadLine().Trim();
                                if (!int.TryParse(serialNum, out int serialnum))
                                {
                                    throw new MyException("Invalid book ID number.");
                                }
                                if (books.FindIndex(b => b.BookID == serialNum) != -1)
                                {
                                    throw new MyException("Book is already registered.");
                                }
                                Console.Write("Enter Title: ");
                                string title = Console.ReadLine().Trim();
                                if (title.Length==0) //test if a title was entered
                                {
                                    throw new MyException("Title field cannot be left empty.");
                                }
                                Console.WriteLine("Enter Author(s) (comma separated): ");
                                string authorList= Console.ReadLine().Trim();
                                string[] authors = authorList.Split(',');
                                if (authorList.Length<3 )//test if a name was entered 
                                {
                                    throw new MyException("Invalid Authors.");
                                }
                                if (!authorList.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)|| c==','))//test if a name was entered and if it only has letters
                                {
                                    throw new MyException("Invalid Authors.");
                                }
                                authors = authors.Select(author => author.Trim()).ToArray();//remove white spaces before and after author names in array using LINQ
                                RegisterBook(serialNum, title, authors.ToList());
                            }
                            //display caught exceptions in red
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n" + ex.Message + "\n");
                                Console.ResetColor();
                            }

                            break;
                        case LibraryMenu.RegisterStudent:
                            try 
                            {
                                Console.Write("\nEnter Student ID: ");
                                string id = Console.ReadLine();
                                if (id.Length != 6 && int.TryParse(id, out int studentid))//make sure student number only has 6 numbers
                                {
                                    throw new MyException("Invalid student number. StudentID must be in the format of 012345 with no spaces.");
                                }
                                if (students.FindIndex(s => s.StudentID == id) != -1)
                                {
                                    throw new MyException("Student is already registered.");
                                }
                                Console.Write("Enter First Name: ");
                                string name = Console.ReadLine().Trim();
                                if (name.ToUpper().Substring(0, 1) != name.Substring(0, 1) && !name.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))//test if first letter is a capital letter & name only has letters
                                {
                                    throw new MyException("Invalid name. Name must start with a capital letter and consist only of letters.");
                                }
                                Console.Write("Enter Surname: ");
                                string surname = Console.ReadLine().Trim();
                                if (!surname.All(c=>char.IsLetter(c) || char.IsWhiteSpace(c)) || surname.Length<1 )// use LINQ to make sure surname only consist of letters
                                {
                                   throw new MyException("Invalid surname. Surname must only contain letters and spaces.");
                                }
                                Console.Write("Enter Email: ");
                                string email = Console.ReadLine().Trim();
                                if (!email.Contains("@") && email.Length>6)//make sure email is valid
                                {
                                    throw new MyException("Invalid email.");
                                }
                                Console.Write("Enter Phone: ");
                                string phone = Console.ReadLine().Trim();
                                if (phone.Length!=10 && int.TryParse(phone, out int num))//make sure phone num is valid
                                {
                                    throw new MyException("Invalid phone number. Phone number must be in the format of 0123456789 with no spaces.");
                                }
                                //only create object if all inputs are valid
                                RegisterStudent(id, name, surname, email, phone);
                            }
                            //display all caught exceptions in red
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n" + ex.Message + "\n");
                                Console.ResetColor();
                            }

                            break;
                        case LibraryMenu.ReturnBook:
                            Console.Write("\nEnter Book ID to return: ");
                            string returnId = Console.ReadLine().Trim();
                            ReturnBook(returnId); //call method to return book
                            break;
                        case LibraryMenu.EmailStudent: 
                            try
                            {
                                Console.Write("Enter Student Email: ");
                                string email = Console.ReadLine().Trim();
                                Console.WriteLine("Enter Subject: ");
                                string subject = Console.ReadLine().Trim();
                                Console.WriteLine("Enter Message: ");
                                string message = Console.ReadLine().Trim();
                                mailer.SendEmail(email, subject, message);//send email using class method from Manual email helper class
                            }
                            //display caught exceptions in red
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n" + ex.Message + "\n");
                                Console.ResetColor();
                            }

                            break;
                        case LibraryMenu.OverdueBooks: 
                            ShowOverdueBooks(); //call method
                            break;
                        case LibraryMenu.ReturnedBooks:
                            try
                            {
                                Console.WriteLine("Enter the first date in the range of returns you are looking for in the format mm/dd/yyyy");
                                DateTime date1 = DateTime.Parse(Console.ReadLine().Trim());
                                Console.WriteLine("Enter the second date in the range of returns you are looking for in the format mm/dd/yyyy");
                                DateTime date2 = DateTime.Parse(Console.ReadLine().Trim());
                                IEnumerable<Loan> returns = ShowReturnedBooks(date1,date2);//call method to show returned books
                                if (returns.Any())
                                {
                                    returns.OrderBy(loan => loan.ReturnDate);//return list as sorted using IComparer in class automatically
                                    int count = 0;
                                    //get all the necessary info on the loan using LINQ
                                    var displayReturns = from ret in returns
                                                join book in books on ret.BookID equals book.BookID
                                                join stu in students on ret.StudentID equals stu.StudentID
                                                select new { book.Title, book.Author, ret.ReturnDate, ret.DateDue, 
                                                    fine = ret.CalculateFine(), stu.Name, stu.Surname, stu.StudentID, ret.BookID};
                                    //display loan info
                                    if (displayReturns.Any())
                                    {
                                        Console.WriteLine("\nThe following books were returned in that timeperiod:");
                                        foreach (var item in displayReturns)
                                        {
                                            count++;
                                            Console.WriteLine($"{count}. {item.Title} by {string.Join(", ", item.Author)}, BookID: {item.BookID}, was returned on {item.ReturnDate} and was due " +
                                                $"{item.DateDue} with a payable fine of R{item.fine} by {item.Name} {item.Surname}, Student ");
                                        }
                                    }
                                    else
                                    {
                                        throw new MyException("Missing student or book record.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("No books were returned within that timeperiod.");
                                }

                            }
                            //display exceptions in red
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n"+ex.Message+"\n");
                                Console.ResetColor();
                            }
                                ; break;
                        case LibraryMenu.FindBook: 
                            try
                            {
                                FindBook(); //call method
                            }
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n" + ex.Message + "\n");
                                Console.ResetColor();
                            }
                            
                            break;
                        case LibraryMenu.FindStudent:
                            try
                            {
                                FindStudent(); //call method
                            }
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n" + ex.Message + "\n");
                                Console.ResetColor();
                            }

                            break;
                        case LibraryMenu.Loanedbooks:
                            ShowLoanedBooks();
                            break;
                        case LibraryMenu.Close:
                            //use an anonymous delegate with thread due to methods needing a parameter
                            Thread saveStudents = new Thread(delegate ()
                            {
                                textfile.AppendStudents(students);
                            });
                            Thread saveBooks = new Thread(delegate ()
                            {
                                textfile.AppendBooks(books);
                            });
                            Thread saveLoans = new Thread(delegate ()
                            {
                                textfile.AppendLoans(loans);
                            });
                            //start threads
                            saveStudents.Start();
                            saveBooks.Start();
                            saveLoans.Start();
                            //make threads finish before main program continues
                            saveStudents.Join();
                            saveBooks.Join();
                            saveLoans.Join();
                            running = false; 
                            break;
                        
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid input. Please enter a number from the menu.");
                    Console.ResetColor();
                }
            }
        }

        // ========== FUNCTIONS ==========

        static void RegisterBook(string id, string title, List<string> authors)//method to register new book
        {

            Book newBook = new Book (id, title, authors);
            books.Add(newBook);

            Console.WriteLine("\nBook registered successfully!\n");
        }

        static void RegisterStudent(string id, string name, string surname, string email, string phone) //method to register new student
        {
            Student student = new Student(id, name, surname, email, phone);//custom constructor from Student class
            students.Add(student);

            Console.WriteLine("\nStudent registered successfully!\n");
        }

        static void LoanBook(string studentId, string bookId, double fine )
        {
            //test if student & book exist
            var student = students.Find(s => s.StudentID == studentId);
            var book = books.Find(b => b.BookID == bookId);

            if (student != null && book != null)
            {
                Loan loan = new BookLoan(studentId, bookId, DateTime.Now, DateTime.Now.AddDays(21), DateTime.Now ,false, fine);//use class custom constructor to create object
                loans.Add(loan);
                Console.WriteLine("\nBook loaned successfully!\n");
            }
            else if (student == null)
            {
                throw new MyException("Student doesn't exist.");

            }
            else if (book == null)
            {
                throw new MyException("Book doesn't exist.");

            }
        }
        static void LoanTextbook(string studentId, string bookId,DateTime due, double fine, double cost)
        {


            var student = students.Find(s => s.StudentID == studentId);
            var book = books.Find(b => b.BookID == bookId);

            if (student != null && book != null)
            {
                Loan loan = new TextbookLoan(studentId, bookId, DateTime.Now,due, DateTime.Now, false , fine, cost);//use class custom constructor to create object
                loans.Add(loan);
                Console.WriteLine("\nTextook loaned successfully!\n");
            }
            else if (student == null)
            {
                throw new MyException("Student doesn't exist.");

            }
            else if (book == null)
            {
                throw new MyException("Book doesn't exist.");

            }
        }

        static void ReturnBook(string bookId) //method to find and return book using the book's ID
        {
            //look for loan record
            var loan = loans.Find(l => l.BookID == bookId && !l.IsReturned);
            
            if (loan != null)
            {
                loan.ReturnBook();//class method to return a book
                Console.WriteLine($"\nFine: R{loan.CalculateFine()}");
                Console.WriteLine("Book returned successfully.\n");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nNo active loan found for this book.\n");
                Console.ResetColor();
            }
        }



        static void ShowOverdueBooks()
        {
            int count = 0;
            var displayBooks = from loan in loans
                        join book in books on loan.BookID equals book.BookID
                        join stu in students on loan.StudentID equals stu.StudentID
                        where !loan.IsReturned && loan.DateDue.Date<=DateTime.Now.Date
                        select new
                        {
                            book.Title,
                            book.Author,
                            book.BookID,
                            loan.DateLoaned,
                            loan.DateDue,
                            fine = loan.CalculateFine(),
                            stu.Name,
                            stu.Surname,
                            stu.StudentID
                        };
            if (displayBooks.Any())
            {
                Console.WriteLine("\nThe following books are overdue:");

                foreach (var item in displayBooks)
                {
                    count++;
                    Console.WriteLine($"{count}. {item.Title} by {string.Join(", ", item.Author)}, BookID: {item.BookID}, was loaned on {item.DateLoaned} and was due " +
                        $"{item.DateDue} with a payable fine of R{Math.Round(item.fine,2)} by {item.Name} {item.Surname}, StudentID: {item.StudentID} ");
                }
            }
            else
            {
                Console.WriteLine("\nThere are no overdue books.\n");
            }
            
        }
        static void ShowLoanedBooks()
        {
            int count = 0;
            var displayBooks = from loan in loans
                               join book in books on loan.BookID equals book.BookID
                               join stu in students on loan.StudentID equals stu.StudentID
                               where !loan.IsReturned 
                               select new
                               {
                                   book.Title,
                                   book.Author,
                                   book.BookID,
                                   loan.DateLoaned,
                                   loan.DateDue,
                                   fine = loan.CalculateFine(),
                                   stu.Name,
                                   stu.Surname,
                                   stu.StudentID
                               };
            if (displayBooks.Any())
            {
                Console.WriteLine("\nThe following books are currently loaned:");

                foreach (var item in displayBooks)
                {
                    count++;
                    Console.WriteLine($"{count}. {item.Title} by {string.Join(", ", item.Author)}, BookID: {item.BookID}, was loaned on {item.DateLoaned} and is due " +
                        $"{item.DateDue} with a payable fine of R{item.fine} by {item.Name} {item.Surname}, StudentID: {item.StudentID} ");
                }
            }
            else
            {
                Console.WriteLine("\nThere are no overdue books.\n");
            }

        }

        static IEnumerable<Loan> ShowReturnedBooks(DateTime date1, DateTime date2)
        {
            //test how to sort dates
            if (date1 < date2)
            {
                IEnumerable<Loan> loanList = loans.Where(loan => loan.ReturnDate.Date >= date1.Date && loan.ReturnDate.Date <= date2.Date && loan.IsReturned);
                return loanList;
            }
            else
            {
                 IEnumerable<Loan> loanList = loans.Where(loan => loan.ReturnDate.Date >= date2.Date && loan.ReturnDate.Date <= date1.Date && loan.IsReturned);
                return loanList;
            }
            

        }

        static void FindBook()
        {
            Console.Write("Enter Book ID: ");
            string id = Console.ReadLine().Trim();
            var book = books.Find(b => b.BookID == id);
            if (book != null) book.Print();//use class method to display info
            else throw new MyException("\nBook not found.\n");
        }

        static void FindStudent()
        {
            Console.Write("Enter Student ID: ");
            string id = Console.ReadLine().Trim();
            var student = students.Find(s => s.StudentID == id);
            if (student != null) student.Print();//use class method to display info
            else throw new MyException("Student not found.");
        }
        
    }
}
    


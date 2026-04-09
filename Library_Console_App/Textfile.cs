using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PRG281_Milestone2
{
    internal class Textfile
    {
        public List<Student> GenerateStudents()
        {
            List<Student> students = new List<Student>();
            if (!File.Exists("Students.txt")) //generate textfile with some data if it doesn't exist
            {
                StreamWriter write = new StreamWriter("Students.txt");
                write.WriteLine("601635#Chanri#Du Randt#chanri0702@gmail.com#0767704150");
                write.Close();
            }
                //extract data from textfile
                StreamReader reader = new StreamReader("Students.txt");
            string line;
            line = reader.ReadLine();
            while (line != null)
            {
                string[] student = line.Split('#');
                students.Add(new Student(student[0], student[1], student[2], student[3], student[4]));
                line = reader.ReadLine();
            }
            reader.Close();
            //return extracted data
            return students;
        }
        public List<Book> GenerateBooks()
        {
            List<Book> books = new List<Book>();
            if (!File.Exists("Books.txt")) //generate textfile with some data if it doesn't exist
            {
                StreamWriter writeBooks = new StreamWriter("Books.txt");
                writeBooks.WriteLine("1#The Great Gatsby#F. Scott Fitzgerald");
                writeBooks.WriteLine("2#Principles of Compiler Design#Alfred Aho,Jeffrey Ullman");
                writeBooks.WriteLine("3#1984#George Orwell");
                writeBooks.WriteLine("4#Pride and Prejudice#Jane Austen");
                writeBooks.WriteLine("5#The Catcher in the Rye#J.D. Salinger");
                writeBooks.WriteLine("6#Moby-Dick#Herman Melville");
                writeBooks.WriteLine("7#War and Peace#Leo Tolstoy");
                writeBooks.WriteLine("8#The Hobbit#J.R.R. Tolkien");
                writeBooks.WriteLine("9#Crime and Punishment#Fyodor Dostoevsky");
                writeBooks.WriteLine("10#Brave New World#Aldous Huxley");
                writeBooks.WriteLine("11#Jane Eyre#Charlotte Brontë");
                writeBooks.WriteLine("12#Wuthering Heights#Emily Brontë");
                writeBooks.WriteLine("13#The Odyssey#Homer");
                writeBooks.WriteLine("14#Les Misérables#Victor Hugo");
                writeBooks.WriteLine("15#The Divine Comedy#Dante Alighieri");
                writeBooks.Close();
            }
            //extract data from textfile
            StreamReader readerBooks = new StreamReader("Books.txt");
            string line;
            line = readerBooks.ReadLine();
            while (line != null)
            {
                string[] book = line.Split('#');
                List<string> authors = book[2].Split(',').ToList();
                books.Add(new Book(book[0], book[1], authors));
                line = readerBooks.ReadLine();
            }
            readerBooks.Close();
            //return extracted data
            return books;

        }
        public List<Loan> GenerateLoans()
        {
            List<Loan> loans = new List<Loan>();
            if (!File.Exists("Loans.txt"))//generate textfile with some data if it doesn't exist
            {
                StreamWriter writeLoans = new StreamWriter("Loans.txt");
                writeLoans.WriteLine("601635#1#08/18/2025#08/20/2025#08/18/2025#0#100");
                writeLoans.WriteLine("601635#2#08/18/2025#08/20/2025#08/18/2025#0#100#600");
                writeLoans.Close();
            }
            //get data from textfile
            StreamReader readerLoans = new StreamReader("Loans.txt");
            string line;
            line = readerLoans.ReadLine();
            while (line != null)
            {
                string[] loan = line.Split('#');
                bool returned;
                if (loan[5] == "0") { returned = false; }
                else { returned = true; }
                //determine if loan is a book or textbook
                if (loan.Length == 7)
                {
                    loans.Add(new BookLoan(loan[0], loan[1], DateTime.Parse(loan[2]), DateTime.Parse(loan[3]), DateTime.Parse(loan[4]), returned, double.Parse(loan[6])));
                }
                else if (loan.Length == 8)
                {
                    loans.Add(new TextbookLoan(loan[0], loan[1], DateTime.Parse(loan[2]), DateTime.Parse(loan[3]), DateTime.Parse(loan[4]), returned, double.Parse(loan[6]), double.Parse(loan[7])));
                }
                line = readerLoans.ReadLine();
            }
            readerLoans.Close();
            return loans;
        }
        //strore the last date automatic emails were sent out
        public DateTime EmailSentTime()
        {
            if (!File.Exists("Email.txt"))
            {
                StreamWriter writeEmail = new StreamWriter("Email.txt");
                writeEmail.WriteLine(DateTime.Now.AddDays(-1));
                writeEmail.Close();
            }
            StreamReader readerEmail = new StreamReader("Email.txt");
            string line;
            line = readerEmail.ReadLine();
            readerEmail.Close();
            return DateTime.Parse(line);
        }
        //save data entered
        public void AppendStudents(List<Student> students)
        {
            StreamWriter appendStudents = new StreamWriter("Students.txt");
            
            string line;
            foreach (Student student in students)
            {
                line = $"{student.StudentID}#{student.Name}#{student.Surname}#{student.Email}#{student.Phone}";
                appendStudents.WriteLine(line);
            }
            appendStudents.Close();
        }
        public void AppendBooks(List<Book> books)
        {
            StreamWriter appendBooks= new StreamWriter("Books.txt");

            string line;
            foreach (Book book in books)
            {
                line = $"{book.BookID}#{book.Title}#{string.Join(",", book.Author)}";
                appendBooks.WriteLine(line);
            }
            appendBooks.Close();
        }
        public void AppendLoans(List<Loan> loans)
        {
            StreamWriter appendLoans = new StreamWriter("Loans.txt");

            string line="";
            foreach (Loan loan in loans)
            {
                int returned;
                if (loan.IsReturned == true)
                {
                    returned = 1;
                }
                else
                {
                    returned = 0;
                }
                if (loan is BookLoan bl)
                {
                    
                        line = $"{loan.StudentID}#{loan.BookID}#{loan.DateLoaned}#{loan.DateDue}#{loan.ReturnDate}#{returned.ToString()}#{loan.Fine.ToString()}";
                }
                else if (loan is TextbookLoan tl)
                {
                    line = $"{loan.StudentID}#{loan.BookID}#{loan.DateLoaned}#{loan.DateDue}#{loan.ReturnDate}#{returned.ToString()}#{loan.Fine.ToString()}#{tl.Cost.ToString()}";
                }

                    appendLoans.WriteLine(line);
            }
            appendLoans.Close();
        }
        public void updateEmailSentTime()//added to event in AutoEmail to automatically update the date emails were sent out
        {
            StreamWriter newEmailTime = new StreamWriter("Email.txt");
            newEmailTime.WriteLine(DateTime.Now);
            newEmailTime.Close();
        }
        public void deleteFiles()
        {
            File.Delete("Students.txt");
            File.Delete("Books.txt");
            File.Delete("Loans.txt");
        }
    }
}
